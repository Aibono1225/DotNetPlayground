using ArchPractice.Model;

namespace ArchPractice.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> Query();
    }
}
