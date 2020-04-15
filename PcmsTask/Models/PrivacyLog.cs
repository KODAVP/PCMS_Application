namespace PcmsTask.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PrivacyLog
    {
        public int ID { get; set; }

        public string changes { get; set; }

        public DateTime createdate { get; set; }

        public string creater { get; set; }

        public int? privacy_ID { get; set; }

        public virtual Privacy Privacy { get; set; }
    }
}
