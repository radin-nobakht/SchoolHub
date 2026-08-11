using SchoolHub.Dto;
using SchoolHub.Dto.School;

namespace SchoolHub.Models;

public class ClassInfoViewModel
{
 public ClassDto Class { get; set; } = new ClassDto();
 public List<StudentDto> Students { get; set; } = new List<StudentDto>();
 public List<TeacherDto> Teachers { get; set; } = new List<TeacherDto>();
}
