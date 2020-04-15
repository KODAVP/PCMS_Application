using PcmsTask.Commons;
using System;

namespace PcmsTask.Models
{
    public partial class Batch
    {
        public int ID { get; set; }

        public string name { get; set; }

        public BatchStatus status { get; set; }

        public string message { get; set; }

        public DateTime createdate { get; set; }

        public string creater { get; set; }

        public BoundType bound { get; set; }
    }
}
