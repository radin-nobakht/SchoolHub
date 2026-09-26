using AutoMapper;
using SchoolHub.Dto;
using SchoolHub.Dto.School;
using SchoolHub.Entity;

namespace SchoolHub.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, UserEntity>().ReverseMap();
            CreateMap<SchoolDto, SchoolEntity>().ReverseMap();
            CreateMap<ClassDto, ClassEntity>().ReverseMap();
            CreateMap<GeneralItemDto, GeneralItemEntity>().ReverseMap();
            CreateMap<ScoreDto, ScoreEntity>().ReverseMap();
            CreateMap<StudentDto, StudentEntity>().ReverseMap();
            CreateMap<SchoolHub.Dto.TeacherDto, TeacherEntity>().ReverseMap();
        }
    }
}
