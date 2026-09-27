using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto.Manager;
using SchoolHub.Dto.School;
using SchoolHub.Dto;
using SchoolHub.Entity;
using SchoolHub.Interface;
using System.Reflection.Metadata.Ecma335;
using SchoolHub.Models;
using System.Security.Cryptography.Xml;

namespace SchoolHub.Service;

public class SchoolManagerService(MyContext db, IMapper mapper, IGeneralService generalService) : ISchoolManagerService
{
    public bool IsSchoolExist(int schoolId)
    {
        return db.Schools.Any(s => s.Id == schoolId);
    }

    public bool IsStudentInSchool(int classId, int studentUserId)
    {
        var schoolId = db.Classes.FirstOrDefault(c => c.Id == classId)?.SchoolId;
        if (schoolId == null)
            return true;

        var classIds = db.Classes.Where(x => x.SchoolId == schoolId).Select(x => x.Id).ToList();
        if (classIds == null || classIds.Count == 0)
            return true;

        return db.Students.Any(x => classIds.Contains(x.ClassId) && x.StudentUserId == studentUserId && x.IsDeleted == false);
    }

    public OperationResultDto AddStudent(StudentDto student, string nationalIdNumber)
    {
        if (!IsClassExist(student.ClassId))
            return new OperationResultDto { Success = false, Message = "کلاس وجود ندارد" };

        var user = IsUserExistByNationalId(nationalIdNumber);
        if (!user.Exist)
            return new OperationResultDto { Success = false, Message = "کاربری با این کد ملی وجود ندارد" };
        student.StudentUserId = user.Id;



        if (IsStudentInSchool(student.ClassId, student.StudentUserId))
            return new OperationResultDto { Success = false, Message = "دانش آموز قبلا در این مدرسه ثبت شده" };

        if (IsStudentInClass(student.ClassId, student.StudentUserId))
        {
            try
            {
                var studentThatRemoved = db.Students.FirstOrDefault(x => x.StudentUserId == student.StudentUserId && x.ClassId == student.ClassId && x.IsDeleted == true);
                studentThatRemoved.IsDeleted = false;
                db.SaveChanges();
                return new OperationResultDto { Success = true, Message = "دانش آموزز با موفقیت بازگردانده شد" };

            }
            catch
            {
                return new OperationResultDto { Success = false, Message = "دانش آموز قبلا تو این کلاس ثبت شده" };
            }

        }

        try
        {
            db.Students.Add(mapper.Map<StudentEntity>(student));
            db.SaveChanges();
            return new OperationResultDto { Success = true, Message = "دانش آموز با موفقیت ثبت شد" };
        }
        catch
        {
            return new OperationResultDto { Success = false, Message = "ثبت با خطا روبرو شد" };
        }
    }

    public bool IsClassExist(int classId) => db.Classes.Any(x => x.Id == classId);

    public (int Id, bool Exist) IsUserExistByNationalId(string nationalIdNumber)
    {
        var user = db.Users.FirstOrDefault(x => x.NationalIdNumber == nationalIdNumber);
        return (user?.Id ?? 0, user != null);
    }

    public bool IsStudentInClass(int classId, int studentUserId) => db.Students.Any(x => x.ClassId == classId && x.StudentUserId == studentUserId);

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

    public ManagerClassInfoDto GetManagerClassDetail(ManagerClassInfoDto managerClass)
    {
        var clas = db.Classes.FirstOrDefault(x => x.Id == managerClass.ClassId);
        managerClass.SchoolName = db.Schools.FirstOrDefault(x => x.Id == managerClass.SchoolId)?.Name ?? "نامشخص";
        managerClass.ClassName = clas?.Name ?? "نامشخص";
        managerClass.MajorName = db.GeneralItems.FirstOrDefault(x => x.Id == (clas != null ? clas.MajorGeneralId : 0))?.Title;
        managerClass.GradeName = db.GeneralItems.FirstOrDefault(x => x.Id == (clas != null ? clas.GradeGeneralId : 0))?.Title ?? "نامشخص";


        return managerClass;
    }

    public double GetClassAverage(int classId)
    {
        var studentIds = db.Students.Where(x => x.ClassId == classId).ToList().Select(x => x.Id) ?? [];
        var scores = db.Scores.Where(x => studentIds.Contains(x.StudentId) && x.Status == true).ToList();
        return Math.Round(scores?.Select(x => (double?)double.Parse(x.Score)).Average() ?? 0, 2);
    }

