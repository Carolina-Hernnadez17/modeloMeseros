using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using modeloMeseros.Models;

namespace modeloMeseros.Controllers
{
    public class Detalle_PedidoController : Controller
    {
        private readonly MeserosContext _context;

        public Detalle_PedidoController(MeserosContext context)
        {
            _context = context;
        }

        // GET: Detalle_Pedido
        public async Task<IActionResult> Index()
        {
            var idMesa = HttpContext.Session.GetInt32("idMesa");
            var pedido = _context.Pedido_Local.FirstOrDefault(d => d.id_mesa == idMesa && d.estado == "Abierta");

            string nombreCliente = pedido.nombre_cliente;
            int idMesero = pedido.id_mesero;
            int idPedido = pedido.id_pedido;

            var detallesPedido = await _context.Detalle_Pedido
                .Where(d => d.encabezado_id == idPedido)
                .ToListAsync();

            var detallesConPlatillo = detallesPedido.Select(detalle =>
            {
                var plato = _context.platos.FirstOrDefault(p => p.id == detalle.item_id);
                var combo = _context.combos.FirstOrDefault(c => c.id == detalle.item_id);

                return new DetalleConPlatilloViewModel
                {
                    DetallePedidoId = detalle.id_detalle_pedido,
                    NombrePlatillo = plato?.nombre ?? combo?.nombre ?? "Item desconocido",
                    DescripcionPlatillo = plato?.descripcion ?? combo?.descripcion ?? "Descripción no disponible",
                    Comentarios = detalle.comentarios
                };
            }).ToList();



            var model = new DetallePedidoViewModel
            {
                IdMesa = idMesa,
                NombreCliente = nombreCliente,
                IdMesero = idMesero,
                Detalles = detallesConPlatillo
            };

            return View(model);
        }

        // GET: Detalle_Pedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle_Pedido = await _context.Detalle_Pedido
                .FirstOrDefaultAsync(m => m.id_detalle_pedido == id);
            if (detalle_Pedido == null)
            {
                return NotFound();
            }

            return View(detalle_Pedido);
        }

        // GET: Detalle_Pedido/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Detalle_Pedido/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id_detalle_pedido,encabezado_id,tipo_venta,tipo_Item,item_id,cantidad,comentarios,estado,subtotal")] Detalle_Pedido detalle_Pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalle_Pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(detalle_Pedido);
        }

        // GET: Detalle_Pedido/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle_Pedido = await _context.Detalle_Pedido.FindAsync(id);
            if (detalle_Pedido == null)
            {
                return NotFound();
            }
            return View(detalle_Pedido);
        }

        // POST: Detalle_Pedido/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_detalle_pedido,encabezado_id,tipo_venta,tipo_Item,item_id,cantidad,comentarios,estado,subtotal")] Detalle_Pedido detalle_Pedido)
        {
            if (id != detalle_Pedido.id_detalle_pedido)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalle_Pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Detalle_PedidoExists(detalle_Pedido.id_detalle_pedido))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(detalle_Pedido);
        }

        // GET: Detalle_Pedido/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle_Pedido = await _context.Detalle_Pedido
                .FirstOrDefaultAsync(m => m.id_detalle_pedido == id);
            if (detalle_Pedido == null)
            {
                return NotFound();
            }

            return View(detalle_Pedido);
        }

        // POST: Detalle_Pedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalle_Pedido = await _context.Detalle_Pedido.FindAsync(id);
            if (detalle_Pedido != null)
            {
                _context.Detalle_Pedido.Remove(detalle_Pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Detalle_PedidoExists(int id)
        {
            return _context.Detalle_Pedido.Any(e => e.id_detalle_pedido == id);
        }
    }
}
