using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.Student;
using SchoolHub.Interface;
using System.Security.Claims;

namespace SchoolHub.Controllers;

public class StudentController(IStudentService studentService) : Controller
{
    public IActionResult ClassInfo(int schoolId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var classId = studentService.GetStudentClassId(userId, schoolId);
        if (classId == 0 || classId == null)
        {
            TempData["Message"] = "شما اجازه دسترسی به این صفحه را ندارید.";
            return RedirectToAction("SchoolPage", "School");
        }
        else
        {
            var studentHome = new StudentHomeDto { StudentInfo = new StudentInfoDto { ClassId = classId?? 0} };
            studentHome.Subjects = studentService.GetSubjects(classId ?? 0);
            studentHome.SubjectInfo.SubjectId = studentHome.Subjects.FirstOrDefault().Id;
            studentHome.SubjectInfo.Scores = studentService.GetScores(userId, studentHome.SubjectInfo.SubjectId);
            studentHome.SubjectInfo = studentService.GetSubjectInfo(subjectInfo: studentHome.SubjectInfo,classId:classId??0,userId:userId);
            studentHome.StudentInfo = studentService.GetStudentInfo(userId: userId, classId: classId ?? 0, schoolId: schoolId);
            return View(studentHome);
        }


    }
    [HttpGet]
    public IActionResult SubjectInfo(int subjectId, int classId)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var model = new SubjectInfoDto
        {
            SubjectId = subjectId,
            Scores = studentService.GetScores(userId,subjectId)
        };

        model = studentService.GetSubjectInfo(
            model,
            classId,
            userId
        );

        return PartialView("_SubjectInfo", model);
    }
}
