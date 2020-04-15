using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class ReportSearch
    {
        [DisplayName("등록일자 기준 조회")]
        public DateTime? consentbegindt { get; set; }
        [DisplayName("등록일자 기준 조회")]
        public DateTime? consentenddt { get; set; }
        [DisplayName("승인일자 기준 조회")]
        public DateTime? collectbegindt { get; set; }
        [DisplayName("승인일자 기준 조회")]
        public DateTime? collectenddt { get; set; }

        [DisplayName("Nucleus 중복여부")]
        public bool distinct { get; set; }
    }
}