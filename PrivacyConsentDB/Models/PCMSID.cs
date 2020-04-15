using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class PcmsId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string KEY { get; set; }
        public DateTime createdate { get; set; } // 생성일자
    }
}