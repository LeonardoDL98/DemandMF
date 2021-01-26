using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.Description;
using System.Web;

namespace DemandMetalFab.Models
{
    public class ExtendModel
    {
    }


    #region MF_Part
    [MetadataType(typeof(MetadataMF_Part))]
    public partial class MF_Part
    { }
    public class MetadataMF_Part
    {
        [Display(Name = "Num", ResourceType = typeof(Recursos))]
        public int Id_Part { get; set; }
        public string Num_Part { get; set; }
        [Display(Name = "Project", ResourceType = typeof(Recursos))]
        public Nullable<int> Id_Project { get; set; }
        [Display(Name = "Machine", ResourceType = typeof(Recursos))]
        public Nullable<int> Id_Machine { get; set; }
    }
    #endregion

    #region MF_Project
    [MetadataType(typeof(MetadataMF_Project))]
        public partial class MF_Project
    { }
        public class MetadataMF_Project
    {
            [Display(Name = "Id", ResourceType = typeof(Recursos))]
            public int Id_Project { get; set; }
            [Display(Name = "Project", ResourceType = typeof(Recursos))]
            public string Project { get; set; }
            [Display(Name = "Num", ResourceType = typeof(Recursos))]
            public Nullable<int> Id_Demand { get; set; }
        }
    #endregion

    #region MF_Demand
    [MetadataType(typeof(MetadataMF_Demand))]
        public partial class MF_Demand
    { }
        public class MetadataMF_Demand
    {
            public int Id_Demand { get; set; }
            [Display(Name = "Demand", ResourceType = typeof(Recursos))]
            public string Demand { get; set; }
            public int Id_Customer { get; set; }
            public Nullable<int> Id_Sector { get; set; }
        }
    #endregion

    #region MF_Customer
    [MetadataType(typeof(MetadataMF_Customer))]
    public partial class MF_Customer
    { }
    public class MetadataMF_Customer
    {
        public int Id_Customer { get; set; }
        [Display(Name = "Client", ResourceType = typeof(Recursos))]
        public string Customer { get; set; }
    }
    #endregion
    #region MF_Sector
    [MetadataType(typeof(MetadataMF_Sector))]
    public partial class MF_Sector
    { }
    public class MetadataMF_Sector
    {
        public int Id_Sector { get; set; }
        [Display(Name = "Sector", ResourceType = typeof(Recursos))]
        public string Sector { get; set; }
    }
    #endregion
}