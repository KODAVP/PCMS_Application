using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class ApprovalIndexDto
    {
        public IEnumerable<Approval> Approvals;
        public ApprovalSearch Search;

        public ApprovalIndexDto() {
        }
    }
}