
using PcmsTask.Commons;
using System;
using System.Collections.Generic;

namespace PcmsTask.Models
{
    public partial class Channel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Channel()
        {
            Collections = new HashSet<Collection>();
            Privacies = new HashSet<Privacy>();
        }

        public int ID { get; set; }

        public string name { get; set; }

        public BoundType bound { get; set; }

        public int type { get; set; }

        public int athour { get; set; }

        public bool usage { get; set; }

        public DateTime modifieddate { get; set; }

        public string host { get; set; }

        public string account { get; set; }

        public string pwd { get; set; }

        public string path { get; set; }

        public ActionStatus action { get; set; }

        public bool Instantrun { get; set; }

        public string exportpath { get; set; }

        public string backuppath { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Collection> Collections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Privacy> Privacies { get; set; }
    }
}
