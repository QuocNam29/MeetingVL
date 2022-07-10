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
    
    public partial class MeetingMinute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MeetingMinute()
        {
            this.Actions = new HashSet<Action>();
        }
    
        public int ID { get; set; }
        public string User_ID { get; set; }
        public int SessionReport { get; set; }
        public System.DateTime Date { get; set; }
        public string Location { get; set; }
        public string Objectives { get; set; }
        public string Content { get; set; }
        public string Customer { get; set; }
        public string Mentor { get; set; }
        public string TeamMember { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Action> Actions { get; set; }
        public virtual User User { get; set; }
    }
}