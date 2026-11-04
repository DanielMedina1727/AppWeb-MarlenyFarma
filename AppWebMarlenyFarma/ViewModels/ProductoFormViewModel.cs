
using System.ComponentModel.DataAnnotations;

namespace AppWebMarlenyFarma.ViewModels
{
    public class ProductoFormViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue)]
        public decimal? Precio { get; set; }

        [Required(ErrorMessage = "El precio original es obligatorio.")]
        [Range(0.01, double.MaxValue)]
        public decimal? PrecioOriginal { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "Debe indicar si es destacado.")]
        public bool? EsDestacado { get; set; }

        [Required(ErrorMessage = "Debe indicar si requiere receta.")]
        public bool? RequiereReceta { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [Range(1, int.MaxValue)]
        public int? CategoriaId { get; set; }

        public int? ProveedorId { get; set; }
    }
}
