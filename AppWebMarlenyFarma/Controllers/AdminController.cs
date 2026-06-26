using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppWebMarlenyFarma.Data;
using AppWebMarlenyFarma.Models;

namespace AppWebMarlenyFarma.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly FarmaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(FarmaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // 1. DASHBOARD PRINCIPAL
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // Estadísticas Generales
            ViewBag.TotalVentas = await _context.Pedidos.Where(p => p.Estado != EstadoPedido.Cancelado).SumAsync(p => p.Total);
            ViewBag.CantidadPedidos = await _context.Pedidos.CountAsync();
            ViewBag.CantidadProductos = await _context.Productos.CountAsync();
            ViewBag.CantidadProveedores = await _context.Proveedores.CountAsync();
            ViewBag.StockBajoCount = await _context.Productos.CountAsync(p => p.Stock < 10);
            
            var clientes = await _userManager.GetUsersInRoleAsync("Cliente");
            ViewBag.CantidadClientes = clientes.Count;

            // Datos para Gráfico de Estados de Pedidos
            var pedidosPorEstado = await _context.Pedidos
                .GroupBy(p => p.Estado)
                .Select(g => new { Estado = g.Key.ToString(), Cantidad = g.Count() })
                .ToListAsync();

            ViewBag.EstadosPedidoJson = pedidosPorEstado;

            // Datos para Gráfico de Productos con Bajo Stock (menos de 15)
            var bajoStock = await _context.Productos
                .Where(p => p.Stock < 15)
                .OrderBy(p => p.Stock)
                .Take(5)
                .Select(p => new { Nombre = p.Nombre, Stock = p.Stock })
                .ToListAsync();

            ViewBag.BajoStockJson = bajoStock;

            // Recientes Pedidos
            var pedidosRecientes = await _context.Pedidos
                .OrderByDescending(p => p.FechaPedido)
                .Take(5)
                .ToListAsync();

            return View(pedidosRecientes);
        }

        // ==========================================
        // 2. GESTIÓN DE PRODUCTOS Y STOCK
        // ==========================================
        public async Task<IActionResult> Productos(string? busqueda, int page = 1)
        {
            const int pageSize = 10;
            IQueryable<Producto> query = _context.Productos.Include(p => p.Categoria).Include(p => p.Proveedor);

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

            ViewBag.Busqueda = busqueda;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(productos);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarStock(int productoId, int stockAdicional)
        {
            if (stockAdicional <= 0)
            {
                TempData["Error"] = "La cantidad a añadir debe ser mayor a cero.";
                return RedirectToAction(nameof(Productos));
            }

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto != null)
            {
                producto.Stock += stockAdicional;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Se agregaron {stockAdicional} unidades al stock de {producto.Nombre}.";
            }

            return RedirectToAction(nameof(Productos));
        }

        public async Task<IActionResult> CrearProducto()
        {
            ViewBag.Categorias = await _context.Categorias.ToListAsync();
            ViewBag.Proveedores = await _context.Proveedores.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.FechaCreacion = DateTime.Now;
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Producto creado con éxito.";
                return RedirectToAction(nameof(Productos));
            }

            ViewBag.Categorias = await _context.Categorias.ToListAsync();
            ViewBag.Proveedores = await _context.Proveedores.ToListAsync();
            return View(producto);
        }

        public async Task<IActionResult> EditarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            ViewBag.Categorias = await _context.Categorias.ToListAsync();
            ViewBag.Proveedores = await _context.Proveedores.ToListAsync();
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> EditarProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Producto modificado con éxito.";
                return RedirectToAction(nameof(Productos));
            }

            ViewBag.Categorias = await _context.Categorias.ToListAsync();
            ViewBag.Proveedores = await _context.Proveedores.ToListAsync();
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Producto eliminado del catálogo.";
            }
            return RedirectToAction(nameof(Productos));
        }

        // ==========================================
        // 3. GESTIÓN DE CATEGORÍAS
        // ==========================================
        public async Task<IActionResult> Categorias()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return View(categorias);
        }

        public IActionResult CrearCategoria()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearCategoria(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Categoría creada con éxito.";
                return RedirectToAction(nameof(Categorias));
            }
            return View(categoria);
        }

        public async Task<IActionResult> EditarCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> EditarCategoria(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Categorias.Update(categoria);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Categoría modificada con éxito.";
                return RedirectToAction(nameof(Categorias));
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias.Include(c => c.Productos).FirstOrDefaultAsync(c => c.CategoriaId == id);
            if (categoria != null)
            {
                if (categoria.Productos.Any())
                {
                    TempData["Error"] = "No se puede eliminar la categoría porque contiene productos asociados.";
                }
                else
                {
                    _context.Categorias.Remove(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Categoría eliminada con éxito.";
                }
            }
            return RedirectToAction(nameof(Categorias));
        }

        // ==========================================
        // 4. GESTIÓN DE PROVEEDORES
        // ==========================================
        public async Task<IActionResult> Proveedores()
        {
            var proveedores = await _context.Proveedores.ToListAsync();
            return View(proveedores);
        }

        public IActionResult CrearProveedor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearProveedor(Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Proveedor registrado con éxito.";
                return RedirectToAction(nameof(Proveedores));
            }
            return View(proveedor);
        }

        public async Task<IActionResult> EditarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> EditarProveedor(Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Proveedores.Update(proveedor);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Proveedor modificado con éxito.";
                return RedirectToAction(nameof(Proveedores));
            }
            return View(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.Include(p => p.Productos).FirstOrDefaultAsync(p => p.ProveedorId == id);
            if (proveedor != null)
            {
                if (proveedor.Productos.Any())
                {
                    TempData["Error"] = "No se puede eliminar este proveedor porque tiene productos asociados.";
                }
                else
                {
                    _context.Proveedores.Remove(proveedor);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Proveedor eliminado con éxito.";
                }
            }
            return RedirectToAction(nameof(Proveedores));
        }

        // ==========================================
        // 5. HISTORIAL DE COMPRAS (PEDIDOS)
        // ==========================================
        public async Task<IActionResult> HistorialPedidos()
        {
            var pedidos = await _context.Pedidos
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
            return View(pedidos);
        }

        public async Task<IActionResult> DetallePedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarEstadoPedido(int pedidoId, EstadoPedido estado)
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido != null)
            {
                pedido.Estado = estado;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Estado del pedido actualizado con éxito.";
            }
            return RedirectToAction(nameof(DetallePedido), new { id = pedidoId });
        }

        // ==========================================
        // 6. GESTIÓN DE CLIENTES
        // ==========================================
        public async Task<IActionResult> Clientes()
        {
            var users = await _userManager.GetUsersInRoleAsync("Cliente");
            var infoClientes = new List<ClienteViewModel>();

            foreach (var user in users)
            {
                var pedidosUsuario = await _context.Pedidos
                    .Where(p => p.UsuarioId == user.Id || p.Email == user.Email)
                    .ToListAsync();

                infoClientes.Add(new ClienteViewModel
                {
                    UsuarioId = user.Id,
                    Email = user.Email ?? "Sin email",
                    Username = user.UserName ?? "Sin usuario",
                    TotalPedidos = pedidosUsuario.Count,
                    TotalGastado = pedidosUsuario.Where(p => p.Estado != EstadoPedido.Cancelado).Sum(p => p.Total)
                });
            }

            return View(infoClientes);
        }
    }

    public class ClienteViewModel
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int TotalPedidos { get; set; }
        public decimal TotalGastado { get; set; }
    }
}
