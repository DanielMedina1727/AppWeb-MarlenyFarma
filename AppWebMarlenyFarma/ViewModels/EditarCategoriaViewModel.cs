
using System.ComponentModel.DataAnnotations;

namespace AppWebMarlenyFarma.ViewModels
{
    public class EditarCategoriaViewModel : CategoriaFormViewModel
    {
        [Required(ErrorMessage = "El identificador es obligatorio.")]
        public int? CategoriaId { get; set; }
    }
}
