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
    
    public partial class ViewFDPIdea
    {
        public int Id { get; set; }
        public Nullable<int> CrewId { get; set; }
        public string InitFlts { get; set; }
        public string InitRoute { get; set; }
        public Nullable<System.DateTime> DutyStart { get; set; }
        public int DutyType { get; set; }
        public Nullable<System.DateTime> RestUntil { get; set; }
    }
}
