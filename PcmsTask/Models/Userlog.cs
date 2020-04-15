using System;

namespace PcmsTask.Models
{
    public class Userlog
    {
        public int ID { get; set; }
        public string username { get; set; }

        public string useremail { get; set; }

        public string ip { get; set; }

        public string url { get; set; }
        
        public string reqtype { get; set; }

        public string parameters { get; set; }

        public DateTime createdate { get; set; }
        
    }
}