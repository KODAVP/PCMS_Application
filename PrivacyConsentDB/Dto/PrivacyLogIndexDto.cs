using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PrivacyConsentDB.Dto
{
    public class PrivacyLogIndexDto
    {
        public IEnumerable<PrivacyLog> Logs;
        public PrivacyLogSearch Search;

        [DisplayName("수정자")]
        public string modifier { get; set; } // 고객명
        [DisplayName("수정일자")]
        public DateTime? chngbegindt { get; set; }
        [DisplayName("수정일자")]
        public DateTime? chngenddt { get; set; }
        [DisplayName("PCMS Code")]
        public string pcmsid { get; set; }

        public int startIndex { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }

        public PrivacyLogIndexDto() {
            this.Search = new PrivacyLogSearch();
            Logs = null;
            startIndex = 0;
            pageSize = 20;
        }
    }
}