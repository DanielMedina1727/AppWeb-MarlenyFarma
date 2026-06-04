using Microsoft.AspNetCore.Mvc;
using AppWebMarlenyFarma.Data;
using Microsoft.EntityFrameworkCore;

namespace AppWebMarlenyFarma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosApiController : ControllerBase
    {
        private readonly FarmaDbContext _context;

        public ProductosApiController(FarmaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos(int? categoriaId = null, string? busqueda = null)
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock > 0)
                .AsQueryable();

            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);
            }

            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(p => p.Nombre.Contains(busqueda));
            }

            var productos = await query.ToListAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetProductosPorCategoria(int categoriaId)
        {
            var productos = await _context.Productos
                .Where(p => p.CategoriaId == categoriaId && p.Stock > 0)
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("destacados")]
        public async Task<IActionResult> GetProductosDestacados()
        {
            var productos = await _context.Productos
                .Where(p => p.EsDestacado && p.Stock > 0)
                .Take(6)
                .ToListAsync();

            return Ok(productos);
        }
    }
}

