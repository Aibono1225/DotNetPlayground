using ArchPractice.IService;
using ArchPractice.Model;
using ArchPractice.Repository;

namespace ArchPractice.Service
{
    public class UserService : IUserService
    {
        public async Task<List<UserVo>> Query()
        {
            var userRepo = new UserRepository();
            var users = await userRepo.Query();
            return users.Select(d => new UserVo() { UserName = d.Name }).ToList();
        }
    }
}
