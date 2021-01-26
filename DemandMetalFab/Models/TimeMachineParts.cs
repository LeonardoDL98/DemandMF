using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemandMetalFab.Models
{
    public class TimeMachineParts
    {
        public TimeMachineParts()
        {
            this.partes = new MF_Part();

        }
        public MF_Part partes { set; get; }
        public string opmaquina { set; get; }
    }
}