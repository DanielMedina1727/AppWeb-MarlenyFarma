namespace AppWebMarlenyFarma.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal PrecioOriginal { get; set; }
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
        public bool EsDestacado { get; set; }
        public bool RequiereReceta { get; set; }
        public int CategoriaId { get; set; }
        public DateTime FechaCreacion { get; set; } 

        public Categoria? Categoria { get; set; }
        public ICollection<ItemCarrito> ItemsCarrito { get; set; } = new List<ItemCarrito>();
        public ICollection<ItemPedido> ItemsPedido { get; set; } = new List<ItemPedido>();
    }
}