    public ClassDto GetClassDetail(int classId) => mapper.Map<ClassDto>(db.Classes.FirstOrDefault(x => x.Id == classId));

    public List<ManagerClassStudentDto> GetStudentsDetail(int classId)
    {
        var students = db.Students
            .Where(s => s.ClassId == classId && s.IsDeleted == false)
            .Select(s => new
            {
                StudentId = s.Id,

                FullName = db.Users
                    .Where(u => u.Id == s.StudentUserId)
                    .Select(u => u.Name + " " + u.LastName)
                    .FirstOrDefault() ?? "نامشخص",

                Gender = db.Users
                    .Where(u => u.Id == s.StudentUserId)
                    .Join(
                        db.GeneralItems,
                        u => u.GenderGeneralId,
                        g => g.Id,
                        (u, g) => g.Title
                    )
                    .FirstOrDefault(),
                NationalIdNumber = db.Users.Where(u => u.Id == s.StudentUserId).Select(u => u.NationalIdNumber).FirstOrDefault() ?? "0",
                StudentUserId = s.StudentUserId
            })
            .ToList();

        var studentIds = students
            .Select(x => x.StudentId)
            .ToList();

        var averages = db.Scores
            .Where(x => studentIds.Contains(x.StudentId) && x.Status == true)
            .AsEnumerable()
            .GroupBy(x => x.StudentId)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(
                    g.Select(x => double.Parse(x.Score))
                     .DefaultIfEmpty(0)
                     .Average(),
                    2
                )
            );

