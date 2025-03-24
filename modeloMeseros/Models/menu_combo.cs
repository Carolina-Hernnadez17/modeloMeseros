using System.ComponentModel.DataAnnotations;

namespace modeloMeseros.Models
{
    public class menu_combo
    {
        [Key]
        public int id { get; set; }
        public int menu_id { get; set; }
        public int combo_id { get; set; }
        public int estado {  get; set; }
    }
}
