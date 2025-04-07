using Microsoft.EntityFrameworkCore;

namespace modeloMeseros.Models
{
    public class MeserosContext: DbContext
    {
        public MeserosContext(DbContextOptions options) : base(options)
        { }

        public DbSet<combo_promocion> combo_promocion { get; set; }
        public DbSet<combos> combos { get; set; }
        public DbSet<Detalle_Pedido> Detalle_Pedido { get; set; }
        public DbSet<menu> menu { get; set; }
        public DbSet<menu_combo> menu_combo { get; set; }
        public DbSet<menu_plato> menu_plato { get; set; }
        public DbSet<mesas> mesas { get; set; }
        public DbSet<Pedido_Local> Pedido_Local { get; set; }
        public DbSet<platos> platos { get; set; }
        public DbSet<platos_combos> platos_combos { get;set; }
        public DbSet<promociones> promociones { get; set; }
        public DbSet<categoria> categoria { get; set; }
        public DbSet<empleados> empleados { get; set; }
        public DbSet<cargo> cargo { get; set; }

    }
}
