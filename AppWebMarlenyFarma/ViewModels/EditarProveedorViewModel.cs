using System.ComponentModel.DataAnnotations;

namespace AppWebMarlenyFarma.ViewModels
{
    public class EditarProveedorViewModel : ProveedorFormViewModel
    {
        [Required(ErrorMessage = "El identificador es obligatorio.")]
        public int? ProveedorId { get; set; }
    }
}

