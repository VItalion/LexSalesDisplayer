using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Data {
    public class RoleInitializer {
        public const string ADMIN_ROLE = "admin";
        public const string USER_ROLE = "employee";
        public const string UPLOADER_ROLE = "uploader";


        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, string adminLogin, string adminPwd) {
            var uploaderName = "uploader";
            var adminUserName = string.IsNullOrWhiteSpace(adminLogin) ? "admin" : adminLogin;
            var password = string.IsNullOrWhiteSpace(adminPwd) ? "ThisIsMyPass264" : adminPwd;
            if (await roleManager.FindByNameAsync(ADMIN_ROLE) == null) {
                await roleManager.CreateAsync(new IdentityRole(ADMIN_ROLE));
            }
            if (await roleManager.FindByNameAsync(USER_ROLE) == null) {
                await roleManager.CreateAsync(new IdentityRole(USER_ROLE));
            }
            if (await roleManager.FindByNameAsync(UPLOADER_ROLE) == null) {
                await roleManager.CreateAsync(new IdentityRole(UPLOADER_ROLE));
            }
            if (await userManager.FindByNameAsync(adminUserName) == null) {
                var admin = new User { Id = Guid.NewGuid().ToString(), UserName = adminUserName };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(admin, ADMIN_ROLE);
                }
            }
            if (await userManager.FindByNameAsync(uploaderName) == null) {
                var uploader = new User { Id = Guid.NewGuid().ToString(), UserName = uploaderName };
                var result = await userManager.CreateAsync(uploader, password);
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(uploader, UPLOADER_ROLE);
                }
            }
        }
    }
}
