using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Models
{
    
    public class Approval
    {
        [Key, ForeignKey("privacy")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int privacyId { get; set; }

        public virtual Privacy privacy { get; set; }

        [DisplayName("승인상태")]
        public ApprovalStatus status { get; set; } // 승인상태

        [DisplayName("사유")]
        public string message { get; set; } // 사유

        [DisplayName("요청일자")]
        public DateTime createdate { get; set; } // 생성일자
        [DisplayName("요청자")]
        public string creater { get; set; } // 생성자
        [DisplayName("승인일자")]
        public DateTime modifieddate { get; set; } // 수정일자
        [DisplayName("승인자")]
        public string modifier { get; set; } // 수정자
        //[DisplayName("회사 코드")]
        //public string COMP_CODE { get; set; } // 회사 코드

    }
}