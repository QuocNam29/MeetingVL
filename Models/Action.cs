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

    public partial class Action
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "No user_id yet!")]
        public string User_ID { get; set; }
        [Required(ErrorMessage = "No meeting_id yet!")]
        public int Meeting_ID { get; set; }
        [Required(ErrorMessage = "You have not entered the action content")]
        public string Work { get; set; }
        [Required(ErrorMessage = "you haven't set a deadline for the action")]
        public System.DateTime Deadline { get; set; }
        public string Description { get; set; }
    
        public virtual MeetingMinute MeetingMinute { get; set; }
        public virtual User User { get; set; }
    }
}
