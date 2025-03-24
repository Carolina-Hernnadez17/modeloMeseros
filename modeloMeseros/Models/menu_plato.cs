using System.ComponentModel.DataAnnotations;

namespace modeloMeseros.Models
{
    public class menu_plato
    {
        [Key]
        public int id { get; set; }
        public int menu_id { get; set; }
        public int plato_id { get; set; }
        public int estado { get; set; }
    }
}
