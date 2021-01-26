using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemandMetalFab.Models
{
    public class PartNumber
    {
        public PartNumber()
        {
            this.num_part = new List<string>();
            this.id_project = new List<int>();
            this.id_machine = new List<int>();
        }
        public List<String> num_part { get; set; }
        public List<int> id_machine { get; set; }
        public List<int> id_project { get; set; }
    }
}