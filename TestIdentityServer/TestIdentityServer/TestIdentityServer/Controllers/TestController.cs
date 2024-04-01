using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace TestIdentityServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly RoleManager<MyRole> _roleManager;
        private readonly UserManager<MyUser> _userManager;

        public TestController(RoleManager<MyRole> roleManager, UserManager<MyUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;

        }

        [HttpPost]
        public async Task<ActionResult> CreateUserRole()
        {
            bool roolExists = await _roleManager.RoleExistsAsync("admin");
            if (!roolExists)
            {
                MyRole role = new MyRole
                {
                    Name = "admin",
                };

                var res = await _roleManager.CreateAsync(role);
                if(!res.Succeeded)
                {
                    return BadRequest(res.Errors);
                }
            }

            MyUser? user = await _userManager.FindByNameAsync("chloe");
            if (user == null)
            {
                user = new MyUser
                {
                    UserName = "chloe",
                    Email = "myemailaddress@gmail.com",
                    EmailConfirmed = true
                };

                var res = await _userManager.CreateAsync(user, "123456");
                if (!res.Succeeded)
                {
                    return BadRequest(res.Errors);
                }

                res = await _userManager.AddToRoleAsync(user, "admin");
                if (!res.Succeeded)
                {
                    return BadRequest(res.Errors);
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginRequest req)
        {
            string userName = req.UserName;
            string password = req.Password;

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound($"用户名不存在：{userName}");
            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest($"用户已被锁定，锁定结束时间：{user.LockoutEnd}");
            }

            var success = await _userManager.CheckPasswordAsync(user, password);
            if (success)
            {
                var token = await _userManager.CreateSecurityTokenAsync(user);
                //return Ok("登陆成功");
                await _userManager.ResetAccessFailedCountAsync(user);
                return Ok(token);
            }
            else
            {
                await _userManager.AccessFailedAsync(user);
                return BadRequest("用户名或者密码错误");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendResetRequest(SendResetPasswordTokenRequest req)
        {
            string email = req.Email;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"邮箱不存在：{email}");
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok($"向邮箱{email}发送验证码:{token}");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
        {
            var user = await _userManager.FindByNameAsync(req.userName);
            if (user == null)
            {
                return NotFound($"用户名不存在：{req.userName}");
            }

            var result = await _userManager.ResetPasswordAsync(user, req.token, req.newPassword);
            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                return Ok("密码重置成功");
            }
            else
            {
                await _userManager.AccessFailedAsync(user);
                return BadRequest("密码重置失败");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"用户不存在");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("删除角色成功");
            }
            else
            {
                return BadRequest("删除角色失败");
            }
        }
    }
}
