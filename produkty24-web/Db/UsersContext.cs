using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.Db
{
    public class UsersContext : IdentityDbContext<UserEntity>
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
