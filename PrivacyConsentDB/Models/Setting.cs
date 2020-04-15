using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Models
{

    public class Setting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("종류")]
        public SettingType type { get; set; }
        [DisplayName("항목")]
        public string name { get; set; }
        [DisplayName("설정값")]
        public string value { get; set; }

        public string COMP_CODE { get; set; }
    }
}