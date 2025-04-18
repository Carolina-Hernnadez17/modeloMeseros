﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        verifiacrMesa();

        var horaActual = DateTime.Now.TimeOfDay;

        var platos = ObtenerPlatos(tipoMenu, horaActual).ToList();
        var combos = ObtenerCombos(tipoMenu, horaActual).ToList();
        var promociones = ObtenerPromociones(tipoMenu, horaActual).ToList();

        if (tipoCombo.HasValue)
        {
            combos = combos.Where(c => c.id == tipoCombo.Value).ToList();
        }

        if (tipoPromocion.HasValue)
        {
            promociones = promociones.Where(p => p.id == tipoPromocion.Value).ToList();
        }

        if (!platos.Any())
        {
            ViewBag.ErrorMessage = "No hay platos disponibles para este menú en este horario.";
        }

        var model = new PedidoPlatosViewModel
        {
            Pedido = new Pedido_Local(),
            ListaPlatos = platos,
            ListaCombos = combos,
            ListaPromocion = promociones
        };

        ViewBag.TipoMenu = tipoMenu;
        ViewBag.ListaCombos = ObtenerCombos(tipoMenu, horaActual).ToList(); 
        ViewBag.ListaPromociones = ObtenerPromociones(tipoMenu, horaActual).ToList(); 
        ViewBag.SelectedCombo = tipoCombo;
        ViewBag.SelectedPromocion = tipoPromocion;

        return View("~/Views/Pedido_Local/Create.cshtml", model);
    }


    private IEnumerable<platos> ObtenerPlatos(string tipoMenu, TimeSpan horaActual)
    {
        verifiacrMesa();

        var query = _context.platos
            .Join(_context.menu_plato, p => p.id, mp => mp.plato_id, (p, mp) => new { p, mp })
            .Join(_context.menu, temp => temp.mp.menu_id, m => m.id, (temp, m) => new { temp.p, m, temp.mp })
            .Where(x =>
                (string.IsNullOrEmpty(tipoMenu) || x.m.tipo_menu == tipoMenu) &&  
                x.m.estado == 1 &&  
                x.mp.estado == 1 && 
                x.p.estado == 1);   

        return query.AsEnumerable()
            .Where(x =>
                horaActual >= (x.m.hora_inicio ?? TimeOnly.MinValue).ToTimeSpan() &&  
                horaActual <= (x.m.hora_fin ?? TimeOnly.MinValue).ToTimeSpan())       
            .Select(x => x.p);  
    }

    private IEnumerable<combos> ObtenerCombos(string tipoMenu, TimeSpan horaActual)
    {
        verifiacrMesa();

        var query = _context.combos
            .Join(_context.menu_combo, c => c.id, mc => mc.combo_id, (c, mc) => new { c, mc })
            .Join(_context.menu, temp => temp.mc.menu_id, m => m.id, (temp, m) => new { temp.c, m, temp.mc })
            .Where(x =>
                (string.IsNullOrEmpty(tipoMenu) || x.m.tipo_menu == tipoMenu) &&  
                x.m.estado == 1 &&  
                x.mc.estado == 1 && 
                x.c.estado == 1);   

        
        return query.AsEnumerable()
            .Where(x =>
                horaActual >= (x.m.hora_inicio ?? TimeOnly.MinValue).ToTimeSpan() &&  
                horaActual <= (x.m.hora_fin ?? TimeOnly.MinValue).ToTimeSpan())      
            .Select(x => x.c);  
    }


    private IEnumerable<promociones> ObtenerPromociones(string tipoMenu, TimeSpan horaActual)
    {
        verifiacrMesa();

         var query = _context.promociones
            .Join(_context.combo_promocion, p => p.id, cp => cp.promocion_id, (p, cp) => new { p, cp })
            .Join(_context.combos, temp => temp.cp.combo_id, c => c.id, (temp, c) => new { temp.p, c, temp.cp })
            .Join(_context.menu_combo, temp => temp.c.id, mc => mc.combo_id, (temp, mc) => new { temp.p, temp.c, temp.cp, mc })
            .Join(_context.menu, temp => temp.mc.menu_id, m => m.id, (temp, m) => new { temp.p, temp.c, temp.cp, temp.mc, m })
            .Where(x =>
                (string.IsNullOrEmpty(tipoMenu) || x.m.tipo_menu == tipoMenu) &&  
                x.m.estado == 1 &&  
                x.mc.estado == 1 && 
                x.c.estado == 1 &&  
                x.p.estado == 1 &&  
                x.cp.estado == 1);  

        return query.AsEnumerable()
            .Where(x =>
                x.cp.fecha_inicio <= DateTime.Now.Date &&  
                x.cp.fecha_fin >= DateTime.Now.Date &&     
                horaActual >= (x.m.hora_inicio ?? TimeOnly.MinValue).ToTimeSpan() &&  
                horaActual <= (x.m.hora_fin ?? TimeOnly.MinValue).ToTimeSpan())       
            .Select(x => x.p);  
    }

    [HttpPost]
    public ActionResult AbrirCuenta(PedidoPlatosViewModel model)
    {


        model.Pedido.estado = "Abierta";
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine($"Error: {error.ErrorMessage}");
        }
        if (!ModelState.IsValid)
        {
            
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage); 
            }
            return View("~/Views/Pedido_Local/Create.cshtml", model);
        }

        if (model.Pedido == null)
        {
            ModelState.AddModelError("", "El modelo del pedido no es válido.");
            return View("~/Views/Pedido_Local/Create.cshtml", model);
        }

        model.Pedido.fechaApertura = DateTime.Now;
       

        Console.WriteLine($"Guardando Pedido: Cliente: {model.Pedido.nombre_cliente}, Mesa: {model.Pedido.id_mesa}, Mesero: {model.Pedido.id_mesero}, Estado: {model.Pedido.estado}");

        try
        {
            
            if (_context == null)
            {
                ModelState.AddModelError("", "El contexto de base de datos no está configurado.");
                return View("~/Views/Pedido_Local/Create.cshtml", model);
            }

            
            _context.Pedido_Local.Add(model.Pedido);
            int result = _context.SaveChanges();

            ViewBag.nombreCliente = model.Pedido.nombre_cliente;
            ViewBag.idMesa = model.Pedido.id_mesa;
            ViewBag.idMesero = model.Pedido.id_mesero;




            
            if (result > 0)
            {
                Console.WriteLine($"Se guardaron {result} registros.");
            }
            else
            {
                Console.WriteLine("No se guardaron registros.");
            }

            return View("~/Views/Pedido_Local/Create.cshtml");
        }
        catch (Exception ex)
        {
            
            ModelState.AddModelError("", "Error al guardar: " + ex.Message);
            Console.WriteLine($"Error al guardar: {ex.Message}"); 
            return View("~/Views/Pedido_Local/Create.cshtml", model);
        }
    }

    [HttpPost]
    public IActionResult GuardarPlatos(string comentarios_plato,string tipo_item, decimal precio, int item_id, int cantidad_plato , int idMesita)
    {
        var pedido = _context.Pedido_Local.FirstOrDefault(d => d.id_mesa == idMesita && d.estado == "Abierta");

        if (comentarios_plato == null )
        {
            comentarios_plato = "Sin comentarios";
        }
        verifiacrMesa();


        if(cantidad_plato > 0)
        {
            var detalle_p = new Detalle_Pedido
                {
                    encabezado_id = pedido.id_pedido,
                    tipo_venta = "Local",
                    tipo_Item= tipo_item,
                    item_id= item_id,
                    cantidad= cantidad_plato,
                    comentarios= comentarios_plato,
                    estado="Pendiente",
                    subtotal= cantidad_plato * precio
                };
                _context.Detalle_Pedido.Add(detalle_p);
                _context.SaveChanges();
        }
        

        return View("~/Views/Pedido_Local/Create.cshtml");
    }
    [HttpPost]
    public IActionResult GuardarCombos(string comentarios,string tipo_item, decimal precio, int item_id, int cantidad, int idMesita)
    {
        var pedido = _context.Pedido_Local.FirstOrDefault(d => d.id_mesa == idMesita && d.estado == "Abierta");
        if (comentarios == null)
        {
            comentarios = "Sin comentarios";
        }
        verifiacrMesa();


        if (cantidad > 0)
        {
            var detalle_p = new Detalle_Pedido
                {
                    encabezado_id = pedido.id_pedido,
                    tipo_venta = "Local",
                    tipo_Item= tipo_item,
                    item_id= item_id,
                    cantidad= cantidad,
                    comentarios= comentarios,
                    estado="Pendiente",
                    subtotal= cantidad* precio
                };
                _context.Detalle_Pedido.Add(detalle_p);
                _context.SaveChanges();
        }
        

        return View("~/Views/Pedido_Local/Create.cshtml");
    }

    
    public IActionResult EnviarId(int id_mesa)
    {
        
        HttpContext.Session.SetInt32("IdPedido" , id_mesa);

        return RedirectToAction("Index", "Detalle_Pedido");
    }

    public void verifiacrMesa()
    {
        int? mesaLibre = HttpContext.Session.GetInt32("id_mesa");
        int? mesaOCupada = HttpContext.Session.GetInt32("id_mesaOcupada");

        if (mesaLibre != null)
        {


            var pedido = _context.Pedido_Local.FirstOrDefault(d => d.id_mesa == mesaLibre && d.estado == "Abierta");

            if (pedido != null)
            {
                HttpContext.Session.SetString("nombreCliente", pedido.nombre_cliente);
                HttpContext.Session.SetInt32("idMesero", pedido.id_mesero);
                HttpContext.Session.SetInt32("idMesa", pedido.id_mesa);

                ViewBag.nombreCliente = pedido.nombre_cliente;
                ViewBag.idMesero = pedido.id_mesero;
                ViewBag.idMesa = pedido.id_mesa;
            }
            else
            {
                HttpContext.Session.SetInt32("idMesa", mesaLibre.Value);
                ViewBag.idMesa = mesaLibre.Value;
            }


        }

        if (mesaOCupada != null)
        {
            var pedido = _context.Pedido_Local.FirstOrDefault(d => d.id_mesa == mesaOCupada && d.estado == "Abierta");

            if (pedido != null)
            {
                HttpContext.Session.SetString("nombreCliente", pedido.nombre_cliente);
                HttpContext.Session.SetInt32("idMesero", pedido.id_mesero);
                HttpContext.Session.SetInt32("idMesa", pedido.id_mesa);

                ViewBag.nombreCliente = pedido.nombre_cliente;
                ViewBag.idMesero = pedido.id_mesero;
                ViewBag.idMesa = pedido.id_mesa;
            }
        }
    }
}

