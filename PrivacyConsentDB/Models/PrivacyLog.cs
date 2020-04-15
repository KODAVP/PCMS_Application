using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class PrivacyLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public virtual Privacy privacy { get; set; }

        [DisplayName("변경사항")]
        public string changes { get; set; }

        [DisplayName("변경일자")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime createdate { get; set; }
        [DisplayName("변경자")]
        public string creater { get; set; }
    }
}