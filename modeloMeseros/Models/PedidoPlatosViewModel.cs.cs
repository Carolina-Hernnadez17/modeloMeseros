namespace modeloMeseros.Models
{
    
        public class PedidoPlatosViewModel
        {
            public Pedido_Local Pedido { get; set; } = new Pedido_Local();
            public List<platos> ListaPlatos { get; set; } = new List<platos>();  // Evita que sea null

            public List<categoria> ListaCategorias { get; set; } = new List<categoria>(); // Agregar esta lista

       




    }


}

