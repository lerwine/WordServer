namespace Erwine.Leonard.T.WordServer.WordNetDataReader
{
    public class VerbFrame
    {
        public short FrameNumber { get; set; }
        public short WordNumber { get; set; }

        public VerbFrame(short f_num, short w_num)
        {
            this.FrameNumber = f_num;
            this.WordNumber = w_num;
        }
    }
}
