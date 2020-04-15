using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class ReportDto
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

        public List<ReportSP> resultTotal;
        public List<ReportSP> resultEmail;
        public List<ReportSP> resultMobile;

        public List<ReportSP> rsTotal;
        public List<ReportSP> rsGRV;
        public List<ReportSP> rsMMS;
        public List<ReportSP> rsDOC;
        public List<ReportSP> rsPFRX;

        public List<ReportSP> rsRejected;
        public List<ReportSP> rsApproved;
        public List<ReportSP> rsN360;
        public List<ReportSP> rsOnekey;

        public int pcms_count { get; set; }
        public int n360_count { get; set; }
        public int n360_distinct_count { get; set; }
        public int onekey_count { get; set; }
        public int onekey_distinct_count { get; set; }
        public int active_count { get; set; }
        public int inactive_count { get; set; }

        public ReportDto()
        {
            consentbegindt = null;
            collectbegindt = DateTime.Now.AddMonths(-1);
            distinct = true;
        }
    }
}