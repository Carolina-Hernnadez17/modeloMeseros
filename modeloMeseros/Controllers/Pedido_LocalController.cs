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
    public class Pedido_LocalController : Controller
    {
        private readonly MeserosContext _context;

        public Pedido_LocalController(MeserosContext context)
        {
            _context = context;
        }

        // GET: Pedido_Local
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pedido_Local.ToListAsync());
        }

        // GET: Pedido_Local/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido_Local = await _context.Pedido_Local
                .FirstOrDefaultAsync(m => m.id_pedido == id);
            if (pedido_Local == null)
            {
                return NotFound();
            }

            return View(pedido_Local);
        }

        // GET: Pedido_Local/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pedido_Local/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            return View(pedido_Local);
        }

        // GET: Pedido_Local/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido_Local = await _context.Pedido_Local.FindAsync(id);
            if (pedido_Local == null)
            {
                return NotFound();
            }
            return View(pedido_Local);
        }

        // POST: Pedido_Local/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_pedido,id_mesa,nombre_cliente,fechaApertura,estado,id_mesero")] Pedido_Local pedido_Local)
        {
            if (id != pedido_Local.id_pedido)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido_Local);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Pedido_LocalExists(pedido_Local.id_pedido))
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
            return View(pedido_Local);
        }

        // GET: Pedido_Local/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido_Local = await _context.Pedido_Local
                .FirstOrDefaultAsync(m => m.id_pedido == id);
            if (pedido_Local == null)
            {
                return NotFound();
            }

            return View(pedido_Local);
        }

        // POST: Pedido_Local/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido_Local = await _context.Pedido_Local.FindAsync(id);
            if (pedido_Local != null)
            {
                _context.Pedido_Local.Remove(pedido_Local);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Pedido_LocalExists(int id)
        {
            return _context.Pedido_Local.Any(e => e.id_pedido == id);
        }
    }
}
