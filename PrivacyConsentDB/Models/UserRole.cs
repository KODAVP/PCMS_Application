using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static PrivacyConsentDB.Commons.MyRoleManager;

namespace PrivacyConsentDB.Models
{
    public class UserRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("사용자")]
        public string username { get; set; }
        [DisplayName("권한")]
        public RoleType roletype { get; set; }
        public string COMP_CODE { get; set; }
    }
}