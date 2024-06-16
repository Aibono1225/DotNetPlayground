using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TestIdentityServer
{
    public class MyDbContext: IdentityDbContext<MyUser, MyRole, long>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
            
        }
    }
}
