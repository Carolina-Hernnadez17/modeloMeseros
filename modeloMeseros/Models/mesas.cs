using System.ComponentModel.DataAnnotations;

namespace modeloMeseros.Models
{
    public class mesas
    {
        [Key]

        public int id { get; set; }

        public int numero { get; set; }
        public int capacidad { get; set; }
        public string disponibilidad { get; set; }
        public int estado { get; set; }
    }
}
