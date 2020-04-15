using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class SettingsViewModel
    {
        public List<Setting> PBGSettings { get; set; }
        public List<Setting> UpjohnSettings { get; set; }
    }
}