namespace AppWebMarlenyFarma.Models
{
    public class ItemCarrito
    {
        public int ItemCarritoId { get; set; }
        public int CarritoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public Carrito? Carrito { get; set; }
        public Producto? Producto { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}

