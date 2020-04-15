using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Models
{
    public class PforceRXFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string IND_ID { get; set; }
        public string CONSENT_EMAIL { get; set; }
        public string CONSENT_MOBILE { get; set; }
        public string CONSENT_STATUS { get; set; }
        public DateTime CONSENT_DATE { get; set; }
        public string CONSENT_SOURCE { get; set; }
        public DateTime EXTRACT_DATE { get; set; }
        public string COUNTRY_CD { get; set; }

        public bool MODIFIED { get; set; }  // privacy 반영여부
        public DateTime createdate { get; set; } // 생성일자
        public int CollectionId { get; set; }
        public virtual Collection collection { get; set; }

        public int? privacyId { get; set; }
        public virtual Privacy privacy { get; set; }

        public ConsentStatus CONSENTSTATUS { get {
                if (this.CONSENT_STATUS.Equals("Opt Out"))
                    return ConsentStatus.OptOut;
                else
                    return ConsentStatus.OptIn;
            }
        }
        public PforceRXFile() {
            this.MODIFIED = false;
        }

        public PforceRXFile(Collection collection, string[] arr)
        {
            this.collection = collection;
            this.IND_ID = arr[0];
            this.CONSENT_EMAIL = arr[1];
            this.CONSENT_MOBILE = arr[2];
            this.CONSENT_STATUS = arr[3];
            this.CONSENT_DATE = Convert.ToDateTime(arr[4]);
            this.CONSENT_SOURCE = arr[5];
            this.EXTRACT_DATE = Convert.ToDateTime(arr[6]);
            this.COUNTRY_CD = arr[7];
            this.MODIFIED = false;
        }
        public string Export() {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.IND_ID);
            sb.Append(this.CONSENT_EMAIL);
            sb.Append(this.CONSENT_MOBILE);
            sb.Append(this.CONSENT_STATUS);
            sb.Append(this.CONSENT_DATE.ToString("yyyy-MM-DD HH:mm:ss"));
            sb.Append(this.CONSENT_SOURCE);
            sb.Append(this.EXTRACT_DATE.ToString("yyyy-MM-DD HH:mm:ss"));
            sb.Append(this.COUNTRY_CD);

            return sb.ToString();
        }
    }
}