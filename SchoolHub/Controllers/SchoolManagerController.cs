using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.Manager;
using SchoolHub.Dto.School;
using SchoolHub.Dto.Student;
using SchoolHub.Interface;
using System.Security.Claims;

namespace SchoolHub.Controllers
{
    [Authorize]
    public class SchoolManagerController(
        IGeneralService generalService,
        ISchoolManagerService schoolManagerService,
        ISchoolStudentService studentService
    ) : Controller
    {
        public async Task<IActionResult> SchoolInfo(int schoolId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isManager = schoolManagerService.IsManagerOfSchool(
                managerUserId: userId,
                schoolId: schoolId
            );
            if (!isManager)
            {
                TempData["Eror"] = "شما اجازه دسترسی به این صفحه را ندارید.";
                return RedirectToAction("SchoolPage", "School");
            }
            else
            {
                var schoolData = new AllDatailAboutSchool();

                schoolData.School = await generalService.GetFullDataOfSchoolById(schoolId);
                schoolData.Classes = schoolManagerService.GetAllClasses(schoolId);
                return View(schoolData);
            }
        }

        [HttpPost]
        public IActionResult AddClass(ClassDto classDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var schoolId = schoolManagerService.GetSchoolIdByClassId(classDto.Id);

            var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId ?? 0, userId);

            if (!isManager)
            {
                TempData["Error"] = "شما اجازه اضافه کردن کلاس را ندارید";
            }
            else
            {
                var validation = schoolManagerService.AddClass(classDto);

                if (!validation)
                    TempData["Error"] = "کلاس تکراری است.";
            }

            return RedirectToAction(
                "SchoolInfo",
                "SchoolManager",
                new { schoolId = classDto.SchoolId }
            );
        }

        public IActionResult GetMajorForAddClass(int generalGradeId)
        {
            var majors = generalService.GetGenerals("Major", generalGradeId);

            return Json(majors);
        }

        public IActionResult GetGradesForAddClass(int schoolId)
        {
            var grades = generalService.GetGeneralGrades(schoolId);

            return Json(grades);
        }

