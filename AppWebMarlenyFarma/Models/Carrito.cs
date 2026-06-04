namespace AppWebMarlenyFarma.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } 
        public bool Activo { get; set; } = true;

        public ICollection<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();

        public decimal ObtenerTotal() => Items.Sum(i => i.Subtotal);
    }
}