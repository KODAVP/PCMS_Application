using PrivacyConsentDB.Commons;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivacyConsentDB.Models
{
    
    public class Collection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("파일명")]
        public string name { get; set; }
        [DisplayName("FTP경로")]
        public string ftpname { get; set; }
        [DisplayName("상태")]
        public CollectionStatus status { get; set; }

        [DisplayName("처리일자")]
        public DateTime createdate { get; set; }
        [DisplayName("수행일자")]
        public DateTime modifieddate { get; set; }

        public int channelId { get; set; }
        public virtual Channel channel { get; set; }
    }
}