using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;
using task15_11fronttoback.Utilities.Extensions;
using task15_11fronttoback.Utilities.Enums;

namespace task15_11fronttoback.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Register(RegisterVM uservm)
        {
            if (!ModelState.IsValid) return View();

            string emailPattern = @"^(\w+([.]\w+)*@\w+([.]\w+)*\.\w+([.]\w+)*)$";
            if (!Regex.IsMatch(uservm.Email, emailPattern))
            {
                ModelState.AddModelError("Email", "Email duzgun formatda deyil");
                return View();
            }
            if (uservm.Name is not null)
            {
                uservm.Name = uservm.Name.Capitalize();
            }
            if (uservm.Surname is not null)
            {
                uservm.Surname = uservm.Name.Capitalize();
            }

            if (!Enum.IsDefined(typeof(Gender), uservm.Genders))
            {
                ModelState.AddModelError("Genders", "Invalid gender");
                return View();
            }
            AppUser user = new AppUser
            {
                Name = uservm.Name,
                Surname = uservm.Surname,
                Email = uservm.Email,
                UserName = uservm.UserName,
                gender = uservm.Genders

            };
            IdentityResult result = await _userManager.CreateAsync(user, uservm.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginvm, string? returnurl)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(loginvm.UsernameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(loginvm.UsernameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError(String.Empty, "Username,Email or Password is incorrect");
                    return View();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginvm.Password, loginvm.IsRemembered, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "So many incorrect tries, please try later");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username,Email or Password is incorrect");
                return View();
            }

            Response.Cookies.Delete("Basket");

            if (returnurl is null) return RedirectToAction("Index", "Home");


            return Redirect(returnurl);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (!(await _roleManager.RoleExistsAsync(role.ToString())))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString()
                    });

                }

            }
            return RedirectToAction("Index", "Home");
        }
    }
}
