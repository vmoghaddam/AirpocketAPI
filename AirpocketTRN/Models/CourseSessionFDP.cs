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
    
    public partial class CourseSessionFDP
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int FDPId { get; set; }
        public string SessionKey { get; set; }
        public int CourseId { get; set; }
    
        public virtual Course Course { get; set; }
        public virtual FDP FDP { get; set; }
    }
}
