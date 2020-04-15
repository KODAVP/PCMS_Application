namespace PcmsTask.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PcmsId
    {
        public int ID { get; set; }

        public string KEY { get; set; }

        public DateTime createdate { get; set; }
    }
}
