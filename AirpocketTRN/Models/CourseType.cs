//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirpocketTRN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CourseType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseType()
        {
            this.CourseTypeJobGroups = new HashSet<CourseTypeJobGroup>();
            this.Courses = new HashSet<Course>();
        }
    
        public int Id { get; set; }
        public Nullable<int> CalenderTypeId { get; set; }
        public Nullable<int> CourseCategoryId { get; set; }
        public Nullable<int> LicenseResultBasicId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Interval { get; set; }
        public Nullable<bool> IsGeneral { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<int> CertificateTypeId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseTypeJobGroup> CourseTypeJobGroups { get; set; }
        public virtual CertificateType CertificateType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses { get; set; }
    }
}
