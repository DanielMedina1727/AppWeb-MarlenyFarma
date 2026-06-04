using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppWebMarlenyFarma.Data;
using AppWebMarlenyFarma.Models;

namespace AppWebMarlenyFarma.Controllers
{
    public class ProductosController : Controller
    {
        private readonly FarmaDbContext _context;

        public ProductosController(FarmaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoriaId, string? busqueda, int page = 1)
        {
            const int pageSize = 12;

            IQueryable<Producto> query = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock > 0);

            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);
            }

            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(p => p.Nombre.Contains(busqueda) || p.Descripcion.Contains(busqueda));
            }

            var totalItems = await query.CountAsync();
            var productos = await query
                .OrderBy(p => p.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categorias = await _context.Categorias.ToListAsync();

            ViewBag.CategoriaId = categoriaId;
            ViewBag.Busqueda = busqueda;
            ViewBag.Categorias = categorias;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(productos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }
    }
}

