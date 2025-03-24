using System.ComponentModel.DataAnnotations;

namespace modeloMeseros.Models
{
    public class platos_combos
    {
        [Key]
        public int id { get; set; }
        public int estado { get; set; }
        public int combo_id { get; set; }
        public int plato_id { get; set; }
    }
}
