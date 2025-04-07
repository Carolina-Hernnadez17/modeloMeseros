using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using modeloMeseros.Models;

namespace modeloMeseros.Controllers
{
    public class MesasController : Controller
    {
        private readonly MeserosContext _context;

        public MesasController(MeserosContext context)
        {
            _context = context;
        }

        // GET: Mesas
        public async Task<IActionResult> Index()
        {
            return View(await _context.mesas.ToListAsync());
        }

        // GET: Mesas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesas = await _context.mesas
                .FirstOrDefaultAsync(m => m.id == id);
            if (mesas == null)
            {
                return NotFound();
            }

            return View(mesas);
        }

        // GET: Mesas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mesas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,numero,capacidad,disponibilidad,estado")] mesas mesas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mesas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mesas);
        }

        // GET: Mesas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesas = await _context.mesas.FindAsync(id);
            if (mesas == null)
            {
                return NotFound();
            }
            return View(mesas);
        }

        // POST: Mesas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,numero,capacidad,disponibilidad,estado")] mesas mesas)
        {
            if (id != mesas.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mesas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!mesasExists(mesas.id))
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
            return View(mesas);
        }

        // GET: Mesas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesas = await _context.mesas
                .FirstOrDefaultAsync(m => m.id == id);
            if (mesas == null)
            {
                return NotFound();
            }

            return View(mesas);
        }

        // POST: Mesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mesas = await _context.mesas.FindAsync(id);
            if (mesas != null)
            {
                _context.mesas.Remove(mesas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ocupar(int id)
        {
            var mesa = await _context.mesas.FindAsync(id);
            if (mesa == null)
            {
                return NotFound();
            }

            mesa.estado = 1;  // 1 representa "ocupado"
            mesa.disponibilidad = "ocupado";

            try
            {
                _context.Update(mesa);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MesasExists(mesa.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            HttpContext.Session.Clear();


            HttpContext.Session.SetInt32("id_mesa", id);

            return RedirectToAction("Create", "Pedido_Local");
        }


        // Método para verificar si la mesa existe
        private bool MesasExists(int id)
        {
            return _context.mesas.Any(e => e.id == id);
        }

        public async Task<IActionResult> MesasOcupadas()
        {
            var mesasOcupadas = await _context.mesas
                .Where(m => m.estado == 1)  // Filtrar mesas ocupadas
                .ToListAsync();

            return View(mesasOcupadas);
        }


        private bool mesasExists(int id)
        {
            return _context.mesas.Any(e => e.id == id);
        }

        public IActionResult BuscarPedido(int id)
        {

            HttpContext.Session.Clear();

            HttpContext.Session.SetInt32("id_mesaOcupada", id);


            return RedirectToAction("Create", "Pedido_Local");
        }


    }
}
