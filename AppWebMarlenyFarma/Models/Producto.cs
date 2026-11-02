using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AppWebMarlenyFarma.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        [JsonRequired]
        public decimal Precio { get; set; }
        [JsonRequired]
        public decimal PrecioOriginal { get; set; }
        [JsonRequired]
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
        [JsonRequired]
        public bool EsDestacado { get; set; }
        [JsonRequired]
        public bool RequiereReceta { get; set; }
        [JsonRequired]
        public int CategoriaId { get; set; }
        public int? ProveedorId { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Categoria? Categoria { get; set; }
        public Proveedor? Proveedor { get; set; }
        public ICollection<ItemCarrito> ItemsCarrito { get; set; } = new List<ItemCarrito>();
        public ICollection<ItemPedido> ItemsPedido { get; set; } = new List<ItemPedido>();

        public string ObtenerImagenUrl()
        {
            if (string.IsNullOrWhiteSpace(ImagenUrl))
            {
                return "/images/default-product.svg";
            }

            var url = ImagenUrl.Trim();

            // Detect and convert Google Drive share links
            if (url.Contains("drive.google.com", StringComparison.OrdinalIgnoreCase))
            {
                var match = Regex.Match(url, @"/d/([a-zA-Z0-9_-]+)", RegexOptions.None, TimeSpan.FromSeconds(1));
                if (match.Success)
                {
                    return $"https://lh3.googleusercontent.com/d/{match.Groups[1].Value}";
                }
                var idMatch = Regex.Match(url, @"[?&]id=([a-zA-Z0-9_-]+)", RegexOptions.None, TimeSpan.FromSeconds(1));
                if (idMatch.Success)
                {
                    return $"https://lh3.googleusercontent.com/d/{idMatch.Groups[1].Value}";
                }
            }

            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("//") ||
                url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            if (url.StartsWith("~/"))
            {
                return url.Substring(1);
            }

            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            return url;
        }
    }
}
