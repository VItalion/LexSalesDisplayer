using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TableDisplayer.Data;
using TableDisplayer.Models;

namespace TableDisplayer.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHubContext<TableHub> _hubContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IHubContext<TableHub> hubContext) {
            _userManager = userManager;
            _signInManager = signInManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpGet]
        [Authorize(Roles = RoleInitializer.ADMIN_ROLE)]
        public IActionResult Register(string returnUrl = null) {
            return PartialView(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleInitializer.ADMIN_ROLE)]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                var user = new User {
                    UserName = model.UserName,
                    Name = model.Name,
                    LexId = model.LexId,
                    Ext = model.Ext,
                    IsSuspended = model.IsSuspended
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    await _userManager.AddToRoleAsync(user, RoleInitializer.USER_ROLE);
                    return RedirectToAction("Users", "Account");
                } else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = RoleInitializer.ADMIN_ROLE)]
        public async Task<IActionResult> ResetPassword(string username, string password) {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) { return NotFound(); }

            try {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, password);
                if (result.Succeeded) {
                    return Ok();
                } else {
                    return BadRequest();
                }
            } catch (Exception ex) {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null || user.IsSuspended) { return RedirectToAction("AccessDenied", "Account"); ; }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded) {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) {
                        return Redirect(model.ReturnUrl);
                    } else {
                        return RedirectToAction("GetLex", "Sales");
                    }
                } else {
                    ModelState.AddModelError("", "Incorrect login/password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = RoleInitializer.ADMIN_ROLE)]
        public async Task<IActionResult> Users() {
            var users = await _userManager.GetUsersInRoleAsync(RoleInitializer.USER_ROLE);
            var dtos = new List<UserViewModel>(users.Count);
            foreach (var user in users) {
                var dto = new UserViewModel {
                    Login = user.UserName,
                    Name = user.Name,
                    Ext = user.Ext,
                    LexId = user.LexId,
                    IsSuspended = user.IsSuspended
                };
                dtos.Add(dto);
            }

            return View(dtos);
        }

        [HttpGet]
        [Authorize(Roles = RoleInitializer.ADMIN_ROLE)]
        public async Task<IActionResult> Edit(string username) {
            if (string.IsNullOrWhiteSpace(username)) { return BadRequest(); }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) { return NotFound(); }

            var vm = new UserViewModel {
                Ext = user.Ext,
                IsSuspended = user.IsSuspended,
                LexId = user.LexId,
                Login = user.UserName,
                Name = user.Name
            };

            return PartialView("EditUserView", vm);
        }

        [HttpPost]
        [Authorize(Roles = RoleInitializer.ADMIN_ROLE)]
        public async Task<IActionResult> Edit(UserViewModel user) {
            if (user == null) { return BadRequest(); }

            var model = await _userManager.FindByNameAsync(user.Login);
            if (model == null) { return NotFound(); }

            model.Name = user.Name;
            model.Ext = user.Ext;
            model.LexId = user.LexId;
            model.IsSuspended = user.IsSuspended;
            await _userManager.UpdateAsync(model);

            if (user.IsSuspended && TableHub.ActiveUsers.TryGetValue(user.Login, out var connectionId)) {
                var hubClient = _hubContext.Clients.Client(TableHub.ActiveUsers[user.Login]);
                if (hubClient != null) {
                    await hubClient.SendAsync("Suspend");
                }
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [Authorize(Roles = RoleInitializer.ADMIN_ROLE)]
        public async Task<IActionResult> Remove(string username) {
            if (string.IsNullOrWhiteSpace(username)) { return BadRequest(); }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) { return NotFound(); }

            user.IsSuspended = true;
            await _userManager.UpdateAsync(user);

            if (user.IsSuspended && TableHub.ActiveUsers.TryGetValue(username, out var connectionId))
            {
                var hubClient = _hubContext.Clients.Client(TableHub.ActiveUsers[username]);
                if (hubClient != null)
                {
                    await hubClient.SendAsync("Suspend");
                }
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AccessDenied() {
            return View();
        }
    }
}
