using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserCRUD.DbContext
{
    public class UserCRUDDbContext : IdentityDbContext<IdentityUser>
    {
        public UserCRUDDbContext(DbContextOptions<UserCRUDDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
