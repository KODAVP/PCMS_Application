using static PcmsTask.Commons.Status;

namespace PcmsTask.Models
{
    public class Setting
    {
        public int ID { get; set; }

        public SettingType type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
}
