using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class SearchDto
    {
        public int id { get; set; }
        public string searchkey { get; set; }
        public int count { get; set; }
        public bool result { get; set; }
        public List<PrivacyDto> lists { get; set; }

        public SearchDto() {
            id = 0;
        }
    }
}