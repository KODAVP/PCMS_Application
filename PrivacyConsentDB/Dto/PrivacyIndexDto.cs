using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class PrivacyIndexDto
    {
        public IEnumerable<Privacy> Privacies;
        public PrivacySearch Search;
        public IEnumerable<Channel> Channels;
        
        public PrivacyIndexDto() {
        
        }
    }
    public class UnsubscribeDto
    {
        public string stat { get; set; }
        public List<string> contacts { get; set; }

        public bool unsubscribe
        {
            get
            {
                if (string.IsNullOrEmpty(this.stat)) return false;
                if (this.stat.ToUpper().Equals("Y")) return true;
                return false;
            }
        }
    }
}