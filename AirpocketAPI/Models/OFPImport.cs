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
    
    public partial class OFPImport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OFPImport()
        {
            this.OFPImportItems = new HashSet<OFPImportItem>();
            this.OFPImportProps = new HashSet<OFPImportProp>();
        }
    
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FlightNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public Nullable<System.DateTime> DateFlight { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public string Text { get; set; }
        public string User { get; set; }
        public string TextOutput { get; set; }
        public Nullable<int> FlightId { get; set; }
        public string DateUpdate { get; set; }
        public Nullable<System.DateTime> DateConfirmed { get; set; }
        public string UserConfirmed { get; set; }
        public Nullable<int> PICId { get; set; }
        public Nullable<System.DateTime> JLDatePICApproved { get; set; }
        public string JLSignedBy { get; set; }
        public string PIC { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFPImportItem> OFPImportItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFPImportProp> OFPImportProps { get; set; }
    }
}
