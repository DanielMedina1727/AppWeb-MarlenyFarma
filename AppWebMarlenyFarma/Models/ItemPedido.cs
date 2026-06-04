namespace AppWebMarlenyFarma.Models
{
    public class ItemPedido
    {
        public int ItemPedidoId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public Pedido? Pedido { get; set; }
        public Producto? Producto { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
