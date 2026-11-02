using System.Text.Json.Serialization;

namespace AppWebMarlenyFarma.Models
{
    public class ItemCarrito
    {
        [JsonRequired]
        public int ItemCarritoId { get; set; }
        [JsonRequired]
        public int CarritoId { get; set; }
        [JsonRequired]
        public int ProductoId { get; set; }
        [JsonRequired]
        public int Cantidad { get; set; }
        [JsonRequired]
        public decimal PrecioUnitario { get; set; }

        public Carrito? Carrito { get; set; }
        public Producto? Producto { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
