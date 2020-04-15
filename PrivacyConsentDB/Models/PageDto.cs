using PrivacyConsentDB.Commons;
using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class PageDto
    {
        public int startIndex { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
        public List<Batch> list { get; set; }

        public string channels { get; set; }
        public string bound { get; set; }
        public string status { get; set; }

        public DateTime? executedt { get; set; }

        public PageDto()
        {
            startIndex = 0;
            pageSize = 20;
        }
    }
}