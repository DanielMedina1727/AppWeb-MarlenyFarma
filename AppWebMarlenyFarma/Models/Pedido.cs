using System.Text.Json.Serialization;

namespace AppWebMarlenyFarma.Models
{
    public class Pedido
    {
        [JsonRequired]
        public int PedidoId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        [JsonRequired]
        public DateTime FechaPedido { get; set; }
        [JsonRequired]
        public decimal Total { get; set; }
        [JsonRequired]
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
        public string NombreCliente { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Notas { get; set; }

        public ICollection<ItemPedido> Items { get; set; } = new List<ItemPedido>();
    }

    public enum EstadoPedido
    {
        Pendiente,
        Confirmado,
        EnPreparacion,
        Enviado,
        Entregado,
        Cancelado
    }
}