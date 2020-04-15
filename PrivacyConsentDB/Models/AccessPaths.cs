namespace PrivacyConsentDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AccessPaths
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AccessPaths()
        {
            AccessAuthorities = new HashSet<AccessAuthorities>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string path { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        public DateTime createdate { get; set; }

        [Required]
        [StringLength(50)]
        public string creator { get; set; }

        public DateTime? modifieddate { get; set; }

        [StringLength(50)]
        public string modifier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccessAuthorities> AccessAuthorities { get; set; }
    }
}
