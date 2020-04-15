using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PrivacyConsentDB.Commons;

namespace PrivacyConsentDB.Models
{
    
    public class Batch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("채널명")]
        public string name { get; set; }
        [DisplayName("방식")]
        public BoundType bound { get; set; }
        public BatchStatus status { get; set; }
        public string message { get; set; }

        public DateTime createdate { get; set; }
        public string creater { get; set; }
    }

}