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
    
    public partial class MF_Bitacora
    {
        public int Id_Bitacora { get; set; }
        public Nullable<int> Id_Usuario { get; set; }
        public string Descripcion { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
    
        public virtual MF_Usuario MF_Usuario { get; set; }
    }
}