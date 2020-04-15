
using PcmsTask.Commons;
using System;
using System.Collections.Generic;

namespace PcmsTask.Models
{
    public partial class Collection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Collection()
        {
            N360File = new HashSet<N360File>();
            PforceRXFiles = new HashSet<PforceRXFile>();
        }

        public int ID { get; set; }

        public string name { get; set; }

        public string ftpname { get; set; }

        public CollectionStatus status { get; set; }

        public DateTime createdate { get; set; }

        public DateTime modifieddate { get; set; }

        public int channelId { get; set; }

        public virtual Channel Channel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<N360File> N360File { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PforceRXFile> PforceRXFiles { get; set; }
    }
}
