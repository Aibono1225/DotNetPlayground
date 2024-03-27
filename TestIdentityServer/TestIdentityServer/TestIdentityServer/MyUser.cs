using Microsoft.AspNetCore.Identity;

namespace TestIdentityServer
{
    public class MyUser: IdentityUser<long>
    {
        public string? WeChat { get; set; }
    }
}
