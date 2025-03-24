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
            return View();
        }

        [HttpPost]
        public IActionResult BuscarPedidos(string nombreCliente, int numeroMesa)
        {
            var pedidos = _context.Pedido_Local
                .Where(p => p.nombre_cliente == nombreCliente && p.id_mesa == numeroMesa)
                .ToList();

            var detalles = _context.Detalle_Pedido
                .Where(d => pedidos.Select(p => p.id_pedido).Contains(d.encabezado_id))
                .ToList();

            ViewBag.Pedidos = pedidos;
            ViewBag.Detalles_Pedido = detalles;

            return View("EstadoDeOrden"); // Retornamos la misma vista con los datos
        }
        [HttpPost]
        public IActionResult CambiarEstado(int idPedido)
        {
            var pedido = _context.Pedido_Local.FirstOrDefault(p => p.id_pedido == idPedido);

            if (pedido != null)
            {
                pedido.estado = "Cerrada";  // Cambiar el estado a "Cerrada"
                _context.SaveChanges();
            }

            return RedirectToAction("EstadoDeOrden");  // Redirigir a la vista después de actualizar el estado
        }





        // GET: EstadoOrdenController



        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult EstadoDeOrden()
        //{
        //    return View();
        //}


        // GET: EstadoOrdenController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EstadoOrdenController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoOrdenController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoOrdenController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EstadoOrdenController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoOrdenController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EstadoOrdenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
