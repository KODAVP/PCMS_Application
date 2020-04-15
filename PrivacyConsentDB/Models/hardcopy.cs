using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class hardcopy
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }



        public string filename { get; set; }

        [DisplayName("생성일자")]
        public DateTime createdate { get; set; }
    }
}