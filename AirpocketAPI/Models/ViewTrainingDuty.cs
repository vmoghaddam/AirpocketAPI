//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirpocketAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewTrainingDuty
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> DateLocal { get; set; }
        public Nullable<int> CrewId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string ClassId { get; set; }
        public string JobGroup { get; set; }
        public string JobGroupCode { get; set; }
        public string ScheduleName { get; set; }
        public int IsCockpit { get; set; }
        public int IsCabin { get; set; }
        public Nullable<System.DateTime> DutyStart { get; set; }
        public Nullable<System.DateTime> DutyEnd { get; set; }
        public Nullable<System.DateTime> DutyStartLocal { get; set; }
        public Nullable<System.DateTime> DutyEndLocal { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> LocalDate { get; set; }
        public string PDate { get; set; }
        public string PYearName { get; set; }
        public Nullable<int> PYear { get; set; }
        public string PMonthName { get; set; }
        public Nullable<int> PMonth { get; set; }
        public string MonthName { get; set; }
        public string Month { get; set; }
        public string DayName { get; set; }
        public string YearName { get; set; }
    }
}
