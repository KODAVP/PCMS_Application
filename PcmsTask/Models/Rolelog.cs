using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcmsTask.Models
{
   public class Rolelog
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public string IP { get; set; }

        public string Target_User_ID { get; set; }

        public string Activity { get; set; }
        public DateTime createdate { get; set; }

    }
}
