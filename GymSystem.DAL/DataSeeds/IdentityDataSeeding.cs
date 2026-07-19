using GymSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.DataSeeds
{
    public static class IdentityDataSeeding
    {
        public async static Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
        ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool HasUsers = userManager.Users.Any();
                bool HasRoles = roleManager.Roles.Any();

                if (HasRoles && HasUsers) return;

                if (!HasRoles)
                {
                    var roles = new List<IdentityRole>
                    {
                        new IdentityRole { Name = "SuperAdmin" },
                        new IdentityRole { Name = "Admin" }
                    };
                    foreach (var roleName in roles.Select(R => R.Name))
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            var roleresult = await roleManager.CreateAsync(new IdentityRole(roleName));
                            if (!roleresult.Succeeded)
                            {
                                logger.LogError($"Failed to Seed role");
                                return;
                            }
                        }

                    }
                }
                if (!HasUsers)
                {
                    var MainUser = new ApplicationUser
                    {
                        FirstName = "Mahmoud",
                        LastName = "Sami",
                        UserName = "MahmoudSami122",
                        Email = "Mahmoud@gmail.com",
                        PhoneNumber = "01110062000",

                    };
                    var UserResult = await userManager.CreateAsync(MainUser, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainUser, "SuperAdmin");
                    if (!UserResult.Succeeded)
                    {
                        logger.LogError($"Failed to Seed user");
                        return;
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while seeding identity data");
                throw;
            }
        }
    }
}
