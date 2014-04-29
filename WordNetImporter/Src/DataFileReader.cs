using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordNetImporter.Src
{
    public class DataFileReader : StatusReportingObject<DataFileReader.DataFileReaderWorker>
    {
        public FileInfo[] IndexFiles { get; set; }

        public class DataFileReaderWorker : StatusReportingObject<DataFileReader.DataFileReaderWorker>.Worker
        {
            private static Regex _filter = new Regex(@"^data\.(adj|adv|noun|verb)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            private static Regex _commentLine = new Regex(@"^\s+\d+.*$", RegexOptions.Compiled);
            private static Regex _first4Fields = new Regex(@"^(?<synset_offset>\d+)\s+(?<lex_filenum>\d+)\s+(?<ss_type>[nvasr])\s+(?<w_cnt>[\da-f]+\s+\(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            private static Regex _words = new Regex(@"^(?<word>\S{1,1024}\s+(?<lex_id>[\da-f])\s+(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            private static Regex _count = new Regex(@"^(?<count>\d+)\s+(?<r>\S.*)$", RegexOptions.Compiled);
            private static Regex _pointer = new Regex(@"^(?<pointer_symbol>\S{1,2})\s+(?<synset_offset>\d+)\s+(?<pos>)[nvasr])\s+(?<src>[\da-f]{2})(?<targ>[\da-f]{2})\s+(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            private static Regex _frame = new Regex(@"^(?<f_num>\d+)\s+(?<w_num>[\da-f]{2})\s+(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            private static Regex _gloss = new Regex(@"^|\s*(?<gloss>\S+.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            private FileInfo[] _indexFiles;
            private Dest.WordNetDbEntities _dbContext;
            private string _createdBy;

            public DataFileReaderWorker(FileInfo[] indexFiles)
            {
                this._indexFiles = indexFiles;
                this._createdBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }

            public override void Invoke()
            {
                if (this._indexFiles == null)
                    return;

                this._dbContext = new Dest.WordNetDbEntities();
                try
                {
                    this.ExecutePass(true);
                    this.ExecutePass(false);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    this._dbContext.Dispose();
                }
            }

            private void ExecutePass(bool isFirst)
            {
                foreach (FileInfo fi in this._indexFiles.Where(fi => fi != null))
                {
                    if (!DataFileReaderWorker._filter.IsMatch(fi.Name))
                    {
                        this.RaiseStatusUpdate(StatusCode.Progress, "Skipping non-data file", fi.FullName);
                        continue;
                    }

                    if (!fi.Exists)
                    {
                        this.RaiseStatusUpdate(StatusCode.Warning, "File does not exist", fi.FullName);
                        continue;
                    }

                    this.RaiseStatusUpdate(StatusCode.Progress, "Processing data file", fi.FullName);

                    StreamReader reader = null;

                    try
                    {
                        reader = new StreamReader(fi.FullName, Encoding.UTF8);
                    }
                    catch (Exception error)
                    {
                        this.RaiseStatusUpdate(error, "Error creating file reader", fi.FullName);
                    }

                    if (reader == null)
                        continue;

                    try
                    {
                        this.ProcessLines(reader, isFirst);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }

            private void ProcessLines(StreamReader reader, bool isFirst)
            {
                while (!reader.EndOfStream)
                {
                    string currentLine = reader.ReadLine();

                    if (DataFileReaderWorker._commentLine.IsMatch(currentLine))
                        continue;

                    int position = 0;
                    Match m = DataFileReaderWorker._first4Fields.Match(currentLine);
                    if (!m.Success)
                    {
                        this.RaiseStatusUpdate(StatusCode.Warning, "Parse error",
                            String.Format("Position={0}, Pattern={1}\r\nLine={2}", position, DataFileReaderWorker._first4Fields.ToString(), currentLine));
                        continue;
                    }

                    SynsetData synset = new SynsetData()
                    {
                        synset_offset = Convert.ToInt64(m.Groups["synset_offset"].Value),
                        lex_filenum = Convert.ToInt16(m.Groups["lex_filenum"].Value),
                        ss_type = this.GetSynsetType(m.Groups["ss_type"].Value),
                        w_cnt = Convert.ToInt32(m.Groups["w_cnt"].Value, 16),
                        words = new Collection<SynsetWordData>(),
                        pointers = new Collection<SynsetPointerData>()
                    };

                    for (int i = 0; i < synset.w_cnt; i++)
                    {
                        position += m.Length;
                        m = DataFileReaderWorker._words.Match(m.Groups["r"].Value);
                        if (m.Success)
                            synset.words.Add(new SynsetWordData
                            {
                                word = m.Groups["word"].Value,
                                lex_id = Convert.ToInt16(m.Groups["lex_id"].Value, 16)
                            });
                        else
                        {
                            this.RaiseStatusUpdate(StatusCode.Warning, "Parse error",
                                String.Format("Position={0}, Pattern={1}\r\nLine={2}", position, DataFileReaderWorker._words.ToString(), currentLine));
                            synset.words = null;
                            break;
                        }
                    }

                    if (synset.words == null)
                        continue;

                    position += m.Length;
                    m = DataFileReaderWorker._count.Match(m.Groups["r"].Value);
                    if (!m.Success)
                    {
                        this.RaiseStatusUpdate(StatusCode.Warning, "Parse error",
                            String.Format("Position={0}, Pattern={1}\r\nLine={2}", position, DataFileReaderWorker._count.ToString(), currentLine));
                        continue;
                    }

                    synset.p_cnt = Convert.ToInt32(m.Groups["count"].Value, 16);
                    for (int i = 0; i < synset.p_cnt; i++)
                    {
                        position += m.Length;
                        m = DataFileReaderWorker._pointer.Match(m.Groups["r"].Value);
                        if (m.Success)
                            synset.pointers.Add(new SynsetPointerData
                            {
                                pointer_symbol = m.Groups["pointer_symbol"].Value,
                                synset_offset = Convert.ToInt64(m.Groups["synset_offset"].Value),
                                pos = this.GetSynsetType(m.Groups["pos"].Value),
                                src = Convert.ToInt16(m.Groups["src"].Value),
                                targ = Convert.ToInt16(m.Groups["targ"].Value)
                            });
                        else
                        {
                            this.RaiseStatusUpdate(StatusCode.Warning, "Parse error",
                                String.Format("Position={0}, Pattern={1}\r\nLine={2}", position, DataFileReaderWorker._pointer.ToString(), currentLine));
                            synset.pointers = null;
                            break;
                        }
                    }

                    if (synset.pointers == null)
                        continue;

                    if (synset.ss_type == SynsetTypeValues.v)
                    {
                        position += m.Length;
                        m = DataFileReaderWorker._count.Match(m.Groups["r"].Value);
                        if (!m.Success)
                        {
                            this.RaiseStatusUpdate(StatusCode.Warning, "Parse error",
                                String.Format("Position={0}, Pattern={1}\r\nLine={2}", position, DataFileReaderWorker._count.ToString(), currentLine));
                            continue;
                        }

                        synset.f_cnt = Convert.ToInt32(m.Groups["count"].Value);
                        for (int i = 0; i < synset.f_cnt; i++)
                        {
                            position += m.Length;
                            m = DataFileReaderWorker._frame.Match(m.Groups["r"].Value);
                            if (m.Success)
                                synset.frames.Add(new VerbFrameData
                                {
                                    f_num = Convert.ToInt16(m.Groups["f_num"].Value),
                                    w_num = Convert.ToInt16(m.Groups["w_num"].Value, 16)
                                });
                            else
                            {
                                this.RaiseStatusUpdate(StatusCode.Warning, "Parse error",
                                    String.Format("Position={0}, Pattern={1}\r\nLine={2}", position, DataFileReaderWorker._frame.ToString(), currentLine));
                                synset.pointers = null;
                                break;
                            }
                        }
                    }

                    position += m.Length;
                    m = DataFileReaderWorker._gloss.Match(m.Groups["r"].Value);
                    if (!m.Success)
                    {
                        this.RaiseStatusUpdate(StatusCode.Warning, "Parse error",
                            String.Format("Position={0}, Pattern={1}\r\nLine={2}", position, DataFileReaderWorker._gloss.ToString(), currentLine));
                        continue;
                    }

                    synset.gloss = m.Groups["gloss"].Value;

                    if (isFirst)
                        this.CreateSynsetAndWords(synset);
                    else
                        this.UpdateReferences(synset);
                }
            }

            private SynsetTypeValues GetSynsetType(string value)
            {
                SynsetTypeValues result;
                if (Enum.TryParse<SynsetTypeValues>(value, true, out result))
                    return result;

                return default(SynsetTypeValues);
            }

            private void CreateSynsetAndWords(SynsetData synsetData)
            {
                Dest.Synset synset = this._dbContext.Synsets.Create();
                synset.Id = Guid.NewGuid();
                synset.CreatedBy = this._createdBy;
                synset.CreatedOn = DateTime.Now;
                synset.Definition = synsetData.gloss;
                synset.LexFileNum = synsetData.lex_filenum;
                synset.SynsetType = (short)(synsetData.ss_type);
                synset.Offset = synsetData.synset_offset;

                this._dbContext.Synsets.Add(synset);

                this._dbContext.SaveChanges();

                for (var i = 0; i < synsetData.w_cnt; i++)
                {
                    Dest.IndexWord word = this._dbContext.IndexWords.FirstOrDefault(w => String.Compare(w.Word, synsetData.words[i].word, true) == 0);
                    if (word == null)
                    {
                        word = this._dbContext.IndexWords.Create();
                        word.Id = Guid.NewGuid();
                        word.CreatedBy = this._createdBy;
                        word.CreatedOn = DateTime.Now;
                        word.Word = synsetData.words[i].word;
                        this._dbContext.IndexWords.Add(word);
                        this._dbContext.SaveChanges();
                    }

                    Dest.SynsetWord synsetWord = this._dbContext.SynsetWords.Create();
                    synsetWord.CreatedBy = this._createdBy;
                    synsetWord.CreatedOn = DateTime.Now;
                    synsetWord.LexId = synsetData.words[i].lex_id;
                    synsetWord.Position = (short)i;
                    synsetWord.SynsetId = synset.Id;
                    synsetWord.WordId = word.Id;
                    this._dbContext.SynsetWords.Add(synsetWord);
                }

                this._dbContext.SaveChanges();
            }

            private void UpdateReferences(SynsetData synsetData)
            {
                Dest.Synset synset = this._dbContext.Synsets.FirstOrDefault(s => s.Offset == synsetData.synset_offset && s.SynsetType == (short)(synsetData.ss_type));
                if (synset == null)
                {
                    this.RaiseStatusUpdate(StatusCode.Warning, "Unable to find synset during second pass", String.Format("Synset Type = {0}, Synset Offset = {1}",
                        synsetData.ss_type, synsetData.synset_offset));
                }

                for (var i = 0; i < synsetData.p_cnt; i++)
                {
                    Dest.Synset target = this._dbContext.Synsets.FirstOrDefault(s => s.Offset == synsetData.pointers[i].synset_offset && s.SynsetType == (short)(synsetData.pointers[i].pos));
                    if (synset == null)
                    {
                        this.RaiseStatusUpdate(StatusCode.Warning, "Unable to find synset during second pass", String.Format("Synset Type = {0}, Synset Offset = {1}",
                            synsetData.ss_type, synsetData.synset_offset));
                    }

                    Dest.SynsetPointer synsetPointer = this._dbContext.SynsetPointers.Create();
                    synsetPointer.CreatedBy = this._createdBy;
                    synsetPointer.CreatedOn = DateTime.Now;
                    synsetPointer.PartOfSpeech = (short)(synsetData.pointers[i].pos);
                    synsetPointer.Position = (short)i;
                    synsetPointer.TargetSynsetId = target.Id;
                    this._dbContext.SynsetPointers.Add(synsetPointer);
                }
            }
        }

        protected override DataFileReader.DataFileReaderWorker CreateWorker()
        {
            return new DataFileReaderWorker(this.IndexFiles);
        }
    }
}
