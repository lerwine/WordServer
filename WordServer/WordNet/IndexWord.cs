//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WordServer.WordNet
{
    using System;
    using System.Collections.Generic;
    
    public partial class IndexWord
    {
        public IndexWord()
        {
            this.SynsetWords = new HashSet<SynsetWord>();
        }
    
        public System.Guid Id { get; set; }
        public string Word { get; set; }
        public System.Guid SourceId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
    
        public virtual ICollection<SynsetWord> SynsetWords { get; set; }
    }
}