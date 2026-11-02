using System.Text.Json.Serialization;

namespace AppWebMarlenyFarma.Models
{
    public class ItemPedido
    {
        [JsonRequired]
        public int ItemPedidoId { get; set; }
        [JsonRequired]
        public int PedidoId { get; set; }
        [JsonRequired]
        public int ProductoId { get; set; }
        [JsonRequired]
        public int Cantidad { get; set; }
        [JsonRequired]
        public decimal PrecioUnitario { get; set; }

        public Pedido? Pedido { get; set; }
        public Producto? Producto { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
