using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class ApprovalSearch
    {
        [DisplayName("담당자")]
        public string owner { get; set; } // 담당자
        [DisplayName("고객명")]
        public string name { get; set; } // 고객명

        [DisplayName("진료과")]
        public string sp { get; set; } // 진료과
        [DisplayName("근무처")]
        public string hospital { get; set; } // 근무처

        [DisplayName("승인상태")]
        public string status { get; set; } // 승인상태

        [DisplayName("요청일자")]
        public DateTime? requestbegindt { get; set; }
        [DisplayName("요청일자")]
        public DateTime? requestenddt { get; set; }

        [DisplayName("승인일자")]
        public DateTime? approvalbegindt { get; set; }
        [DisplayName("승인일자")]
        public DateTime? approvalenddt { get; set; }

        public ApprovalSearch()
        {
            status = "total";
        }
    }
}