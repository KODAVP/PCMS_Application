using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class Userlog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("사용자")]
        public string username { get; set; }

        [DisplayName("이메일")]
        public string useremail { get; set; }

        [DisplayName("IP")]
        public string ip { get; set; }

        [DisplayName("URL")]
        public string url { get; set; }
        
        [DisplayName("요청타입")]
        public string reqtype { get; set; }

        [DisplayName("요청인자")]
        public string parameters { get; set; }

        [DisplayName("생성일자")]
        public DateTime createdate { get; set; }

        [DisplayName("회사")]
        public string COMP_CODE { get; set; }

    }
}