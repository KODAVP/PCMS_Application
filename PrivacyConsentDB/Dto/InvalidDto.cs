using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using static PrivacyConsentDB.Commons.MyRoleManager;

namespace PrivacyConsentDB.Dto
{
    public class InvalidDto
    {        
        [DisplayName("이름")]
        public string searchname { get; set; }

        public int startIndex { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
        public IEnumerable<Privacy> list { get; set; }

        public InvalidDto() {
            startIndex = 0;
            pageSize = 20;
        }
    }
    
}