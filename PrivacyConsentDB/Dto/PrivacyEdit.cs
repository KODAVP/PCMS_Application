using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class PrivacyEdit
    {
        public int ID { get; set; }
        public string WKP_NAME { get; set; }
        public string WKP_TEL { get; set; }
        public string ZIP { get; set; }
        public string FULL_ADDR { get; set; }
        public string IND_SP { get; set; }
        public string TITLE { get; set; }
        public string IND_FULL_NAME { get; set; }

        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$")]
        public string EMAIL { get; set; }
        [RegularExpression(@"^[0-9]{2,3}-[0-9]{3,4}-[0-9]{4}$")]
        public string MOBILE { get; set; }
        public string LINK_RESERVATION { get; set; }
        public string LINK_PHONE { get; set; }
        public string Unsubscribe { get; set; }
        public Consent CONSENT { get; set; }
    }
}