        [HttpPost]
        public IActionResult DeleteClass(int classId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var schoolId = schoolManagerService.GetSchoolIdByClassId(classId);
            var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId ?? 0, userId);
            if (!isManager)
            {
                return Json(false);
            }
            else
            {
                var validation = schoolManagerService.DeleteClass(classId);
                return Json(validation);
            }
        }

        public IActionResult ClassInfo(int classId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var schoolId = schoolManagerService.GetSchoolIdByClassId(classId) ?? 0;
            var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId, userId);
            if (!isManager)
            {
                TempData["Eror"] = "شما اجازه دسترسی به این کلاس را ندارید.";
                return RedirectToAction("SchoolInfo", "Manager", new { schoolId = schoolId });
            }
            else
            {
                var managerClassInfo = new ManagerClassInfoDto { ClassId = classId, SchoolId = schoolId };
                managerClassInfo = schoolManagerService.GetManagerClassDetail(managerClassInfo);
                managerClassInfo.Average = schoolManagerService.GetClassAverage(classId);
                managerClassInfo.Students = schoolManagerService.GetStudentsDetail(classId);
                managerClassInfo.Teachers = schoolManagerService.GetTeachersDetail(classId);
                managerClassInfo.Class = schoolManagerService.GetClassDetail(classId);

                return View(managerClassInfo);
            }
        }


        public IActionResult UpdateClass(ClassDto clas)
        {
            var msg = schoolManagerService.UpdateClass(clas);
            if (msg.Success)
                TempData["Success"] = msg.Message;
            else
                TempData["Eror"] = msg.Message;



            return RedirectToAction("ClassInfo", "SchoolManager", new { classId = clas.Id });
        }


        public IActionResult GetMajorForUpdateClass(int classId, int generalGradeId)
        {

            var currentIem = generalService.GetCurrentItem(classId, "Major");
            var majors = generalService.GetGenerals("Major", generalGradeId);

            return Json(new
            {
                Majors = majors,
                CurrentIem = currentIem
            });
        }
        public IActionResult GetGradeForUpdateClass(int classId, int schoolId)
        {
            var currentIem = generalService.GetCurrentItem(classId, "Grade");
            var grades = generalService.GetGeneralGrades(schoolId);

            return Json(new
            {
                Grades = grades,
                CurrentIem = currentIem
            });
        }



        public IActionResult AddStudent(int classId,string nationalId)
        {
            var isAdd= schoolManagerService.AddStudent(new StudentDto { ClassId = classId} , nationalId);
            if (isAdd.Success)
            {
                TempData["Success"] = isAdd.Message;
            }
            else
            {
                TempData["Eror"] = isAdd.Message;
            }
            return RedirectToAction("ClassInfo", "SchoolManager", new { classId = classId });
        }


        public IActionResult GetAvailableSubjects(int classId)
        {
            var gradeId = schoolManagerService.GetGradeIdByClassId(classId);
            var scores = generalService.GetAvailableSubjectsByGradeId(gradeId:gradeId??0,classId:classId);
            return Json(scores);
        }



        public IActionResult AddTeacher(Dto.Manager.TeacherDto addTeacher, string nationalId)
        {
            var isAdd = schoolManagerService.AddTeacher(new Dto.Manager.TeacherDto { ClassId = addTeacher.ClassId , SubjectIds = addTeacher.SubjectIds }, nationalId);
            if (isAdd.Success)
                TempData["Success"] = isAdd.Message;
            else
                TempData["Eror"] = isAdd.Message;
            
            return RedirectToAction("ClassInfo", "SchoolManager", new { classId = addTeacher.ClassId });
        }


        public IActionResult DeleteStudent(int studentId)
        {
            var isDeleted = schoolManagerService.DeleteStudent(studentId);
            return Json(isDeleted);
        }

        public IActionResult DeleteTeacher(int classId,int teacherUserId)
        {
            var isDeleted = schoolManagerService.DeleteTeacher(classId: classId, teacherUserId: teacherUserId);
            return Json(isDeleted);
        }


        public IActionResult UpdateTeacher(Dto.Manager.TeacherDto teacher)
        {
            var isAdd = schoolManagerService.UpdateTeacher(teacher);
            if (isAdd.Success)
                TempData["Success"] = isAdd.Message;
            else
                TempData["Eror"] = isAdd.Message;

            return RedirectToAction("ClassInfo", "SchoolManager", new { classId = teacher.ClassId }); 
        }


        public IActionResult GetSubjectsForUpdateTeacher(int classId,int teacherUserId)
        {
            var gradeId = schoolManagerService.GetGradeIdByClassId(classId);
            var subjects = generalService.GetAvailableSubjectsByGradeIdForUpdateTeacher(gradeId??0,classId,teacherUserId);
            var teacherSubjectIds = generalService.GetTeacherSubjects(classId, teacherUserId).Select(x=> x.Id);
            return Json(new { AvailableSubjects = subjects, TeacherSubjectIds=teacherSubjectIds});
        }



        public IActionResult StudentInfo(int schoolId,int studentId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId, userId);

            var classId = studentService.GetStudentClassId(studentId, schoolId);
            if ((classId == 0 || classId == null)&& !isManager)
            {
                TempData["Eror"] = "شما اجازه دسترسی به این صفحه را ندارید.";
                return RedirectToAction("SchoolPage", "School");
            }
            else
            {
                var studentHome = new StudentHomeDto { StudentInfo = new StudentInfoDto { ClassId = classId ?? 0 } };

                studentHome.Subjects = studentService.GetSubjects(classId ?? 0);

                studentHome.SubjectInfo.SubjectId = studentHome.Subjects.FirstOrDefault()?.Id??0;

                studentHome.SubjectInfo.Scores = studentService.GetScores(studentId, studentHome.SubjectInfo.SubjectId, classId ?? 0);

                studentHome.SubjectInfo = studentService.GetSubjectInfo(subjectInfo: studentHome.SubjectInfo, classId: classId ?? 0, userId: studentId);

                studentHome.StudentInfo = studentService.GetStudentInfo(userId: studentId, classId: classId ?? 0, schoolId: schoolId);

                studentHome.Averages = studentService.GetAverages(studentId, classId!.Value);
                return View(studentHome);
            }


        }

        [HttpGet]
        public IActionResult SubjectInfo(int subjectId, int classId,int studentId)
        {

            var model = new SubjectInfoDto
            {
                SubjectId = subjectId,
                Scores = studentService.GetScores(studentId, subjectId, classId)
            };

            model = studentService.GetSubjectInfo(
                model,
                classId,
                studentId
            );

            return PartialView("_SubjectInfo", model);
        }
    }
}
