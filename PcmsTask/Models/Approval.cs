namespace PcmsTask.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Approval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int privacyId { get; set; }

        public int status { get; set; }

        public DateTime createdate { get; set; }

        public string creater { get; set; }

        public DateTime modifieddate { get; set; }

        public string modifier { get; set; }

        public string message { get; set; }

        public virtual Privacy Privacy { get; set; }
    }
}
