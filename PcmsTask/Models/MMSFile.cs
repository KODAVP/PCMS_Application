using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcmsTask.Models
{
    public class MMSFile
    {
        public string COMPANY { get; set; }
        public string TEL { get; set; }
        public string ZIP { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string CATEGORY { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string HP { get; set; }
        public string RCV_MAIL { get; set; }
        public string AGR_VER { get; set; }
        public DateTime AGR_DATE { get; set; }
        public string SUB_CHANNEL { get; set; }
        public string AGREE1 { get; set; }
        public string AGREE2 { get; set; }
        public string AGREE3 { get; set; }

        public MMSFile(Collection collection, string[] arr)
        {
            this.COMPANY = arr[0];
            this.TEL = arr[1];
            this.ZIP = arr[2];
            this.ADDRESS1 = arr[3];
            this.ADDRESS2 = arr[4];
            this.CATEGORY = arr[5];
            this.NAME = arr[6];
            this.EMAIL = arr[7];
            this.HP = arr[8];
            this.RCV_MAIL = arr[9];
            this.AGR_VER = arr[10];
            this.AGR_DATE = Convert.ToDateTime(arr[11]);
            this.SUB_CHANNEL = arr[12];
            this.AGREE1 = arr[13];
            this.AGREE2 = arr[14];
            this.AGREE3 = arr[14];
        }
    }
}

