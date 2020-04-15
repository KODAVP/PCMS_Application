using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Dto
{
    public class PrivacyDto
    {
        public string IND_FULL_NAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public string NucleusKey { get; set; }
        public int ID { get; set; }
        public string PCMSID { get; set; }

        public PrivacyStatus status { get; set; }
    }
}