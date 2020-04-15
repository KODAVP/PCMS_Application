using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class PrivacyLogSearch
    {
        [DisplayName("수정자")]
        public string modifier { get; set; } // 고객명
        [DisplayName("수정일자")]
        public DateTime? chngbegindt { get; set; }
        [DisplayName("수정일자")]
        public DateTime? chngenddt { get; set; }

        [DisplayName("PCMS Code")]
        public string pcmsid { get; set; }

        public PrivacyLogSearch() {
        }
    }
}