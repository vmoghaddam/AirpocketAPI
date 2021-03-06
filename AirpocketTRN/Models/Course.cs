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
    
    public partial class Course
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Course()
        {
            this.CourseSessions = new HashSet<CourseSession>();
            this.CourseSessionPresences = new HashSet<CourseSessionPresence>();
            this.CoursePeoples = new HashSet<CoursePeople>();
            this.CourseSessionFDPs = new HashSet<CourseSessionFDP>();
        }
    
        public int Id { get; set; }
        public int CourseTypeId { get; set; }
        public System.DateTime DateStart { get; set; }
        public Nullable<decimal> DateStartP { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public Nullable<decimal> DateEndP { get; set; }
        public string Instructor { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public Nullable<int> OrganizationId { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<int> DurationUnitId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Capacity { get; set; }
        public Nullable<int> Tuition { get; set; }
        public Nullable<int> CurrencyId { get; set; }
        public Nullable<System.DateTime> DateDeadlineRegistration { get; set; }
        public Nullable<decimal> DateDeadlineRegistrationP { get; set; }
        public string TrainingDirector { get; set; }
        public string Title { get; set; }
        public Nullable<int> AircraftTypeId { get; set; }
        public Nullable<int> AircraftModelId { get; set; }
        public Nullable<int> CaoTypeId { get; set; }
        public Nullable<bool> Recurrent { get; set; }
        public Nullable<int> Interval { get; set; }
        public Nullable<int> CalanderTypeId { get; set; }
        public Nullable<bool> IsInside { get; set; }
        public Nullable<bool> Quarantine { get; set; }
        public Nullable<System.DateTime> DateStartPractical { get; set; }
        public Nullable<System.DateTime> DateEndPractical { get; set; }
        public Nullable<decimal> DateStartPracticalP { get; set; }
        public Nullable<decimal> DateEndPracticalP { get; set; }
        public Nullable<int> DurationPractical { get; set; }
        public Nullable<int> DurationPracticalUnitId { get; set; }
        public Nullable<bool> IsGeneral { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string No { get; set; }
        public Nullable<bool> IsNotificationEnabled { get; set; }
    
        public virtual CourseType CourseType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSession> CourseSessions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSessionPresence> CourseSessionPresences { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CoursePeople> CoursePeoples { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSessionFDP> CourseSessionFDPs { get; set; }
    }
}
