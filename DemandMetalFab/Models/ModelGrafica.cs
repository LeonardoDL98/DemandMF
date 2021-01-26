using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemandMetalFab.Models
{
    #region Chart
    public class DataChartModel
    {
        public List<string> labels { get; set; }
        public List<Object> datasets { get; set; }
        //public Options options { get; set; }

        public DataChartModel()
        {
            this.labels = new List<string>();
            this.datasets = new List<Object>();
        }
    }

    public class DataSetDetail
    {
        public string type { get; set; }
        public string label { get; set; }
        public List<string> data { get; set; }
        public string backgroundColor { get; set; }
        public DataSetDetail()
        {
            this.data = new List<string>();
        }

    }

    public class DataSets
    {
        public string type { get; set; }
        public string label { get; set; }
        public List<int> data { get; set; }
        public string borderColor { get; set; }
        public bool fill { get; set; }

        public DataSets()
        {
            this.data = new List<int>();
        }
    }

    public class Options
    {
        public Scales scales { get; set; }
    }

    public class Axes
    {
        public bool stacked { get; set; }
    }

    public class Scales
    {
        public List<Axes> xAxes { get; set; }
        public List<Axes> yAxes { get; set; }
        public Scales()
        {
            this.xAxes = new List<Axes>();
            this.yAxes = new List<Axes>();
        }
    }
    #endregion
   
}