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

    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            this.Projects = new HashSet<Project>();
        }
    
        public int ID { get; set; }
        [Required(ErrorMessage = "You have not entered the name category")]
        [StringLength(100, ErrorMessage = "Name length must be between 1 and 100.", MinimumLength = 1)]
        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "State length must be between 1 and 50.", MinimumLength = 1)]
        public string State { get; set; }
        public string ID_User { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project> Projects { get; set; }
    }
}
