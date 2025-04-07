namespace modeloMeseros.Models
{
    
        public class PedidoPlatosViewModel
        {
            public Pedido_Local Pedido { get; set; } = new Pedido_Local();
            public List<platos> ListaPlatos { get; set; } = new List<platos>();  // Evita que sea null

            public List<categoria> ListaCategorias { get; set; } = new List<categoria>(); // Agregar esta lista

             
           public List<combos> ListaCombos { get; set; } = new List<combos>();  // Evita que sea null

           public List<promociones> ListaPromocion { get; set; } = new List<promociones>();  // Evita que sea null








    }


}

