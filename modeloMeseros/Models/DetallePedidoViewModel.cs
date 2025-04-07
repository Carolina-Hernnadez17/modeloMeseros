namespace modeloMeseros.Models
{
    public class DetallePedidoViewModel
    {
        public int? IdMesero { get; set; }
        public int? IdMesa { get; set; }
        public string NombreCliente { get; set; }
        public IEnumerable<DetalleConPlatilloViewModel> Detalles { get; set; }
    }
}
