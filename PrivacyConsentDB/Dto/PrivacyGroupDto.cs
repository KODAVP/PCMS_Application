using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class PrivacyGroupDto
    {
        [DisplayName("OneKey")]
        public string OneKey { get; set; }

        [DisplayName("갯수")]
        public int Count { get; set; }

        [DisplayName("이름")]
        public string Name { get; set; }

        public IEnumerable<Privacy> Privacies { get; set; }
    }
}