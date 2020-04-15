using PrivacyConsentDB.Commons;
using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Dto
{
    public class ChannelDto
    {
        [DisplayName("채널명")]
        public string name { get; set; }
        [DisplayName("방식")]
        public BoundType bound { get; set; }
        [DisplayName("수정일자")]
        public string runtime { get; set; }
    }
}