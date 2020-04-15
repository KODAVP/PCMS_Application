using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class Rolelog
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("UserID")]
        public string UserID { get; set; }

        [DisplayName("IP")]
        public string IP { get; set; }

        [DisplayName("Target_User_ID")]
        public string Target_User_ID { get; set; }

        [DisplayName("Activity")]
        public string Activity { get; set; }

        [DisplayName("생성일자")]
        public DateTime createdate { get; set; }
        [DisplayName("회사")]
        public string COMP_CODE { get; set; }
    }
}