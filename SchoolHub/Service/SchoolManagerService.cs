using AutoMapper;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Interface;

namespace SchoolHub.Service;

public class SchoolManagerService(MyContext db, IMapper mapper) : ISchoolManagerService
{
    public bool IsManagerOfSchool(int managerUserId, int schoolId)
    {
        return db.Schools.Any(x => x.Id == schoolId && x.ManagerUserId == managerUserId);
    }

    public List<ClassDto> GetAllClasses(int schoolId)
    {
        return (
            from classes in db.Classes

            where classes.SchoolId == schoolId

            join major in db.GeneralItems.Where(x => x.TitleType == "Major")
                on classes.MajorGeneralId equals major.Id
                into majorGroup
            from major in majorGroup.DefaultIfEmpty()

            join grade in db.GeneralItems.Where(x => x.TitleType == "Grade")
                on classes.GradeGeneralId equals grade.Id

            select new ClassDto
            {
                Id = classes.Id,

                Name = classes.Name,

                Major = major != null ? major.Title : null,

                Grade = grade.Title,
            }
        ).ToList();
    }

    public bool AddClass(ClassDto classDto)
    {
        if (string.IsNullOrWhiteSpace(classDto?.Name))
            return false;

        if (classDto.GradeGeneralId == 0 || classDto.SchoolId == 0)
        {
            return false;
        }

        var isDuplicate = db.Classes.Any(x =>
            x.GradeGeneralId == classDto.GradeGeneralId
            && x.Name == classDto.Name
            && x.SchoolId == classDto.SchoolId
            && x.MajorGeneralId == classDto.MajorGeneralId
        );

        if (isDuplicate)
            return false;

        db.Classes.Add(mapper.Map<ClassEntity>(classDto));

        db.SaveChanges();

        return true;
    }

    public bool DeleteClass(int classId)
    {
        var entity = db.Classes.FirstOrDefault(x => x.Id == classId);

        try
        {
            db.Classes.Remove(entity);
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsManagerOfSchoolClass(int schoolId, int managerUserId) =>
        db.Schools.Any(x => x.Id == schoolId && x.ManagerUserId == managerUserId);

    public int? GetSchoolIdByClassId(int classId) =>
        db.Classes.FirstOrDefault(x => x.Id == classId)?.SchoolId;
}
