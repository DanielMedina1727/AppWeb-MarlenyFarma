using System.Text.Json.Serialization;

namespace AppWebMarlenyFarma.Models
{
    public class Carrito
    {
        [JsonRequired]
        public int CarritoId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        [JsonRequired]
        public DateTime FechaCreacion { get; set; }
        [JsonRequired]
        public bool Activo { get; set; } = true;

        public ICollection<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();

        public decimal ObtenerTotal() => Items.Sum(i => i.Subtotal);
    }
}