using ArchPractice.Model;

namespace ArchPractice.IService
{
    public interface IUserService
    {
        Task<List<UserVo>> Query();
    }
}
