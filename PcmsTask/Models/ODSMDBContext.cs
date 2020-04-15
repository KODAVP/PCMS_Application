using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcmsTask.Models
{
    public class ODSMDBContext : DbContext
    {
        public ODSMDBContext() : base("name=ODSM")
        {
        }

        public virtual DbSet<VW_CONSENT> VW_CONSENTs { get; set; }
        public virtual DbSet<VW_CONSENT_ALIGNMENT> VW_CONSENT_ALIGNMENTs { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
        
    }
}
