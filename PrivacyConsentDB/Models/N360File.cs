using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Models
{
    public class N360File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public string PCMSID { get; set; }
        public string OneKey { get; set; }
        public bool modified { get; set; }  // privacy 반영여부
        public DateTime createdate { get; set; } // 생성일자
        public int collectionId { get; set; }
        public virtual Collection collection { get; set; }

        public int PrivacyId { get; set; }

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
        public N360File(Collection collection, string[] arr) {
            this.collection = collection;
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
            //this.PCMSID = arr[29];
            //this.OneKey = arr[30];
            this.modified = false;
        }

        public N360File(Privacy p) {
            this.SOURCE = p.SOURCE;
            this.ACTION = "";
            this.WKP_ID = p.WKP_ID;
            this.WKP_EXT_ID = p.WKP_EXT_ID;
            this.IND_ID = p.IND_ID;
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
            this.PCMSID = p.PCMSID;
            this.OneKey = p.OneKey;
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
            sb.Append(this.CONSENT_STATUS);
            sb.Append("|");
            sb.Append(this.CONSENT_DATE.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("|");
            sb.Append(this.CONSENT_SOURCE);
            sb.Append("|");
            sb.Append(this.CONSENT_TYPE);
            sb.Append("|");
            sb.Append(this.CONSENT_VERSION);

            return sb.ToString();
        }
    }
}