        return students
            .Select(s => new ManagerClassStudentDto
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Gender = s.Gender,
                NationalIdNumber = s.NationalIdNumber,
                Average = averages.TryGetValue(s.StudentId, out var average)
                    ? average
                    : 0,
                StudentUserId = s.StudentUserId
            })
            .ToList();
    }

    public List<ManagerClassTeacherDto> GetTeachersDetail(int classId)
    {
        var assignments = db.TeachingAssignments
            .Where(x => x.ClassId == classId)
            .ToList();

        var teacherIds = assignments
            .Select(x => x.TeacherUserId)
            .Distinct()
            .ToList();

        var teachers = db.Users
            .Where(x => teacherIds.Contains(x.Id))
            .Select(x => new
            {
                TeacherUserId = x.Id,
                FullName = x.Name + " " + x.LastName,
                NationalIdNumber = x.NationalIdNumber
            })
            .ToList();

        var subjects = db.TeachingAssignments
            .Where(x => teacherIds.Contains(x.TeacherUserId) && x.ClassId == classId)
            .Join(
                db.GeneralItems,
                x => x.SubjectId,
                g => g.Id,
                (x, g) => new
                {
                    x.TeacherUserId,
                    Subject = new GeneralItemDto
                    {
                        Id = g.Id,
                        Title = g.Title
                    }
                }
            )
            .ToList();

        return teachers
            .Select(teacher => new ManagerClassTeacherDto
            {
                TeacherUserId = teacher.TeacherUserId,
                FullName = teacher.FullName,
                NationalIdNumber = teacher.NationalIdNumber,
                Subjects = subjects
                    .Where(x => x.TeacherUserId == teacher.TeacherUserId)
                    .Select(x => x.Subject)
                    .DistinctBy(x => x.Id)
                    .ToList()
            })
            .ToList();
    }

    public OperationResultDto UpdateClass(ClassDto clas)
    {
        var isEnable = db.Classes.Any(x => x.Id == clas.Id);
        if (!isEnable)
            return new OperationResultDto { Success = false, Message = "کلاسی یافت نشد" };

        else
            try
            {
                db.Classes.Update(mapper.Map<ClassEntity>(clas));
                db.SaveChanges();
                return new OperationResultDto { Success = true, Message = "ویرایش موفقیت میز بود" };
            }
            catch
            {
                return new OperationResultDto { Success = false, Message = "ویرایش با شکست مواجه شد" };
            }


    }

    public int? GetGradeIdByClassId(int classId) => db.Classes.FirstOrDefault(x => x.Id == classId)?.GradeGeneralId;

    public bool IsTeacherInClass(int classId, int teacherUserId) => db.TeachingAssignments.Any(x => x.ClassId == classId && x.TeacherUserId == teacherUserId);

    public OperationResultDto AddTeacher(Dto.Manager.TeacherDto teacher)
    {

        if (!IsClassExist(teacher.ClassId))
            return new OperationResultDto { Success = false, Message = "کلاس وجود ندارد" };

        var user = IsUserExistById(teacher.TeacherUserId);
        if (!user)
            return new OperationResultDto { Success = false, Message = "کاربر وجود ندارد" };


        if (IsTeacherInClass(teacher.ClassId, teacher.TeacherUserId))
            return new OperationResultDto { Success = false, Message = "معلم قبلا داخل کلاس بوده" };

        if (!generalService.IsSubjectExistForThisGrade(teacher.SubjectIds, GetGradeIdByClassId(teacher.ClassId) ?? 0))
            return new OperationResultDto { Success = false, Message = "همچین درسی وجود ندارد" };

        try
        {
            foreach (var ts in teacher.SubjectIds)
                db.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = teacher.ClassId, SubjectId = ts, TeacherUserId = teacher.TeacherUserId });
            db.SaveChanges();
            return new OperationResultDto { Success = true, Message = "معلم با موفقیت اضافه شد" };
        }
        catch
        {

            return new OperationResultDto { Success = false, Message = "اضافه شدن معلم با خطا روبرو شد" };
        }
    }

    public bool DeleteStudent(int studentId)
    {
        try
        {
            var removedStudent = db.Students.FirstOrDefault(x => x.Id == studentId);
            removedStudent.IsDeleted = true;
            db.SaveChanges();
            return true;
        }
        catch
        {

            return false;
        }
    }

    public bool DeleteTeacher(int classId, int teacherUserId)
    {
        var removeItems = db.TeachingAssignments.Where(x => x.ClassId == classId && x.TeacherUserId == teacherUserId).ToList();
        try
        {
            if (removeItems.Count() <= 0)
                return false;
            db.TeachingAssignments.RemoveRange(removeItems);
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsUserExistById(int userId) => db.Users.Any(x => x.Id == userId);

    public OperationResultDto UpdateTeacher(Dto.Manager.TeacherDto teacher)
    {
        if (!IsClassExist(teacher.ClassId))
            return new OperationResultDto { Success = false, Message = "کلاس وجود ندارد" };

        var user = IsUserExistById(teacher.TeacherUserId);
        if (!user)
            return new OperationResultDto { Success = false, Message = "کاربر وجود ندارد" };

        if (!generalService.IsSubjectExistForThisGrade(teacher.SubjectIds, GetGradeIdByClassId(teacher.ClassId) ?? 0))
            return new OperationResultDto { Success = false, Message = "همچین درسی وجود ندارد" };

        try
        {
            var removedSubjects = db.TeachingAssignments.Where(x => x.ClassId == teacher.ClassId && x.TeacherUserId == teacher.TeacherUserId && !teacher.SubjectIds.Contains(x.SubjectId)).ToList();
            db.TeachingAssignments.RemoveRange(removedSubjects);
            db.SaveChanges();
        }
        catch
        {

            return new OperationResultDto { Success = false, Message = "حذف درس های کم شده با خطا روبرو شد" };
        }

        try
        {
            var duplicateSubjectIds = db.TeachingAssignments.Where(x => x.ClassId == teacher.ClassId && x.TeacherUserId == teacher.TeacherUserId && teacher.SubjectIds.Contains(x.SubjectId)).Select(x => x.SubjectId).ToList();
            teacher.SubjectIds.RemoveAll(x => duplicateSubjectIds.Contains(x));
            foreach (var ts in teacher.SubjectIds)
                db.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = teacher.ClassId, SubjectId = ts, TeacherUserId = teacher.TeacherUserId });
            db.SaveChanges();
            return new OperationResultDto { Success = true, Message = "درس ها با موفقیت اضافه شد" };
        }
        catch
        {

            return new OperationResultDto { Success = false, Message = "اضافه شدن درس ها با خطا روبرو شد" };
        }
    }

    public int GetClassIdByStudentAndSchoolId(int studentUserId, int schoolId)
    {
        return
            (
    from clas in db.Classes
    join student in db.Students
        on clas.Id equals student.ClassId
    where clas.SchoolId == schoolId && !student.IsDeleted
          && student.StudentUserId == studentUserId
    select clas.Id
).FirstOrDefault();
    }

    public int GetStudentIdByStudentAndClassId(int studentUserId, int classId) => db.Students.FirstOrDefault(x => x.ClassId == classId && x.StudentUserId == studentUserId && !x.IsDeleted)?.Id ?? 0;

    public double GetOverallAverage(int studentId)
    {

        var scores = db.Scores.Where(x => x.StudentId == studentId && x.Status).Select(x => double.Parse(x.Score)).ToList();
        return scores?.Any() == true ? Math.Round(scores.Average(), 2) : 0;
    }

    public List<ReportCardSubjectDto> GetSubjectsDetailForManager(int studentId, int classId)
    {
        var subjectIds = generalService.GetGradeSubJectIds(classId);
        var studentUserId = db.Students.FirstOrDefault(x => !x.IsDeleted && x.Id == studentId && x.ClassId == classId)?.Id ?? 0;
        var studentSubjectRecords = db.StudentSubjectRecords.Where(x => x.StudentUserId == studentUserId).ToList();
        var scores = db.Scores.Where(x => x.StudentId == studentId && x.Status).ToList();
        var teacherIds = db.TeachingAssignments.Where(x => x.ClassId == classId).ToList();
        var teachers = db.Users.Where(x => teacherIds.Select(x => x.TeacherUserId).Contains(x.Id)).ToList();

        var subjects = db.GeneralItems
            .Where(x => x.TitleType == "Subject" && subjectIds.Contains(x.Id))
            .ToList();

        return subjects.Select(x => new ReportCardSubjectDto
        {
            SubjectId = x.Id,
            SubjectName = x.Title,

            TeacherName = teachers
                .Where(t => t.Id == (
                    teacherIds.FirstOrDefault(i => i.SubjectId == x.Id)?.TeacherUserId ?? 0
                ))
                .Select(t => t.Name + " " + t.LastName)
                .FirstOrDefault() ?? "نامشخص",

            NegativeCount = studentSubjectRecords
                .Where(n => n.Type == "Negative" && n.SubjectGeneralId == x.Id)
                .Sum(n => n.Count),

            PositiveCount = studentSubjectRecords
                .Where(n => n.Type == "Positive" && n.SubjectGeneralId == x.Id)
                .Sum(n => n.Count),

            AverageScore = scores
                .Where(s => s.StudentId == studentId && s.GeneralSubjectId == x.Id)
                .Select(s => double.Parse(s.Score))
                .DefaultIfEmpty()
                .Average(),

            ScoreCount = scores
                .Count(s => s.StudentId == studentId && s.GeneralSubjectId == x.Id && s.Status),

            Scores = scores
                .Where(s => s.StudentId == studentId && s.GeneralSubjectId == x.Id && s.Status)
                .Select(s => new ReportCardScoreDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    Reason = s.Reason,
                    Score = double.Parse(s.Score)
                })
                .ToList()

        }).ToList();
    }

    public StudentReportCardDto GetClassDetailForManager(int studentId)
    {
        var student = db.Students.FirstOrDefault(x => x.Id == studentId && !x.IsDeleted) ?? new StudentEntity();

        var @class = db.Classes.FirstOrDefault(x => x.Id == student.ClassId) ?? new ClassEntity();
        var studentUser = db.Users.FirstOrDefault(x => x.Id == student.StudentUserId) ?? new UserEntity();
        var generals = db.GeneralItems.Where(x => (x.TitleType == "Gender" && x.Id == studentUser.GenderGeneralId) || (x.TitleType == "Grade" && x.Id == @class.GradeGeneralId) || (x.TitleType == "Major" && x.Id == @class.MajorGeneralId)).ToList();
        var schoolName = db.Schools.FirstOrDefault(x => x.Id == @class.SchoolId)?.Name ?? "نامشخص";
        return new StudentReportCardDto
        {
            Gender = generals.FirstOrDefault(x => x.Id == studentUser.GenderGeneralId)?.Title ?? "نامشخص",
            Grade = generals.FirstOrDefault(x => x.Id == @class.GradeGeneralId)?.Title ?? "نامشخص",
            Major = generals.FirstOrDefault(x => x.Id == @class.MajorGeneralId)?.Title,
            SchoolName = schoolName,
            StudentId = studentId,
            StudentName = (studentUser?.Name ?? "") + " " + (studentUser?.LastName ?? ""),
        };
    }

    public string GetNationalId(int userId) => db.Users.FirstOrDefault(x => x.Id == userId)?.NationalIdNumber ?? "";

    public UserEntity GetUserByNationalId(string nationalId) => db.Users.FirstOrDefault(x => x.NationalIdNumber == nationalId) ?? new UserEntity();


    public ValidationDto UpdateSchool(SchoolDto schoolDto)
    {
        var school = db.Schools.FirstOrDefault(x => x.Id == schoolDto.Id);

        if (school == null)
            return new() { IsCorrect = false, Message = "مدرسه وجود ندارد" };

        var generals = db.GeneralItems.Where(x => x.Active &&
            (
                (x.Id == schoolDto.TypeGeneralId && x.TitleType == "Type") ||
                (x.Id == schoolDto.CityId && x.TitleType == "City") ||
                (x.Id == schoolDto.ShiftGeneralId && x.TitleType == "Shift") ||
                (x.Id == schoolDto.EducationPeriodGeneralId && x.TitleType == "EducationPeriod") ||
                (x.Id == schoolDto.DistrictId && x.TitleType == "District") ||
                (x.Id == schoolDto.EducationLevelGeneralId && x.TitleType == "EducationLevel") ||
                (x.Id == schoolDto.GenderGeneralId && x.TitleType == "AdmissionGender")
            )).ToList();
        var generalCount = schoolDto.DistrictId == 0 ? generals.Count() + 1 : generals.Count();

        if (generalCount != 7)
            return new() { IsCorrect = false, Message = "یکی از داده‌های انتخاب‌شده معتبر نیست." };

        if (!generals.Any(x => x.TitleType == "District" && x.ParentId == schoolDto.CityId) && schoolDto.DistrictId !=0)
            return new() { IsCorrect = false, Message = "منطقه مورد نظر مربوط به شهر مورد نظر نیست." };

        if (!IsUserExistById(schoolDto.ManagerUserId))
            return new() { IsCorrect = false, Message = "مدیر وجود ندارد." };

        school.Name = schoolDto.Name;
        school.ManagerUserId = schoolDto.ManagerUserId;
        school.CityId = schoolDto.CityId;
        school.DistrictId = schoolDto.DistrictId != 0 ? schoolDto.DistrictId :  null;
        school.ShiftGeneralId = schoolDto.ShiftGeneralId;
        school.TypeGeneralId = schoolDto.TypeGeneralId;
        school.GenderGeneralId = schoolDto.GenderGeneralId;
        school.EducationLevelGeneralId = schoolDto.EducationLevelGeneralId;
        school.EducationPeriodGeneralId = schoolDto.EducationPeriodGeneralId;

        db.SaveChanges();

        return new() { IsCorrect = true, Message = "به‌روزرسانی با موفقیت انجام شد." };
    }


    public List<TeacherListItemViewModel> GetTeachersBySchoolId(int schoolId)
    {
        var teachersUserId = db.Teachers.Where(t => t.Active && t.SchoolId == schoolId).Select(x => x.TeacherUserId).ToList();
        return db.Users.Where(x => teachersUserId.Contains(x.Id)).Select(x => new TeacherListItemViewModel
        {
            FullName= x.Name + " " + x.LastName,
            NationalId = x.NationalIdNumber,
            UserId=x.Id
        }).ToList();
         
    }

    public SchoolDto GetSchoolBySchoolId(int schoolId)=> mapper.Map<SchoolDto>(db.Schools.FirstOrDefault(x => x.Id == schoolId) ??new SchoolEntity());

    public ValidationDto AddTeacherToSchool(int schoolId,string teacherNationalId)
    {
        if (!IsSchoolExist(schoolId))
            return new ValidationDto { IsCorrect = false, Message = "مدرسه وجود ندارد" };
        var user = GetUserByNationalId(teacherNationalId);
        if (user == new UserEntity())
        return new ValidationDto { IsCorrect = false, Message = "کاربر وجود ندارد" };

        if (db.Teachers.Any(x => x.TeacherUserId == user.Id))
            return new ValidationDto { IsCorrect = false, Message = "کاربر قبلا در این مدرسه به عنوان معلم ثبت " };

        db.Teachers.Add(new TeacherEntity { SchoolId = schoolId, Active = true, TeacherUserId = user.Id });
        db.SaveChanges();
        return new ValidationDto { IsCorrect = true, Message = "معلم با موفقیت ثبت شد" };
    }

    public List<int> GetTeacherUserIds(int schoolId) => db.Teachers.Where(x => x.SchoolId == schoolId && x.Active).Select(x => x.TeacherUserId).ToList();

    public List<UserDto> GetAvailableTeachersById(List<int> teacherUserIds,int classId)
    {
        var classTeacherUserIds = db.TeachingAssignments.Where(x => x.ClassId == classId).Select(x=> x.TeacherUserId).ToList();
        var teachers = db.Users.Where(x => teacherUserIds.Contains(x.Id) && !classTeacherUserIds.Contains(x.Id)).ToList();
        return mapper.Map<List<UserDto>>(teachers);
    }

}
