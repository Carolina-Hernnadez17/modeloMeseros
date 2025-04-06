using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using modeloMeseros.Models;

namespace modeloMeseros.Controllers
{
    public class EstadoOrdenController : Controller
    {
        private readonly MeserosContext _context;

        public EstadoOrdenController(MeserosContext context)
        {
            _context = context;
        }

        public IActionResult EstadoDeOrden()
        {
            var pedidos = _context.Pedido_Local
            .Where(p => p.estado == "Abierta")
            .ToList();

            var pedidoIds = pedidos.Select(p => p.id_pedido).ToList();

            var detalles = _context.Detalle_Pedido
                .Where(d => pedidoIds.Contains(d.encabezado_id)  && d.tipo_venta == "Local")
                .ToList();

            ViewBag.Pedidos = pedidos;
            ViewBag.Detalles_Pedido = detalles;

            return View();
        }

        //[HttpPost]
        //public IActionResult BuscarPedidos(string nombreCliente, int numeroMesa)
        //{
        //    var pedidos = _context.Pedido_Local
        //        .Where(p => p.nombre_cliente == nombreCliente && p.id_mesa == numeroMesa)
        //        .ToList();

        //    var detalles = _context.Detalle_Pedido
        //        .Where(d => pedidos.Select(p => p.id_pedido).Contains(d.encabezado_id))
        //        .ToList();

        //    ViewBag.Pedidos = pedidos;
        //    ViewBag.Detalles_Pedido = detalles;

        //    return View("EstadoDeOrden"); 
        //}
        ////Me falta arreglar esto 
        //[HttpPost]
        //public IActionResult CambiarEstado(int idPedido)
        //{
        //    var pedido = _context.Pedido_Local.FirstOrDefault(p => p.id_pedido == idPedido);

        //    if (pedido != null)
        //    {
        //        pedido.estado = "Cerrada";  
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("EstadoDeOrden"); 
        //}
        [HttpPost]
        public IActionResult BuscarPedidos(string nombreCliente, int numeroMesa)
        {
            var pedidos = _context.Pedido_Local
                .Where(p => p.nombre_cliente == nombreCliente && p.id_mesa == numeroMesa && p.estado == "Abierta")
                .ToList();

            var pedidoIds = pedidos.Select(p => p.id_pedido).ToList();

            var detalles = _context.Detalle_Pedido
                .Where(d => pedidoIds.Contains(d.encabezado_id) && d.tipo_venta == "Local")
                .ToList();

            ViewBag.Pedidos = pedidos;
            ViewBag.Detalles_Pedido = detalles;

            return View("EstadoDeOrden");
        }

        [HttpPost]
        public IActionResult CambiarEstadoDetalle(int idDetalle, string nuevoEstado)
        {
            var detalle = _context.Detalle_Pedido.FirstOrDefault(d => d.id_detalle_pedido == idDetalle);

            if (detalle != null)
            {
                if (nuevoEstado == "Cancelado" && detalle.estado != "Pendiente")
                {
                    TempData["Error"] = "Solo se puede cancelar un pedido en estado Pendiente.";
                }
                else
                {
                    detalle.estado = nuevoEstado;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("EstadoDeOrden");
        }

    }
}
