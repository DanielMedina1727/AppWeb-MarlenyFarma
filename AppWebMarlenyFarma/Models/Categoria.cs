namespace AppWebMarlenyFarma.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}