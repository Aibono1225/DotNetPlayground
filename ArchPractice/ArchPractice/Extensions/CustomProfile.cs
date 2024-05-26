using ArchPractice.Model;
using AutoMapper;

namespace ArchPractice.Extensions
{
    public class CustomProfile: Profile
    {
        /// <summary>
        /// Constructor used to create relationship mappings
        /// </summary>
        public CustomProfile()
        {
            CreateMap<Role, RoleVo>()
                .ForMember(a => a.RoleName, o => o.MapFrom(d => d.Name));
            CreateMap<RoleVo, Role>()
                .ForMember(a => a.Name, o => o.MapFrom(d => d.RoleName));
        }
    }
}
