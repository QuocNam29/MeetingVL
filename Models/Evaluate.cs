//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeetingVL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Evaluate
    {
        public int ID { get; set; }
        public string User_ID { get; set; }
        public int Group_ID { get; set; }
        public string State { get; set; }
        public string Review { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public Nullable<int> Point { get; set; }
    
        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
    }
}
