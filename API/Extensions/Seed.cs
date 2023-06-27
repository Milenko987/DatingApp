using API.Data;
using API.Entitites;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Extensions
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext dataContext)
        {
            if (await dataContext.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "Extensions/UserSeedData.json"));
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<AppUser> users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hmac.Key;

                dataContext.Users.Add(user);


            }
            await dataContext.SaveChangesAsync();
        }
    }
}
