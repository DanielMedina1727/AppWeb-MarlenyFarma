using Microsoft.EntityFrameworkCore;
using AppWebMarlenyFarma.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AppWebMarlenyFarma.Data
{
    public class FarmaDbContext : IdentityDbContext
    {
        public FarmaDbContext(DbContextOptions<FarmaDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ItemCarrito> ItemsCarrito { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItemsPedido { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Precisión decimal
            modelBuilder.Entity<ItemCarrito>().Property(ic => ic.PrecioUnitario).HasPrecision(18, 2);
            modelBuilder.Entity<ItemPedido>().Property(ip => ip.PrecioUnitario).HasPrecision(18, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.Total).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.Precio).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.PrecioOriginal).HasPrecision(18, 2);

            // Fechas automáticas (ajusta los nombres de propiedad según tus modelos)
            modelBuilder.Entity<Producto>()
                .Property(p => p.FechaCreacion)   // ← asegúrate de que exista en Producto
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Pedido>()
                .Property(p => p.FechaPedido)     // ← asegúrate de que exista en Pedido
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            // Relaciones (tus configuraciones originales)
            modelBuilder.Entity<ItemCarrito>()
                .HasOne(ic => ic.Carrito)
                .WithMany(c => c.Items)
                .HasForeignKey(ic => ic.CarritoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemCarrito>()
                .HasOne(ic => ic.Producto)
                .WithMany(p => p.ItemsCarrito)
                .HasForeignKey(ic => ic.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemPedido>()
                .HasOne(ip => ip.Pedido)
                .WithMany(p => p.Items)
                .HasForeignKey(ip => ip.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemPedido>()
                .HasOne(ip => ip.Producto)
                .WithMany(p => p.ItemsPedido)
                .HasForeignKey(ip => ip.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Proveedor)
                .WithMany(pr => pr.Productos)
                .HasForeignKey(p => p.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data (igual que lo tenías)
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor { ProveedorId = 1, Nombre = "Distribuidora FarmaSalud", Contacto = "Juan Pérez", Telefono = "987654321", Email = "ventas@farmasalud.com", Direccion = "Av. Los Libertadores 123, Lima", Ruc = "20123456789" },
                new Proveedor { ProveedorId = 2, Nombre = "Laboratorios MediTech", Contacto = "Ana Gómez", Telefono = "912345678", Email = "contacto@meditech.com", Direccion = "Jr. Carabaya 456, Lima", Ruc = "20987654321" },
                new Proveedor { ProveedorId = 3, Nombre = "Higiene & Bienestar S.A.C.", Contacto = "Carlos Ruiz", Telefono = "945612378", Email = "ventas@higieneybienestar.com", Direccion = "Av. El Derby 789, Santiago de Surco", Ruc = "20555666777" }
            );

            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { CategoriaId = 1, Nombre = "Analgésicos", Descripcion = "Medicamentos para el dolor" },
                new Categoria { CategoriaId = 2, Nombre = "Antibióticos", Descripcion = "Medicamentos antibacterianos" },
                new Categoria { CategoriaId = 3, Nombre = "Vitaminas", Descripcion = "Suplementos vitamínicos" },
                new Categoria { CategoriaId = 4, Nombre = "Higiene Personal", Descripcion = "Productos de higiene" },
                new Categoria { CategoriaId = 5, Nombre = "Dermacosméticos", Descripcion = "Productos para la piel" }
            );

            modelBuilder.Entity<Producto>().HasData(
                new Producto { ProductoId = 1, Nombre = "Paracetamol 500mg", Descripcion = "Tabletas de paracetamol", Precio = 8.50m, PrecioOriginal = 10.00m, Stock = 100, CategoriaId = 1, ProveedorId = 1, EsDestacado = true, RequiereReceta = false },
                new Producto { ProductoId = 2, Nombre = "Ibuprofeno 400mg", Descripcion = "Comprimidos de ibuprofeno", Precio = 12.00m, PrecioOriginal = 15.00m, Stock = 85, CategoriaId = 1, ProveedorId = 1, EsDestacado = true, RequiereReceta = false },
                new Producto { ProductoId = 3, Nombre = "Amoxicilina 500mg", Descripcion = "Cápsulas de amoxicilina", Precio = 22.00m, PrecioOriginal = 25.00m, Stock = 50, CategoriaId = 2, ProveedorId = 2, EsDestacado = false, RequiereReceta = true },
                new Producto { ProductoId = 4, Nombre = "Vitamina C 1000mg", Descripcion = "Tabletas effervescentes", Precio = 18.00m, PrecioOriginal = 20.00m, Stock = 120, CategoriaId = 3, ProveedorId = 2, EsDestacado = true, RequiereReceta = false },
                new Producto { ProductoId = 5, Nombre = "Jabón Neutro", Descripcion = "Barra de jabón neutro 100g", Precio = 5.00m, PrecioOriginal = 6.00m, Stock = 200, CategoriaId = 4, ProveedorId = 3, EsDestacado = false, RequiereReceta = false },
                new Producto { ProductoId = 6, Nombre = "Crema Facial Hidratante", Descripcion = "Crema nutritiva para el rostro", Precio = 35.00m, PrecioOriginal = 45.00m, Stock = 60, CategoriaId = 5, ProveedorId = 3, EsDestacado = true, RequiereReceta = false }
            );
        }
    }
}