using System.ComponentModel.DataAnnotations;

namespace modeloMeseros.Models
{
    public class cargo
    {
        [Key]
        public int Id { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }
    }
}
