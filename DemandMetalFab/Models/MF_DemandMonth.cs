//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemandMetalFab.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MF_DemandMonth
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MF_DemandMonth()
        {
            this.MF_WorkDate = new HashSet<MF_WorkDate>();
        }
    
        public int Id_DemandMonth { get; set; }
        public string DemandMonth { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MF_WorkDate> MF_WorkDate { get; set; }
    }
}