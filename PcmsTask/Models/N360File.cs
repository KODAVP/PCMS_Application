using System;
using System.Text;
using static PcmsTask.Commons.Status;

namespace PcmsTask.Models
{
    public partial class N360File
    {
        public int ID { get; set; }

        public string SOURCE { get; set; }

        public string ACTION { get; set; }

        public string WKP_ID { get; set; }

        public string WKP_EXT_ID { get; set; }

        public string IND_ID { get; set; }

        public string IND_EXT_ID { get; set; }

        public string ACT_STATUS { get; set; }

        public string WKP_NAME { get; set; }

        public string WKP_TEL { get; set; }

        public string WKP_FAX { get; set; }

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

        public string GENDER { get; set; }

        public string EMAIL { get; set; }

        public string MOBILE { get; set; }

        public string CONSENT_STATUS { get; set; }

        public DateTime CONSENT_DATE { get; set; }

        public string CONSENT_SOURCE { get; set; }

        public string CONSENT_TYPE { get; set; }

        public string CONSENT_VERSION { get; set; }

        public bool modified { get; set; }

        public DateTime createdate { get; set; }

        public int collectionId { get; set; }

        public int PrivacyId { get; set; }

        public string PCMSID { get; set; }

        public string OneKey { get; set; }

        public virtual Collection Collection { get; set; }

        public ConsentStatus CONSENTSTATUS
        {
            get
            {
                if (this.CONSENT_STATUS.Equals("Opt-Out"))
                    return ConsentStatus.OptOut;
                else
                    return ConsentStatus.OptIn;
            }
        }
        
        public N360File() { }
        public N360File(Collection collection, string[] arr)
        {
            this.Collection = collection;
            this.SOURCE = arr[0];
            this.ACTION = arr[1];
            this.WKP_ID = arr[2];
            this.WKP_EXT_ID = arr[3];
            this.IND_ID = arr[4];
            this.IND_EXT_ID = arr[5];
            this.ACT_STATUS = arr[6];
            this.WKP_NAME = arr[7];
            this.WKP_TEL = arr[8];
            this.WKP_FAX = arr[9];
            this.ZIP = arr[10];
            this.PROVINCE = arr[11];
            this.CITY = arr[12];
            this.DONG = arr[13];
            this.STREET = arr[14];
            this.FULL_ADDR = arr[15];
            this.IND_SP = arr[16];
            this.TITLE = arr[17];
            this.IND_LASTNAME = arr[18];
            this.IND_FIRSTNAME = arr[19];
            this.IND_FULL_NAME = arr[20];
            this.GENDER = arr[21];
            this.EMAIL = arr[22];
            this.MOBILE = arr[23];
            this.CONSENT_STATUS = arr[24];
            this.CONSENT_DATE = Convert.ToDateTime(arr[25]);
            this.CONSENT_SOURCE = arr[26];
            this.CONSENT_TYPE = arr[27];
            this.CONSENT_VERSION = arr[28];
            this.PCMSID = this.IND_ID;
            this.modified = false;
        }

