using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class Agreement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("제목")]
        public string title { get; set; }
        [DisplayName("내용")]
        public string contents { get; set; }
        [DisplayName("작성일자")]
        public DateTime createdate { get; set; }
        [DisplayName("작성자")]
        public string creater { get; set; }
        [DisplayName("수정일자")]
        public DateTime modifieddate { get; set; }
        [DisplayName("수정자")]
        public string modifier { get; set; }
    }
}