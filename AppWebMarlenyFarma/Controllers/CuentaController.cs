using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppWebMarlenyFarma.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CuentaController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = new IdentityUser { UserName = model.Email, Email = model.Email };
                var resultado = await _userManager.CreateAsync(usuario, model.Contrasena);

                if (resultado.Succeeded)
                {
                    await _userManager.AddToRoleAsync(usuario, "Cliente");
                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(model.Email, model.Contrasena, model.RecuerdaMe, lockoutOnFailure: false);
                if (resultado.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && await _userManager.IsInRoleAsync(user, "Administrador"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    var productoPendienteId = HttpContext.Session.GetInt32("ProductoPendienteId");
                    var cantidadPendiente = HttpContext.Session.GetInt32("ProductoPendienteCantidad");
                    if (productoPendienteId.HasValue && cantidadPendiente.HasValue)
                    {
                        HttpContext.Session.Remove("ProductoPendienteId");
                        HttpContext.Session.Remove("ProductoPendienteCantidad");
                        return RedirectToAction("AgregarProducto", "Carrito", new { productoId = productoPendienteId.Value, cantidad = cantidadPendiente.Value });
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Correo o contraseña inválidos");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Perfil()
        {
            return View();
        }
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public bool RecuerdaMe { get; set; }
    }

    public class RegistroViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}

