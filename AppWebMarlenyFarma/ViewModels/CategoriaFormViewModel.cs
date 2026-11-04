
using System.ComponentModel.DataAnnotations;

namespace AppWebMarlenyFarma.ViewModels
{
    public class CategoriaFormViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        public string? ImagenUrl { get; set; }
    }
}
