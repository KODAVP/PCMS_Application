using PrivacyConsentDB.Commons;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivacyConsentDB.Models
{   
    public class Channel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("채널명")]
        public string name { get; set;}
        [DisplayName("방향")]
        public BoundType bound { get; set; }
        [DisplayName("방식")]
        public ChannelType type { get; set; }
        [DisplayName("수집시각")]
        public int athour { get; set; }
        [DisplayName("사용여부")]
        public bool usage { get; set; }
        [DisplayName("수정일자")]
        public DateTime modifieddate { get; set; }

        [DisplayName("실행예약")]
        public bool Instantrun { get; set;}
        // SFTP 관련
        [DisplayName("호스트")]
        public string host { get; set; }
        [DisplayName("계정")]
        public string account { get; set; }
        [DisplayName("비밀번호")]
        public string pwd { get; set; }
        [DisplayName("수집/Rmote경로")]
        public string path { get; set; }

        [DisplayName("Local/NAS경로")]
        public string exportpath { get; set; }

        [DisplayName("백업경로")]
        public string backuppath { get; set; }

        public ActionStatus action { get; set; }

        public Channel() {
            this.action = ActionStatus.Waiting;
            this.path = "/";
            this.Instantrun = false;
        }
    }
}