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
        //[HttpPost]
        public IActionResult CambiarEstadoPedido(int idPedido, string nuevoEstado, int idMesa)
        {
            var pedido = _context.Pedido_Local.FirstOrDefault(d => d.id_pedido == idPedido && d.id_mesa == idMesa);

            var mesa = _context.mesas.FirstOrDefault(d => d.id == idMesa);

            var detalles = _context.Detalle_Pedido
                .Where(d => d.encabezado_id == idPedido && d.tipo_venta == "Local")
                .ToList();

            if (pedido == null)
            {
                TempData["Error"] = "El pedido no fue encontrado.";
                return RedirectToAction("EstadoDeOrden");
            }
            else
            {
                if (detalles.Count == 0)
                {
                    TempData["Error"] = "No hay detalles asociados al pedido.";
                    return RedirectToAction("EstadoDeOrden");
                }
                else
                {
                    bool todosEntregadosOCancelados = detalles.All(d =>
                    d.estado == "Entregado" || d.estado == "Cancelado" && d.tipo_venta == "Local");

                    if (todosEntregadosOCancelados)
                    {
                        pedido.estado = nuevoEstado;
                        mesa.estado = 0;
                        mesa.disponibilidad = "libre";
                        _context.SaveChanges();
                        TempData["Mensaje"] = "Estado del pedido actualizado correctamente.";

                    }
                    else
                    {
                        TempData["Error"] = "Solo se puede cerrar la cuenta si todos los detalles están en estado 'Entregado' o 'Cancelado'.";
                    }
                }
                
            }

            return RedirectToAction("EstadoDeOrden");
        }

    }
}
