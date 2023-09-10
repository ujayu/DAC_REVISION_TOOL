using apidotnet.DTO;
using AutoMapper;
using RevisionTool.Entity;

namespace dotnetapi.Helper
{
    public class UserUserInfoAutoMapperHandler : Profile
    {
        public UserUserInfoAutoMapperHandler()
        {
            CreateMap<User, UserInfo>().ReverseMap();
        }
    }
}
