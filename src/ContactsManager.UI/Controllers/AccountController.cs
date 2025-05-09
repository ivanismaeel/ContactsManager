using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUD.NET.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    public class AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager) : Controller
    {
        [HttpGet]
        [Authorize("NotAuthorized")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthorized")]
        // [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registerDTO.Email,
                    Email = registerDTO.Email,
                    PhoneNumber = registerDTO.Phone,
                    PersonName = registerDTO.PersonName
                };

                var result = await userManager.CreateAsync(user, registerDTO.Password);

                if (result.Succeeded)
                {
                    if (registerDTO.UserType == UserTypeOptions.Admin)
                    {
                        if (await roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                        {
                            var applicationRole = new ApplicationRole
                            {
                                Name = UserTypeOptions.Admin.ToString()
                            };
                            await roleManager.CreateAsync(applicationRole);
                        }

                        await userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                    }
                    else
                    {

                        if (await roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                        {
                            var applicationRole = new ApplicationRole
                            {
                                Name = UserTypeOptions.User.ToString()
                            };
                            await roleManager.CreateAsync(applicationRole);
                        }
                        await userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
                    }

                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction(nameof(PersonsController.Index), "Persons");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return View(registerDTO);
            }

            ViewBag.Error = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).FirstOrDefault();

            return View(registerDTO);
        }

        [HttpGet]
        [Authorize("NotAuthorized")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthorized")]
        public async Task<ActionResult> Login(LoginDTO loginDTO, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginDTO.Email,
                    loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(loginDTO.Email);

                    if (user != null)
                    {
                        if (await userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                        {
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                    }
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    return RedirectToAction(nameof(PersonsController.Index), "Persons");
                }

                ModelState.AddModelError("Login", "Invalid email or password.");
                return View(loginDTO);
            }

            ViewBag.Error = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).FirstOrDefault();

            return View(loginDTO);
        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true); // Email is available
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }
    }
}
