using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class Company
    {
        [Key]
        public string COMP_CODE { get; set; }
        public string COMP_NAME { get; set; }
        public string DCE_TSA { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string CREATED_USER { get; set; }
        public string UPDATED_USER { get; set; }
    }
}