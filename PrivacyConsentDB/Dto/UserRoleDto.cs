using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using static PrivacyConsentDB.Commons.MyRoleManager;

namespace PrivacyConsentDB.Dto
{
    public class UserRoleDto
    {        
        [DisplayName("사용자")]
        public string username { get; set; }
        [DisplayName("권한")]
        public List<RoleType> roletypes { get; set; }

        public UserRoleDto() {
            roletypes = new List<RoleType>();
        }
    }

    public class UserRoleUpdateDto
    {
        public string type { get; set; }
        public RoleType role { get; set; }
        public List<string> users { get; set; }
        public string company { get; set; }
    }
    public class UserRoleAddDto
    {
        public RoleType role { get; set; }
        public List<string> users { get; set; }
        public string company { get; set; }
    }
}