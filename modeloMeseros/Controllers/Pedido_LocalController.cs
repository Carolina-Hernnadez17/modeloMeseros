using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using modeloMeseros.Models;

namespace modeloMeseros.Controllers
{
    public class Pedido_LocalController : Controller
    {
        private readonly MeserosContext _context;

        public Pedido_LocalController(MeserosContext context)
        {
            _context = context;
        }

        // Método GET para mostrar la vista con el filtro
        public IActionResult Create(string tipoMenu)
        {
            // Verificar si el tipo de menú se recibió correctamente
            Console.WriteLine($"Tipo de menú recibido: {tipoMenu}");

            // Obtener platos según el tipo de menú seleccionado
            var platos = _context.platos
                .Join(_context.menu_plato, p => p.id, mp => mp.plato_id, (p, mp) => new { p, mp })
                .Join(_context.menu, temp => temp.mp.menu_id, m => m.id, (temp, m) => new { temp.p, m })
                .Where(x => string.IsNullOrEmpty(tipoMenu) || x.m.tipo_menu == tipoMenu)
                .Select(x => x.p)
                .ToList();

            Console.WriteLine($"Total de platos encontrados: {platos.Count}");

            if (!platos.Any())
            {
                ViewBag.ErrorMessage = "No hay platos disponibles para este menú.";
            }

            var model = new PedidoPlatosViewModel
            {
                Pedido = new Pedido_Local(),
                ListaPlatos = platos
            };

            ViewBag.TipoMenu = tipoMenu; // Para que el `select` recuerde la selección

            return View(model);
        }

        // Método POST para guardar el pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id_pedido,id_mesa,nombre_cliente,fechaApertura,estado,id_mesero")] Pedido_Local pedido_Local)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido_Local);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si hay un error, recargar los platos
            var model = new PedidoPlatosViewModel
            {
                Pedido = pedido_Local,
                ListaPlatos = _context.platos.ToList()
            };

            return View(model);
        }
    }
}
