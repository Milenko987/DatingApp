using API.Entitites;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            var users = readJson();
            for (int i = 0; i < users.Count; i++)
            {
                modelBuilder.Entity<AppUser>().HasData(users[i]);
            }

        }



        public static List<AppUser> readJson()
        {
            var json = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Extensions/UserSeedData.json"));
            List<AppUser> users = JsonSerializer.Deserialize<List<AppUser>>(json, _options);
            using var hmac = new HMACSHA512();
            for (int i = 0; i < users.Count; i++)
            {
                users[i].PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                users[i].PasswordSalt = hmac.Key;

            }
            return users;
        }

        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
