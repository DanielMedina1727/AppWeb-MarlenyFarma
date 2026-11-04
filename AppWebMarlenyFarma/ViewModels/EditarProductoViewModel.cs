
using System.ComponentModel.DataAnnotations;

namespace AppWebMarlenyFarma.ViewModels
{
    public class EditarProductoViewModel : ProductoFormViewModel
    {
        [Required(ErrorMessage = "El identificador es obligatorio.")]
        public int? ProductoId { get; set; }
    }
}
