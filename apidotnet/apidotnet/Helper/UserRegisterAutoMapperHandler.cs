using apidotnet.DTO;
using AutoMapper;
using RevisionTool.Entity;

namespace dotnetapi.Helper
{
    public class UserRegisterAutoMapperHandler : Profile
    {
        public UserRegisterAutoMapperHandler()
        {
            CreateMap<User, Register>().ReverseMap();
        }
    }
}
