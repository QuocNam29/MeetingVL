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
    using System.ComponentModel.DataAnnotations;

    public partial class Notification
    {
        public int ID { get; set; }
        public Nullable<int> Evalute_ID { get; set; }
        public Nullable<int> Comment_ID { get; set; }
        public string User_ID { get; set; }
        [Required(ErrorMessage = "You have not entered the Content")]
        [StringLength(250, ErrorMessage = "Stages length must be between 1 and 250.", MinimumLength = 1)]
        public string Content { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public string status { get; set; }
    
        public virtual Comment Comment { get; set; }
        public virtual Evaluate Evaluate { get; set; }
        public virtual User User { get; set; }
    }
}
