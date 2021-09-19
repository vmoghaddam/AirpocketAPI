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
    
    public partial class ViewCoursePeople
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Nullable<int> PersonId { get; set; }
        public int CourseTypeId { get; set; }
        public string CourseType { get; set; }
        public Nullable<int> CertificateTypeId { get; set; }
        public string CertificateType { get; set; }
        public System.DateTime DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public string Instructor { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public Nullable<int> OrganizationId { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<int> DurationUnitId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string Status { get; set; }
        public string TrainingDirector { get; set; }
        public string Title { get; set; }
        public bool Recurrent { get; set; }
        public Nullable<int> Interval { get; set; }
        public Nullable<int> CalanderTypeId { get; set; }
        public string No { get; set; }
        public string DurationUnit { get; set; }
        public string CalendarType { get; set; }
        public string Organization { get; set; }
        public string PID { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public string JobGroup { get; set; }
        public string JobGroupCode { get; set; }
        public string JobGroupRoot { get; set; }
        public int SexId { get; set; }
        public string NID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string IDNo { get; set; }
        public string Sex { get; set; }
        public string Customer { get; set; }
        public string ScheduleName { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> CoursePeopleStatusId { get; set; }
        public Nullable<System.DateTime> DateStatus { get; set; }
        public Nullable<System.DateTime> DateIssue { get; set; }
        public Nullable<System.DateTime> DateExpire { get; set; }
        public string StatusRemark { get; set; }
        public string CoursePeopleStatus { get; set; }
        public string CertificateNo { get; set; }
        public Nullable<bool> IsNotificationEnabled { get; set; }
    }
}