        public N360File(Privacy p, int periodyear)
        {
            this.SOURCE = string.IsNullOrEmpty(p.SOURCE)?"PCMS":p.SOURCE;
            this.ACTION = string.IsNullOrEmpty(p.NucleusKey) ? "I" : "U";
            if (p.status == PrivacyStatus.ERASED) this.ACTION = "U";
            if (p.status == PrivacyStatus.DELETED) this.ACTION = "D";

            this.WKP_ID = p.WKP_ID;
            this.WKP_EXT_ID = p.WKP_EXT_ID;
            this.IND_ID = p.pcmsid;
            this.IND_EXT_ID = p.IND_EXT_ID;
            this.ACT_STATUS = p.ACT_STATUS;
            this.WKP_NAME = p.WKP_NAME;
            this.WKP_TEL = p.WKP_TEL;
            this.ZIP = p.ZIP;
            this.PROVINCE = p.PROVINCE;
            this.CITY = p.CITY;
            this.DONG = p.DONG;
            this.STREET = p.STREET;
            this.FULL_ADDR = p.FULL_ADDR;
            this.IND_SP = p.IND_SP;
            this.TITLE = p.TITLE;
            this.IND_FULL_NAME = p.IND_FULL_NAME;
            this.EMAIL = p.EMAIL;
            this.MOBILE = p.MOBILE;
            this.CONSENT_DATE = p.CONSENTDATE;
            this.CONSENT_SOURCE = p.CONSENT_SOURCE;
            this.CONSENT_VERSION = p.CONSENTVERSION;
            this.PCMSID = p.pcmsid;

            this.CONSENT_STATUS = "Opt-In";
            if (p.status == PrivacyStatus.DELETED || p.status == PrivacyStatus.ERASED)
            {
                this.CONSENT_STATUS = "Opt-Out";
            }
            else if (p.IsExpired(periodyear))
            {
                this.CONSENT_STATUS = "Opt-Out";
            }
        }
        public static string exportHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SOURCE");
            sb.Append("|");
            sb.Append("ACTION");
            sb.Append("|");
            sb.Append("WKP_ID");
            sb.Append("|");
            sb.Append("WKP_EXT_ID");
            sb.Append("|");
            sb.Append("IND_ID");
            sb.Append("|");
            sb.Append("IND_EXT_ID");
            sb.Append("|");
            sb.Append("ACT_STATUS");
            sb.Append("|");
            sb.Append("WKP_NAME");
            sb.Append("|");
            sb.Append("WKP_TEL");
            sb.Append("|");
            sb.Append("WKP_FAX");
            sb.Append("|");
            sb.Append("ZIP");
            sb.Append("|");
            sb.Append("PROVINCE");
            sb.Append("|");
            sb.Append("CITY");
            sb.Append("|");
            sb.Append("DONG");
            sb.Append("|");
            sb.Append("STREET");
            sb.Append("|");
            sb.Append("FULL_ADDR");
            sb.Append("|");
            sb.Append("IND_SP");
            sb.Append("|");
            sb.Append("TITLE");
            sb.Append("|");
            sb.Append("IND_LASTNAME");
            sb.Append("|");
            sb.Append("IND_FIRSTNAME");
            sb.Append("|");
            sb.Append("IND_FULL_NAME");
            sb.Append("|");
            sb.Append("GENDER");
            sb.Append("|");
            sb.Append("EMAIL");
            sb.Append("|");
            sb.Append("MOBILE");
            sb.Append("|");
            sb.Append("CONSENT_STATUS");
            sb.Append("|");
            sb.Append("CONSENT_DATE");
            
            return sb.ToString();
        }
        public string Export()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.SOURCE);
            sb.Append("|");
            sb.Append(this.ACTION);
            sb.Append("|");
            sb.Append(this.WKP_ID);
            sb.Append("|");
            sb.Append(this.WKP_EXT_ID);
            sb.Append("|");
            sb.Append(this.IND_ID);
            sb.Append("|");
            sb.Append(this.IND_EXT_ID);
            sb.Append("|");
            sb.Append(this.ACT_STATUS);
            sb.Append("|");
            sb.Append(this.WKP_NAME);
            sb.Append("|");
            sb.Append(this.WKP_TEL);
            sb.Append("|");
            sb.Append(this.WKP_FAX);
            sb.Append("|");
            sb.Append(this.ZIP);
            sb.Append("|");
            sb.Append(this.PROVINCE);
            sb.Append("|");
            sb.Append(this.CITY);
            sb.Append("|");
            sb.Append(this.DONG);
            sb.Append("|");
            sb.Append(this.STREET);
            sb.Append("|");
            sb.Append(this.FULL_ADDR);
            sb.Append("|");
            sb.Append(this.IND_SP);
            sb.Append("|");
            sb.Append(this.TITLE);
            sb.Append("|");
            sb.Append(this.IND_LASTNAME);
            sb.Append("|");
            sb.Append(this.IND_FIRSTNAME);
            sb.Append("|");
            sb.Append(this.IND_FULL_NAME);
            sb.Append("|");
            sb.Append(this.GENDER);
            sb.Append("|");
            sb.Append(this.EMAIL);
            sb.Append("|");
            sb.Append(this.MOBILE);
            sb.Append("|");
            // µ¿ÀÇ 
            sb.Append(this.CONSENT_STATUS);
            sb.Append("|");
            sb.Append(this.CONSENT_DATE.ToString("yyyy-MM-dd"));
            

            return sb.ToString();
        }
    }
}
