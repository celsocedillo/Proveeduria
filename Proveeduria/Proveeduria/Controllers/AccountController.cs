using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NLog;
using Proveduria.Models;
using Proveduria.Repositories;
using Proveduria.Utils;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Proveduria.Controllers
{
    

    public class AccountController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;
        #region methods

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                if (AuthUtil.IsValidAdUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        UsuarioController usuarioController = new UsuarioController();
                        ViewBag.usuario = UsuarioController.usuario.usuario;
                        ViewBag.nombre = UsuarioController.usuario.nombre;
                        VW_EMPLEADO empleado = unitOfWork.EmpleadoRepository.GetAll().Where(p => p.USUARIO == model.UserName.ToUpper()).FirstOrDefault();
                        if (empleado != null)
                        {
                            Session["usuario"] = empleado.USUARIO;
                            Session["nombre"] = empleado.EMPLEADO;
                            Session["id_direccion"] = empleado.DIRECCION_ID;
                            Session["direccion"] = empleado.DIRECCION;
                            Session["usuario_jefe"] = empleado.USUARIO_JEFE_DEPARTAMENTO;
                            Session["bodega_id"] = 1;
                            Session["bodega"] = "BODEGA PROVEDURIA";
                        }

                        //try
                        //{
                        //    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                        //    ApplicationUserManager _userManager = new ApplicationUserManager(store);
                        //    var manger = _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        //    var user = new ApplicationUser() { Email = model.UserName, UserName = model.UserName };
                        //    var result = manger.Create(user, model.Password);
                        //}
                        //catch (System.Exception ex)
                        //{
                        //    logger.Error(ex, ex.Message);
                        //}
 

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El nombre de usuario o la contraseña provistos son incorrectos.");
                }
                //if (Membership.ValidateUser(model.UserName, model.Password))
                //{
                //    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                //    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                //    {
                //        return Redirect(returnUrl);
                //    }
                //    else
                //    {
                //        return RedirectToAction("Index", "Home");
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                //}
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {

        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
        //            // Enviar correo electrónico con este vínculo
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FirstName,LastName")] AspNetUsers aspNetUsers, string password)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var store = new UserStore<ApplicationUser>(new ApplicationDbContext());

        //        ApplicationUserManager _userManager = new ApplicationUserManager(store);

        //        var manger = _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        //        var user = new ApplicationUser() { Email = aspNetUsers.Email, UserName = aspNetUsers.Email };

        //        var result = manger.Create(user, password);


        //        return RedirectToAction("Index");
        //    }

        //    return View(aspNetUsers);
        //}

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}