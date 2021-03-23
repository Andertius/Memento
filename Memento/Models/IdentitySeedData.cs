using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Memento.Models
{
    public class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            MementoDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<MementoDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            UserManager<User> userManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<User>>();

            User user = await userManager.FindByNameAsync(adminUser);

            if (user is null)
            {
                user = new User()
                {
                    UserName = adminUser,
                    Email = "admin@example.com",
                };

                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}
