using Microsoft.AspNetCore.Mvc;
using modeloMeseros.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class Pedido_LocalController : Controller
{
    private readonly MeserosContext _context;

    public Pedido_LocalController(MeserosContext context)
    {
        _context = context;
    }

    public IActionResult Create(string tipoMenu, int? tipoCombo, int? tipoPromocion)
    {
        // Obtener la hora actual como TimeSpan
        var horaActual = DateTime.Now.TimeOfDay;

        // Obtener las listas filtradas
        var platos = ObtenerPlatos(tipoMenu, horaActual).ToList();
        var combos = ObtenerCombos(tipoMenu, horaActual).ToList();
        var promociones = ObtenerPromociones(tipoMenu, horaActual).ToList();

        // Si no hay platos disponibles, mostrar mensaje de error
        if (!platos.Any())
        {
            ViewBag.ErrorMessage = "No hay platos disponibles para este menú en este horario.";
        }

        // Crear el modelo para la vista
        var model = new PedidoPlatosViewModel
        {
            Pedido = new Pedido_Local(),
            ListaPlatos = platos,
            ListaCombos = combos,
            ListaPromocion = promociones
        };

        // Pasar los filtros seleccionados a la vista
        ViewBag.TipoMenu = tipoMenu;
        ViewBag.ListaCombos = combos;
        ViewBag.ListaPromociones = promociones;
        ViewBag.SelectedCombo = tipoCombo;
        ViewBag.SelectedPromocion = tipoPromocion;

        return View("~/Views/Pedido_Local/Create.cshtml", model);
    }

    // Método privado para obtener platos filtrados
    private IEnumerable<platos> ObtenerPlatos(string tipoMenu, TimeSpan horaActual)
    {
        // Obtener los datos de la base de datos
        var query = _context.platos
            .Join(_context.menu_plato, p => p.id, mp => mp.plato_id, (p, mp) => new { p, mp })
            .Join(_context.menu, temp => temp.mp.menu_id, m => m.id, (temp, m) => new { temp.p, m, temp.mp })
            .Where(x =>
                (string.IsNullOrEmpty(tipoMenu) || x.m.tipo_menu == tipoMenu) &&  // Filtrado por tipo de menú
                x.m.estado == 1 &&  // Aseguramos que el menú esté activo
                x.mp.estado == 1 && // Aseguramos que el plato esté activo
                x.p.estado == 1);   // Aseguramos que el plato esté activo

        // Convertir a Enumerable para filtrar la hora en memoria
        return query.AsEnumerable()
            .Where(x =>
                horaActual >= (x.m.hora_inicio ?? TimeOnly.MinValue).ToTimeSpan() &&  // Comprobamos que la hora actual esté dentro del rango permitido
                horaActual <= (x.m.hora_fin ?? TimeOnly.MinValue).ToTimeSpan())       // Comprobamos que la hora actual esté dentro del rango permitido
            .Select(x => x.p);  // Seleccionamos solo la entidad del plato
    }

    // Método privado para obtener combos filtrados
    private IEnumerable<combos> ObtenerCombos(string tipoMenu, TimeSpan horaActual)
    {
        var query = _context.combos
            .Join(_context.menu_combo, c => c.id, mc => mc.combo_id, (c, mc) => new { c, mc })
            .Join(_context.menu, temp => temp.mc.menu_id, m => m.id, (temp, m) => new { temp.c, m, temp.mc })
            .Where(x =>
                (string.IsNullOrEmpty(tipoMenu) || x.m.tipo_menu == tipoMenu) &&  // Filtrado por tipo de menú
                x.m.estado == 1 &&  // Aseguramos que el menú esté activo
                x.mc.estado == 1 && // Aseguramos que el combo esté activo
                x.c.estado == 1);   // Aseguramos que el combo esté activo

        // Convertir la consulta a Enumerable para que podamos filtrar en memoria
        return query.AsEnumerable()
            .Where(x =>
                horaActual >= (x.m.hora_inicio ?? TimeOnly.MinValue).ToTimeSpan() &&  // Comprobamos que la hora actual esté dentro del rango permitido
                horaActual <= (x.m.hora_fin ?? TimeOnly.MinValue).ToTimeSpan())       // Comprobamos que la hora actual esté dentro del rango permitido
            .Select(x => x.c);  // Seleccionamos solo la entidad del combo
    }


    // Método privado para obtener promociones filtradas
    private IEnumerable<promociones> ObtenerPromociones(string tipoMenu, TimeSpan horaActual)
    {
        var query = _context.promociones
            .Join(_context.combo_promocion, p => p.id, cp => cp.promocion_id, (p, cp) => new { p, cp })
            .Join(_context.combos, temp => temp.cp.combo_id, c => c.id, (temp, c) => new { temp.p, c, temp.cp })
            .Join(_context.menu_combo, temp => temp.c.id, mc => mc.combo_id, (temp, mc) => new { temp.p, temp.c, temp.cp, mc })
            .Join(_context.menu, temp => temp.mc.menu_id, m => m.id, (temp, m) => new { temp.p, temp.c, temp.cp, temp.mc, m })
            .Where(x =>
                (string.IsNullOrEmpty(tipoMenu) || x.m.tipo_menu == tipoMenu) &&  // Filtrado por tipo de menú
                x.m.estado == 1 &&  // Aseguramos que el menú esté activo
                x.mc.estado == 1 && // Aseguramos que el combo esté activo
                x.c.estado == 1 &&  // Aseguramos que el combo esté activo
                x.p.estado == 1 &&  // Aseguramos que la promoción esté activa
                x.cp.estado == 1);  // Aseguramos que la relación combo-promoción esté activa

        // Convertir la consulta a Enumerable para que podamos filtrar en memoria
        return query.AsEnumerable()
            .Where(x =>
                x.cp.fecha_inicio <= DateTime.Now.Date &&  // Verificamos que la fecha de inicio de la promoción haya pasado
                x.cp.fecha_fin >= DateTime.Now.Date &&     // Verificamos que la fecha de fin de la promoción aún no haya pasado
                horaActual >= (x.m.hora_inicio ?? TimeOnly.MinValue).ToTimeSpan() &&  // Comprobamos que la hora actual esté dentro del rango permitido
                horaActual <= (x.m.hora_fin ?? TimeOnly.MinValue).ToTimeSpan())       // Comprobamos que la hora actual esté dentro del rango permitido
            .Select(x => x.p);  // Seleccionamos solo la entidad de la promoción
    }


}

