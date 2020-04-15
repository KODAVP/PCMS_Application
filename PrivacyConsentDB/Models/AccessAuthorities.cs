namespace PrivacyConsentDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AccessAuthorities
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [ForeignKey("AccessPaths")]
        public int pathID { get; set; }
        [ForeignKey("AccessRoles")]
        public int roleID { get; set; }

        public DateTime createdate { get; set; }

        [Required]
        [StringLength(50)]
        public string creator { get; set; }

        public DateTime? modifieddate { get; set; }

        [StringLength(50)]
        public string modifier { get; set; }

        public virtual AccessPaths AccessPaths { get; set; }

        public virtual AccessRoles AccessRoles { get; set; }
    }
}
