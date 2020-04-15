using System;
using System.Text;

namespace PcmsTask.Models
{

    public partial class PforceRXFile
    {
        public int ID { get; set; }

        public string IND_ID { get; set; }

        public string CONSENT_EMAIL { get; set; }

        public string CONSENT_MOBILE { get; set; }

        public bool CONSENT_STATUS { get; set; }

        public DateTime CONSENT_DATE { get; set; }

        public string CONSENT_SOURCE { get; set; }

        public DateTime EXTRACT_DATE { get; set; }

        public string COUNTRY_CD { get; set; }

        public DateTime EXPIRATION_DATE { get; set; }

        public bool MODIFIED { get; set; }

        public DateTime createdate { get; set; }

        public int CollectionId { get; set; }

        public int? privacyId { get; set; }

        public virtual Collection Collection { get; set; }

        public virtual Privacy Privacy { get; set; }

        public PforceRXFile(Collection collection, String[] arr)
        {
            this.Collection = collection;
            this.CollectionId = collection.ID;

            this.IND_ID = arr[0];
            this.CONSENT_EMAIL = arr[1];
            this.CONSENT_MOBILE = arr[2];
            this.CONSENT_STATUS = (string.IsNullOrEmpty(arr[3]) ? false : (arr[3].ToLower() == "opt in" || arr[3].ToLower() == "opt-in"));
            this.CONSENT_DATE = Convert.ToDateTime(arr[4]);
            this.CONSENT_SOURCE = string.IsNullOrEmpty(arr[5]) ? @"PforceRX" : arr[5];
            this.EXTRACT_DATE = Convert.ToDateTime(arr[6]);
            this.COUNTRY_CD = arr[7];
            if (arr[8].Length > 0)
                this.EXPIRATION_DATE = Convert.ToDateTime(arr[8]);

            this.MODIFIED = false;
        }

        public PforceRXFile(Privacy p)
        {
            this.IND_ID = p.NucleusKey;
            this.CONSENT_EMAIL = p.EMAIL;
            this.CONSENT_MOBILE = p.MOBILE;
            this.CONSENT_STATUS = p.CONSENT_USE;
            //added unsubscribe condition as part of APACCR-287 by venkat
            if ( p.Unsubscribe == true || p.status == Commons.Status.PrivacyStatus.DELETED || p.status == Commons.Status.PrivacyStatus.ERASED) this.CONSENT_STATUS = false;
            this.CONSENT_SOURCE = string.IsNullOrEmpty(p.CONSENT_SOURCE) ? @"서면동의서" : p.CONSENT_SOURCE;
            this.CONSENT_DATE = p.CONSENTDATE;
            this.COUNTRY_CD = string.IsNullOrEmpty(p.COUNTRY_CD) ? @"KR" : p.COUNTRY_CD;
            this.EXTRACT_DATE = DateTime.UtcNow;

        }

        public static string exportHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IND_ID");
            sb.Append("|");
            sb.Append("CONSENT_EMAIL");
            sb.Append("|");
            sb.Append("CONSENT_MOBILE");
            sb.Append("|");
            sb.Append("CONSENT_STATUS");
            sb.Append("|");
            sb.Append("CONSENT_DATE");
            sb.Append("|");
            sb.Append("CONSENT_SOURCE");
            sb.Append("|");
            sb.Append("EXTRACT_DATE");
            sb.Append("|");
            sb.Append("COUNTRY_CD");
            return sb.ToString();
        }

        public string Export()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.IND_ID);
            sb.Append("|");
            sb.Append(this.CONSENT_EMAIL);
            sb.Append("|");
            sb.Append(this.CONSENT_MOBILE.Replace(" ",String.Empty));
            sb.Append("|");
            sb.Append(this.CONSENT_STATUS ? "Opt In" : "Opt Out");
            sb.Append("|");
            sb.Append(this.CONSENT_DATE.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("|");
            sb.Append(this.CONSENT_SOURCE);
            sb.Append("|");
            sb.Append(this.EXTRACT_DATE.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("|");
            sb.Append(this.COUNTRY_CD);
            return sb.ToString();
        }
    }
}