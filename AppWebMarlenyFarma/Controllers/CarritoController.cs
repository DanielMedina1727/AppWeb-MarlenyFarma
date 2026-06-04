using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AppWebMarlenyFarma.Data;
using AppWebMarlenyFarma.Models;

namespace AppWebMarlenyFarma.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        private readonly FarmaDbContext _context;

        public CarritoController(FarmaDbContext context)
        {
            _context = context;
        }

        private async Task<Carrito> ObtenerOCrearCarrito()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            
            if (string.IsNullOrEmpty(usuarioId))
            {
                throw new InvalidOperationException("No se pudo identificar al usuario. Inicie sesión nuevamente.");
            }

            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Activo);

            if (carrito == null)
            {
                carrito = new Carrito { UsuarioId = usuarioId, Activo = true };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            return carrito;
        }

        public async Task<IActionResult> Index()
        {
            var carrito = await ObtenerOCrearCarrito();
            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarProducto(int productoId, int cantidad)
        {
            // Verificar si el usuario NO está autenticado
            if (User?.Identity?.IsAuthenticated != true)
            {
                // Guardar producto pendiente en sesión
                HttpContext.Session.SetInt32("ProductoPendienteId", productoId);
                HttpContext.Session.SetInt32("ProductoPendienteCantidad", cantidad);
                // Redirigir a login con returnUrl a la página de detalle del producto
                return RedirectToAction("Login", "Cuenta", new { returnUrl = Url.Action("Detalle", "Productos", new { id = productoId }) });
            }

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null || producto.Stock < cantidad)
                return BadRequest("Producto no disponible");

            var carrito = await ObtenerOCrearCarrito();
            var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

            if (itemExistente != null)
                itemExistente.Cantidad += cantidad;
            else
            {
                var nuevoItem = new ItemCarrito
                {
                    CarritoId = carrito.CarritoId,
                    ProductoId = productoId,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                };
                carrito.Items.Add(nuevoItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCantidadItems()
        {
            if (User?.Identity?.IsAuthenticated != true) return Ok(0);
            var carrito = await ObtenerOCrearCarrito();
            var cantidad = carrito?.Items?.Sum(i => i.Cantidad) ?? 0;
            return Ok(cantidad);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int itemId)
        {
            var item = await _context.ItemsCarrito.FindAsync(itemId);
            if (item != null)
            {
                _context.ItemsCarrito.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int itemId, int cantidad)
        {
            if (cantidad <= 0)
            {
                return await EliminarProducto(itemId);
            }

            var item = await _context.ItemsCarrito.Include(i => i.Producto).FirstOrDefaultAsync(i => i.ItemCarritoId == itemId);
            if (item != null && item.Producto!.Stock >= cantidad)
            {
                item.Cantidad = cantidad;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            var carrito = await ObtenerOCrearCarrito();
            if (carrito.Items.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPedido(string nombreCliente, string email, string telefono, string direccion, string? notas)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // ✅ Validación similar
            if (string.IsNullOrEmpty(usuarioId))
            {
                return RedirectToAction("Login", "Account");
            }

            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Activo);

            if (carrito == null || carrito.Items.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,   // ✅ Ahora usuarioId es seguro (no null)
                NombreCliente = nombreCliente,
                Email = email,
                Telefono = telefono,
                Direccion = direccion,
                Notas = notas,
                Total = carrito.ObtenerTotal(),
                Estado = EstadoPedido.Pendiente
            };

            foreach (var item in carrito.Items)
            {
                pedido.Items.Add(new ItemPedido
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                });

                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto != null)
                {
                    producto.Stock -= item.Cantidad;
                }
            }

            _context.Pedidos.Add(pedido);
            carrito.Activo = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ConfirmacionPedido), new { pedidoId = pedido.PedidoId });
        }

        public async Task<IActionResult> ConfirmacionPedido(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
    }
}
