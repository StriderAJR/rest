//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StridingSoft.Models.GenTrees
{
    using System;
    using System.Collections.Generic;
    
    public partial class GenTree
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string gentree_name { get; set; }
        public string gentree_json { get; set; }
    
        public virtual User User { get; set; }
    }
}
