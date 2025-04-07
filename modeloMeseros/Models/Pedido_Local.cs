using System.ComponentModel.DataAnnotations;

namespace modeloMeseros.Models
{
    public class Pedido_Local
    {
        [Key]
        public int id_pedido { get; set; }

        [Required(ErrorMessage = "El número de mesa es obligatorio.")]
        public int id_mesa { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder los 100 caracteres.")]
        public string nombre_cliente { get; set; }

        [Required(ErrorMessage = "La fecha de apertura es obligatoria.")]
        public DateTime fechaApertura { get; set; }

        [Required(ErrorMessage = "El estado del pedido es obligatorio.")]
        public string estado { get; set; }

        [Required(ErrorMessage = "El ID del mesero es obligatorio.")]
        public int id_mesero { get; set; }
    }
}
