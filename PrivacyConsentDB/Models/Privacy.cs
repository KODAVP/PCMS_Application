using PrivacyConsentDB.Commons;
using PrivacyConsentDB.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Models
{
    public class Privacy
    {
        public string SOURCE { get; set; }
        public string WKP_ID { get; set; }
        public string WKP_EXT_ID { get; set; }

        [DisplayName("OneKey")]
        public string OneKey { get; set; }
        [DisplayName("Nucleus")]
        public string NucleusKey { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("고유ID")]
        public string PCMSID { get; set; }
        [DisplayName("IND_ID")]
        public string IND_ID { get; set; }
        [DisplayName("고유번호")]
        public string IND_EXT_ID { get; set; } // CIM 의사코드

        [DisplayName("담당자")]
        public string OWNER { get; set; }
        [DisplayName("사용여부")]
        public string ACT_STATUS { get; set; }
        [DisplayName("근무처(병원명)")]
        public string WKP_NAME { get; set; }
        [DisplayName("근무지연락처")]
        public string WKP_TEL { get; set; }

        [DisplayName("우편번호")]
        public string ZIP { get; set; }
        [DisplayName("지역")]
        public string PROVINCE { get; set; }
        [DisplayName("주소(시/도)")]
        public string CITY { get; set; }
        [DisplayName("주소(군/구/동)")]
        public string DONG { get; set; }
        [DisplayName("상세주소")]
        public string STREET { get; set; }
        [DisplayName("주소")]
        public string FULL_ADDR { get; set; }
        [DisplayName("진료과")]
        public string IND_SP { get; set; }
        [DisplayName("직위")]
        public string TITLE { get; set; }
        [DisplayName("고객명(이름)")]
        public string IND_LASTNAME { get; set; }
        [DisplayName("고객명(성)")]
        public string IND_FIRSTNAME { get; set; }
        [DisplayName("고객명")]
        public string IND_FULL_NAME { get; set; }

        [DisplayName("이메일")]
        public string EMAIL { get; set; }
        [DisplayName("핸드폰")]
        public string MOBILE { get; set; }
        public Status.SendChannel SENDCHANEL { get; set; } // 1 : N360 , 2: PforceRX , 4: MMS
        [DisplayName("동의서1")]
        public virtual ICollection<Consent> Consents { get; set; }

        public virtual ICollection<PrivacyLog> PrivacyLogs { get; set; }

        [DisplayName("최종동의서")]
        public Consent CONSENT
        {
            get
            {
                Consent result = null;
                DateTime dt = DateTime.MinValue;
                foreach (var c in Consents)
                {
                    if (c.CONSENT_DATE != null)
                    {
                        if (dt == DateTime.MinValue)
                        {
                            result = c;
                        }
                        else
                        {
                            if (dt < c.CONSENT_DATE)
                            {
                                dt = c.CONSENT_DATE;
                                result = c;
                            }
                        }
                    }
                }
                return result;
            }
        }
        [DisplayName("동의서채널")]
        public string CONSENT_CHANNEL
        {
            get
            {
                return this.CONSENT.CONSENT_SOURCE;
            }
        }
        [DisplayName("수집/이용 동의")]
        public bool CONSENT_USE
        {
            get
            {
                return this.CONSENT.CONSENT_USE;
            }
        }
        [DisplayName("마케팅 활용 동의")]
        public bool CONSENT_MARKETING_AGREEMENT   //Added by Nagaraju as part of APC30683265i
        {
            get
            {
                return this.CONSENT.CONSENT_MARKETING_AGREEMENT;
            }
        }
        [DisplayName("위탁 동의")]
        public bool CONSENT_TRUST
        {
            get
            {
                return this.CONSENT.CONSENT_TRUST;
            }
        }
        [DisplayName("국외이전 동의")]
        public bool CONSENT_ABROAD
        {
            get
            {
                return this.CONSENT.CONSENT_ABROAD;
            }
        }
        [DisplayName("서명")]
        public bool CONSENT_SIGN
        {
            get
            {
                if (this.CONSENT == null) return false;
                return this.CONSENT.CONSENT_SIGN;
            }
        }
        [DisplayName("동의일자")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CONSENTDATE
        {
            get
            {
                return this.CONSENT.CONSENT_DATE_KOREA;
            }
        }
        [DisplayName("동의서버전")]
        public string CONSENTVERSION
        {
            get
            {
                return this.CONSENT.CONSENT_VERSION;
            }
        }
        [DisplayName("수신거부")]
        public bool Unsubscribe { get; set; }

        [DisplayName("채널(주)")]
        public string CONSENT_SOURCE { get; set; }

        [DisplayName("채널(부)")]
        public string CONSENT_SUB_SOURCE { get; set; }

        [DisplayName("화이자링크예약")]
        public string LINK_RESERVATION { get; set; }
        [DisplayName("화이자링크연락처")]
        public string LINK_PHONE { get; set; }
        public bool LINK_ALERTED { get; set; }
        public string LINK_RESERVATION_STR
        {
            get
            {
                if (string.IsNullOrEmpty(this.LINK_RESERVATION)) return string.Empty;
                string result = string.Empty;
                string[] arr = LINK_RESERVATION.Split(',');
                foreach (string s in arr)
                {
                    string temp = s;
                    temp = temp.Replace("m", "월");
                    temp = temp.Replace("t", "화");
                    temp = temp.Replace("w", "수");
                    temp = temp.Replace("h", "목");
                    temp = temp.Replace("f", "금");
                    temp += ":00";
                    if (string.IsNullOrEmpty(result))
                    {
                        result = temp;
                    }
                    else
                    {
                        result += " ";
                        result += temp;
                    }
                }
                return result;
            }
        }
        public DateTime? EXTRACT_DATE { get; set; }
        public string COUNTRY_CD { get; set; }

        [DisplayName("생성일자")]
        public DateTime createdate { get; set; }
        [DisplayName("생성자")]
        public string creater { get; set; }
        [DisplayName("수정일자")]
        public DateTime modifieddate { get; set; }

        public PrivacyStatus status { get; set; }

        public int? channelId { get; set; }
        public virtual Channel channel { get; set; }
        public virtual Approval approval { get; set; }
        //public string COMP_CODE { get; set; }

        public Privacy()
        {
            this.Consents = new List<Consent>();
            this.status = PrivacyStatus.REGISTED;
            this.LINK_ALERTED = false;
        }

        public Privacy(string WKP_NAME, string WKP_TEL, string ZIP, string PROVINCE, string CITY, string DONG, string STREET, string FULL_ADDR, string IND_SP
            , string TITLE, string IND_FULL_NAME, string EMAIL, string MOBILE, string CONSENT_SOURCE, string CONSENT_SUB_SOURCE
            , string LINK_RESERVATION, string LINK_PHONE
            , DateTime CONSENT_DATE, string CONSENT_VERSION, bool CONSENT_USE_YN, bool CONSENT_TRUST_YN, bool CONSENT_ABROAD_YN, bool CONSENT_SIGN_YN, bool CONSENT_MARKETING_AGREEMENT,bool Unsubscribe, bool EmptyOwner = false)
        {
            this.PCMSID = IdGenerater.Generater();
            this.WKP_NAME = WKP_NAME;
            this.WKP_TEL = WKP_TEL;
            this.ZIP = ZIP;
            this.PROVINCE = PROVINCE;
            this.CITY = CITY;
            this.DONG = DONG;
            this.STREET = STREET;
            this.FULL_ADDR = FULL_ADDR;
            this.IND_SP = IND_SP;
            this.TITLE = TITLE;
            this.IND_FULL_NAME = IND_FULL_NAME;
            this.EMAIL = EMAIL;
            this.MOBILE = MOBILE;
            this.CONSENT_SOURCE = CONSENT_SOURCE;
            this.CONSENT_SUB_SOURCE = CONSENT_SUB_SOURCE;
            this.LINK_RESERVATION = LINK_RESERVATION;
            this.LINK_PHONE = LINK_PHONE;
            this.Unsubscribe = Unsubscribe;

            this.Consents = new List<Consent>();
            this.Consents.Add(new Models.Consent
            {
                CONSENT_DATE = CONSENT_DATE,
                CONSENT_VERSION = CONSENT_VERSION,
                CONSENT_USE = CONSENT_USE_YN,
                CONSENT_TRUST = CONSENT_TRUST_YN,
                CONSENT_ABROAD = CONSENT_ABROAD_YN,
                CONSENT_SIGN = CONSENT_SIGN_YN ,
                CONSENT_SOURCE = CONSENT_SOURCE,
                CONSENT_MARKETING_AGREEMENT= CONSENT_MARKETING_AGREEMENT  //Added by Nagaraju as part of APC30683265i
            });
            this.COUNTRY_CD = "KR";

            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name) ? HttpContext.Current.User.Identity.Name : "Anonymous";
            currentUsername = currentUsername.Substring(currentUsername.IndexOf('\\') + 1).Replace("\\", "").ToUpper();
            this.OWNER = currentUsername;
            if (EmptyOwner) this.OWNER = null;
            this.status = PrivacyStatus.REGISTED;
            //this.COMP_CODE = COMP_CODE;
        }
    }
}