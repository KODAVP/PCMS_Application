using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class AccessAuthoritiesDto
    {
        public IEnumerable<AccessPaths> AccessPaths;
        public IEnumerable<AccessRoles> AccessRoles;
        public IEnumerable<UserRole> UserRole;

        public AccessAuthorities AccessAuthorities;

        public AccessAuthoritiesDto()
        {

        }
    }
}