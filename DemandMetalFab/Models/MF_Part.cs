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
    
    public partial class MF_Part
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MF_Part()
        {
            this.MF_OpMachine = new HashSet<MF_OpMachine>();
        }
    
        public int Id_Part { get; set; }
        public Nullable<int> Item { get; set; }
        public string Num_Part { get; set; }
        public int Id_Project { get; set; }
        public int Id_Machine { get; set; }
        public Nullable<decimal> Set_Up { get; set; }
        public Nullable<decimal> Cycle { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> Id_Proceso { get; set; }
    
        public virtual MF_Machine MF_Machine { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MF_OpMachine> MF_OpMachine { get; set; }
        public virtual MF_Project MF_Project { get; set; }
        public virtual MF_Proceso MF_Proceso { get; set; }
    }
}
