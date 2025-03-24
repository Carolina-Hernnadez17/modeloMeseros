using System.ComponentModel.DataAnnotations;

namespace modeloMeseros.Models
{
    public class menu
    {
        [Key] 
        public int id { get; set; }
        public string tipo_menu { get; set; }
        public string tipo_venta {  get; set; }
        public TimeOnly? hora_inicio { get; set; }
        public TimeOnly? hora_fin { get; set; }
        public DateTime? fecha_inicio { get; set; }
        public DateTime? fecha_fin { get; set; }
        public int estado {  get; set; }
    }
}
