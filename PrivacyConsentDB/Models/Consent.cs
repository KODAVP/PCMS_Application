using PrivacyConsentDB.Commons;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivacyConsentDB.Models
{

    public class Consent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("수집/이용 동의")]
        public bool CONSENT_USE { get; set; }

        [DisplayName("마케팅 활용 동의")]
        public bool CONSENT_MARKETING_AGREEMENT { get; set; }  //Added by Nagaraju as part of APC30683265i

        [DisplayName("위탁 동의")]
        public bool CONSENT_TRUST { get; set; }
        [DisplayName("국외이전 동의")]
        public bool CONSENT_ABROAD { get; set; }
        [DisplayName("서명")]
        public bool CONSENT_SIGN { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("동의일자")]
        public DateTime CONSENT_DATE { get; set; }

        [DisplayName("동의일자")]
        public DateTime CONSENT_DATE_KOREA
        {
            get
            {
                return CommonUtil.toKoreaDT(this.CONSENT_DATE);
            }
        }

        [DisplayName("동의서채널")]
        public string CONSENT_SOURCE { get; set; }
        public string CONSENT_TYPE { get; set; }
        [DisplayName("동의서버전")]
        public string CONSENT_VERSION { get; set; }

        public bool IS_CONSENTED
        {
            get
            {
                if (CONSENT_DATE != null && CONSENT_DATE >= DateTime.UtcNow)
                    return true;
                else
                    return false;
            }
        }
        public virtual Privacy privacy { get; set; }

    }
}