using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Identity;

namespace DataBrowser.DB.EFCore.Context
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IRepository<Hub> hubRepository)
        {
            //Seed Roles
            var haveRole = await roleManager.FindByNameAsync(UserAndGroup.Roles.Administrator.ToString());
            if (haveRole == null)
                await roleManager.CreateAsync(new ApplicationRole(UserAndGroup.Roles.Administrator.ToString()));
            haveRole = await roleManager.FindByNameAsync(UserAndGroup.Roles.User.ToString());
            if (haveRole == null)
                await roleManager.CreateAsync(new ApplicationRole(UserAndGroup.Roles.User.ToString()));


            await createSuperAdminIfNeedAsync(userManager);

            var hubs = await hubRepository.ListAllAsync();
            if (hubs == null || hubs.Count <= 0)
            {
                var hubDto = new HubDto
                {
                    LogoURL = "",
                    BackgroundMediaURL = "",
                    SupportedLanguages = new List<string> { "en", "it" },
                    DefaultLanguage = "en",
                    MaxObservationsAfterCriteria = 500000,
                    DecimalSeparator = ",",
                    DecimalNumber = 0,
                    EmptyCellDefaultValue = "",
                    DefaultView = "",
                    MaxCells = 1000000,
                    Extras = "{\"pageSize\":3}"
                };
                var hub = Hub.CreateHub(hubDto);
                hubRepository.Add(hub);

                await hubRepository.UnitOfWork.SaveChangesAsync(dispatchDomainEvent: false);
            }
        }

        private static async Task createSuperAdminIfNeedAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = UserAndGroup.SuperAdminUsername,
                Email = UserAndGroup.SuperAdminEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsSuperAdmin = true
            };

            var needToCreateSuperAdminUser = false;

            var adminUsers = await userManager.GetUsersInRoleAsync(UserAndGroup.Roles.Administrator.ToString());
            if (adminUsers == null ||
                !adminUsers.Any())
            {
                needToCreateSuperAdminUser = true;
            }

            var defaultUserFromDb = userManager.Users.FirstOrDefault(u => u.UserName != defaultUser.UserName);
            if (defaultUserFromDb == null)
            {
                await userManager.CreateAsync(defaultUser, UserAndGroup.SuperAdminPassword);
                await userManager.AddToRoleAsync(defaultUser, UserAndGroup.Roles.Administrator.ToString());
            }
            else if (defaultUserFromDb.IsDisable)
            {
                defaultUserFromDb.IsDisable = false;
                await userManager.UpdateAsync(defaultUserFromDb);
                var token = await userManager.GeneratePasswordResetTokenAsync(defaultUserFromDb);
                await userManager.ResetPasswordAsync(defaultUserFromDb, token, UserAndGroup.SuperAdminPassword);
            }


        }
    }
}