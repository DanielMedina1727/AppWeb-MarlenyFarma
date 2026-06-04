using AppWebMarlenyFarma.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AppWebMarlenyFarma.Data;
using Microsoft.EntityFrameworkCore;


namespace AppWebMarlenyFarma.Controllers
{
    public class HomeController : Controller
    {
        private readonly FarmaDbContext _context;

        public HomeController(FarmaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productosDestacados = await _context.Productos
                .Where(p => p.EsDestacado && p.Stock > 0)
                .Include(p => p.Categoria)
                .Take(6)
                .ToListAsync();

            var categorias = await _context.Categorias.ToListAsync();

            ViewBag.ProductosDestacados = productosDestacados;
            ViewBag.Categorias = categorias;

            return View();
        }

        public IActionResult Nosotros()
        {
            return View();
        }

        public IActionResult Contacto()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

