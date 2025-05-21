using AccountProfileServiceProvider.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountProfileServiceProvider.Contexts
{
    public class UserProfileContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<UserProfileEntity> UserProfiles { get; set; }
       
    }
}
