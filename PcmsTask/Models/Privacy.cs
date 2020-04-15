
using PcmsTask.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using static PcmsTask.Commons.Status;

namespace PcmsTask.Models
{
    public partial class Privacy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Privacy()
        {
            Consents = new HashSet<Consent>();
            PforceRXFiles = new HashSet<PforceRXFile>();
            PrivacyLogs = new HashSet<PrivacyLog>();
        }
                
        public int ID { get; set; }

        public string IND_ID { get; set; }

        public string IND_EXT_ID { get; set; }

        public string OWNER { get; set; }

        public string ACT_STATUS { get; set; }

        public string WKP_NAME { get; set; }

        public string WKP_TEL { get; set; }

        public string ZIP { get; set; }

        public string PROVINCE { get; set; }

        public string CITY { get; set; }

        public string DONG { get; set; }

        public string STREET { get; set; }

        public string FULL_ADDR { get; set; }

        public string IND_SP { get; set; }

        public string TITLE { get; set; }

        public string IND_LASTNAME { get; set; }

        public string IND_FIRSTNAME { get; set; }

        public string IND_FULL_NAME { get; set; }

        public string EMAIL { get; set; }

        public string MOBILE { get; set; }

        public string CONSENT_SOURCE { get; set; }

        public string CONSENT_SUB_SOURCE { get; set; }

        public DateTime? EXTRACT_DATE { get; set; }

        public string COUNTRY_CD { get; set; }

        public DateTime createdate { get; set; }

        public string creater { get; set; }

        public DateTime modifieddate { get; set; }

        public PrivacyStatus status { get; set; }

        public string pcmsid { get; set; }

        public int? channelId { get; set; }

        public string SOURCE { get; set; }

        public string WKP_ID { get; set; }

        public string WKP_EXT_ID { get; set; }

        public string OneKey { get; set; }

        public string NucleusKey { get; set; }

        public string LINK_RESERVATION { get; set; }

        public string LINK_PHONE { get; set; }

        public bool LINK_ALERTED { get; set; }

        public SendChannel SENDCHANEL { get; set; }

        public virtual Approval Approval { get; set; }

        public virtual Channel Channel { get; set; }

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

        public bool CONSENT_USE
        {
            get
            {
                return this.CONSENT.CONSENT_USE;
            }
        }


        public bool CONSENT_TRUST
        {
            get
            {
                return this.CONSENT.CONSENT_TRUST;
            }
        }
        public bool CONSENT_ABROAD
        {
            get
            {
                return this.CONSENT.CONSENT_ABROAD;
            }
        }
        public bool CONSENT_SIGN
        {
            get
            {
                if (this.CONSENT == null) return false;
                return this.CONSENT.CONSENT_SIGN;
            }
        }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CONSENTDATE
        {
            get
            {
                return this.CONSENT.CONSENT_DATE;
            }
        }

        public bool IsExpired(int periodyear) {
            return false;
            //return DateTime.UtcNow >  this.CONSENTDATE.AddYears(periodyear);
        }

        public string CONSENTVERSION
        {
            get
            {
                return this.CONSENT.CONSENT_VERSION;
            }
        }

        public bool Unsubscribe { get; set; }

        /*
        ALTER TABLE dbo.pcms_d ADD 
	        IND_SP_SUB VARCHAR(256) NULL
	        , KeyOpinionLeader VARCHAR(1) NULL 
	        , DoctorStatus VARCHAR(1) NULL 
	        , HospitalType VARCHAR(50) NULL 
	        , HospitalBeds VARCHAR(30) NULL 
	        , WKP_FAX VARCHAR(50) NULL 
	        , HOMEPAGE VARCHAR(256) NULL 
	        , POSN_NAME VARCHAR(256) NULL 
	        , GEO_NAME VARCHAR(256) NULL 
	        , PARENT_GEO_NAME VARCHAR(256) NULL 
	        , OWNERNAME VARCHAR(256) NULL 
        ; 
        public string IND_SP_SUB { get;set;}
        public string KeyOpinionLeader { get; set; }
        public string DoctorStatus { get; set; }
        public string HospitalType { get; set; }
        public string HospitalBeds { get; set; }
        public string WKP_FAX { get; set; }
        public string HOMEPAGE { get; set; }
        public string POSN_NAME { get; set; }
        public string GEO_NAME { get; set; }
        public string PARENT_GEO_NAME { get; set; }
        public string OWNERNAME { get; set; }
             
        */

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consent> Consents { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PforceRXFile> PforceRXFiles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrivacyLog> PrivacyLogs { get; set; }

        public Privacy(string WKP_NAME, string WKP_TEL, string ZIP, string PROVINCE, string CITY, string DONG, string STREET, string FULL_ADDR, string IND_SP
            , string TITLE, string IND_FULL_NAME, string EMAIL, string MOBILE, string CONSENT_SOURCE, string CONSENT_SUB_SOURCE
            , string LINK_RESERVATION, string LINK_PHONE
            , DateTime CONSENT_DATE, string CONSENT_VERSION, bool CONSENT_USE_YN, bool CONSENT_TRUST_YN, bool CONSENT_ABROAD_YN, bool CONSENT_SIGN_YN, bool Unsubscribe, bool EmptyOwner = false)
        {
            this.pcmsid = IdGenerater.Generater();
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
            //this.CONSENT_SOURCE = CONSENT_SOURCE;
            this.CONSENT_SUB_SOURCE = CONSENT_SUB_SOURCE;
            this.LINK_RESERVATION = LINK_RESERVATION;
            this.LINK_PHONE = LINK_PHONE;
            this.Unsubscribe = Unsubscribe;

            this.Consents = new List<Consent>();
            this.Consents.Add(new Consent
            {
                CONSENT_DATE = CONSENT_DATE,
                CONSENT_VERSION = CONSENT_VERSION ,
                CONSENT_USE = CONSENT_USE_YN,
                CONSENT_TRUST = CONSENT_TRUST_YN,
                CONSENT_ABROAD = CONSENT_ABROAD_YN,
                CONSENT_SIGN = CONSENT_SIGN_YN,
                CONSENT_SOURCE = CONSENT_SOURCE,
            });
            this.COUNTRY_CD = "KR";

            var currentUsername = "BATCH";
            this.OWNER = currentUsername;
            if (EmptyOwner) this.OWNER = null;
            this.status = PrivacyStatus.IMPORTED;
        }
    }
}
