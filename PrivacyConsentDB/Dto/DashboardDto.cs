using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class DashboardDto
    {
        public int approvalrequests;
        public int expiredconsents;
        public int rejectedrequests;
        public int approvedrequests;

        public int mmscount;
        public int pforcerxcount;
        public int hardcount;
        public int codemappingcount;
        public int grvcount;

        public List<PrivacyLog> logs;

        public DashboardDto() {
            approvalrequests = 0;
            expiredconsents = 0;
            rejectedrequests = 0;
            mmscount = 0;
            pforcerxcount = 0;
            hardcount = 0;
            codemappingcount = 0;
            grvcount = 0;

        }
    }
}
