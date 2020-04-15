namespace PcmsTask.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Consent
    {
        public int ID { get; set; }

        public DateTime CONSENT_DATE { get; set; }

        public string CONSENT_SOURCE { get; set; }

        public string CONSENT_TYPE { get; set; }

        public string CONSENT_VERSION { get; set; }

        public int? privacy_ID { get; set; }

        public bool CONSENT_USE { get; set; }

        public bool CONSENT_TRUST { get; set; }

        public bool CONSENT_ABROAD { get; set; }

        public bool CONSENT_SIGN { get; set; }

        public virtual Privacy Privacy { get; set; }
    }
}
