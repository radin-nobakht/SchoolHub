$(document).ready(function () {

    $(".select2").each(function () {

        const $select = $(this);
        const $modal = $select.closest(".modal");

        $select.select2({
            width: "100%",
            dir: "rtl",
            dropdownParent: $modal.length ? $modal : $(document.body),

            language: {
                noResults: function () {
                    return "موردی پیدا نشد";
                },
                searching: function () {
                    return "در حال جستجو...";
                }
            }
        });

    });

});

$(document).on("click", "#btnOpenAddSchool", function () {
    const modal = new bootstrap.Modal($("#addSchoolModal")[0]);
    generalItems();
    modal.show();
});

function city() {
    let provinceId = $("#provinceGeneral").val();

    $.ajax({
        url: "/school/GetCities",
        type: "GET",
        data: {
            provinceId: provinceId
        },
        success: function (data) {
            let select = $('#schoolCity');
            select.empty();

            $.each(data, function (index, item) {

                select.append(
                    `<option value="${item.id}">
                            ${item.title}
                        </option>`
                );

            });
            district();
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function district() {
    let cityId = $("#schoolCity").val();

    $.ajax({
        url: "/school/GetDistricts",
        type: "GET",
        data: {
            cityId: cityId
        },
        success: function (data) {
            if (data != null) {
                let select = $("#schoolDistrict");
                select.empty();

                $.each(data, function (index, item) {

                    select.append(
                        `<option value="${item.id}">
                             ${item.title}
                         </option>`
                    );

                });
                $("#District").show();
            }
            else {
                $("#District").hide();
            }

        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
};

function generalItems() {
    $.ajax({
        url: "/school/GetGeneralsSchool",
        type: "GET",
        success: function (data) {
            //#region educationLevelGeneral

            let educationLevelGeneralSelect = $("#educationLevelGeneral");
            educationLevelGeneralSelect.empty();

            $.each(data.generalEducationLevels, function (index, item) {

                educationLevelGeneralSelect.append(
                    `<option value="${item.id}">
                             ${item.title}
                         </option>`
                );

            })
            //#endregion

            //#region provinceGeneral

            let provinceGeneralSelect = $("#provinceGeneral");
            provinceGeneralSelect.empty();

            $.each(data.generalProrvince, function (index, item) {

                provinceGeneralSelect.append(
                    `<option value="${item.id}">
                             ${item.title}
                         </option>`
                );
            })
            city();

            //#endregion

            //#region TypeGeneral

            let typeGeneralSelect = $("#typeGeneral");
            typeGeneralSelect.empty();

            $.each(data.generalTypes, function (index, item) {

                typeGeneralSelect.append(
                    `<option value="${item.id}">
                             ${item.title}
                         </option>`
                );

            })
            //#endregion

            //#region shiftGeneral

            let shiftGeneralSelect = $("#shiftGeneral");
            shiftGeneralSelect.empty();

            $.each(data.generalShifts, function (index, item) {

                shiftGeneralSelect.append(
                    `<option value="${item.id}">
                             ${item.title}
                         </option>`
                );

            })
            //#endregion

            //#region educationPeriodGeneral

            let educationLevelSelect = $("#educationPeriodGeneral");
            educationLevelSelect.empty();

            $.each(data.generalEducationPeriods, function (index, item) {

                educationLevelSelect.append(
                    `<option value="${item.id}">
                             ${item.title}
                         </option>`
                );

            })
            //#endregion

            //#region genderGeneral

            let genderGeneral = $("#genderGeneral");
            genderGeneral.empty();

            $.each(data.generalGender, function (index, item) {

                genderGeneral.append(
                    `<option value="${item.id}">
                             ${item.title}
                         </option>`
                );

            })
            //#endregion
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
};

$(document).on("click", "#btnOpenAddClass", function () {
    const modal = new bootstrap.Modal($("#addClassModal")[0]);
    GradesForAddClass();
    modal.show();
});

function GradesForAddClass() {
    let schoolId = $("#schoolId").val();

    $.ajax({
        url: "/SchoolManager/GetGradesForAddClass",
        type: "GET",
        data: {
            schoolId: schoolId
        },
        success: function (data) {
            let select = $('#gradeGeneral');
            select.empty();

            $.each(data, function (index, item) {

                select.append(
                    `<option value="${item.id}">
                            ${item.title}
                        </option>`
                );

            });
            MajorForAddClass();
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function MajorForAddClass() {
    let generalGradeId = $("#gradeGeneral").val();

    $.ajax({
        url: "/SchoolManager/GetMajorForAddClass",
        type: "GET",
        data: {
            generalGradeId: generalGradeId
        },
        success: function (data) {
            if (data != null && data.length > 0) {
                // لیست حداقل یک آیتم دارد
                let select = $("#majorGeneral");
                select.empty();

                $.each(data, function (index, item) {

                    select.append(
                        `<option value="${item.id}">
                             ${item.title}
                         </option>`
                    );

                });
                $("#major").show();
            }
            else {
                $("#major").hide();
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

$(document).on("click", "#btnOpenUpdateClass", function () {
    const modal = new bootstrap.Modal($("#updateClassModal")[0]);
    GradesForUpdateClass();
    modal.show();
});

function GradesForUpdateClass() {
    let schoolId = $("#schoolId").val();
    let classId = $("#classId").val();

    $.ajax({
        url: "/SchoolManager/GetGradeForUpdateClass",
        type: "GET",
        data: {
            schoolId: schoolId,
            classId: classId
        },
        success: function (data) {
            let select = $('#gradeGeneral');
            select.empty();

            $.each(data.grades, function (index, item) {

                select.append(
                    `<option value="${item.id}" ${item.title === data.cuurentItem ? "selected" : ""}>
                             ${item.title}
                         </option>`
                );

            });
            MajorForUpdateClass();
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function MajorForUpdateClass() {
    let generalGradeId = $("#gradeGeneral").val();
    let classId = $("#classId").val();
    $.ajax({
        url: "/SchoolManager/GetMajorForUpdateClass",
        type: "GET",
        data: {
            generalGradeId: generalGradeId,
            classId: classId
        },
        success: function (data) {
            if (data.majors != null && data.majors.length > 0) {
                // لیست حداقل یک آیتم دارد
                let select = $("#majorGeneral");
                select.empty();

                $.each(data.majors, function (index, item) {

                    select.append(
                        `<option value="${item.id}" ${item.title === data.cuurentItem ? "selected" : ""}>
                             ${item.title}
                         </option>`
                    );

                });
                $("#major").show();
            }
            else {
                $("#major").hide();
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

$(document).on("click", "#AddStudentbtn", function () {

    const modal = new bootstrap.Modal($("#addStudentModal")[0]);

    modal.show();
});

$(document).on("input", "#nationalIdInput", function () {

    $(this).val(
        $(this).val()
            .replace(/\D/g, "")
            .slice(0, 10)
    );

});

$(document).on("click", "#btnAddTeacher", function () {

    const modal = new bootstrap.Modal($("#assignTeacherModal")[0]);
    SubjectsForAddTeacher();
    modal.show();
});

function SubjectsForAddTeacher() {
    let classId = $("#classId").val();
    $.ajax({
        url: "/SchoolManager/GetAvailableSubjects",
        type: "GET",
        data: {
            classId: classId
        },
        success: function (data) {
         
                // لیست حداقل یک آیتم دارد
                let select = $("#teacherSubjects");
                select.empty();

                $.each(data, function (index, item) {

                    select.append(
                        `<option value="${item.id}">
                             ${item.title}
                         </option>`
                    );

                });
             
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

$(document).on("click", ".btnDeleteStudent", function () {

    const button = $(this);
    const studentId = button.attr("data-studentid");
    const studentRow = button.closest(".student-row");

    console.log(studentId);
    Swal.fire({
        title: "حذف کلاس؟",
        text: "آیا مطمئن هستید که می‌خواهید این دانش آموز را حذف کنید؟",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "بله، حذف کن",
        cancelButtonText: "لغو",
        reverseButtons: true
    }).then((result) => {

        if (!result.isConfirmed) {
            return;
        }

        $.ajax({
            url: "/SchoolManager/DeleteStudent",
            type: "POST",
            data: {
                studentId: studentId
            },

            success: function (data) {

                if (data) {

                    Swal.fire({
                        title: "حذف شد",
                        text: "داننش آموز با موفقیت حذف شد.",
                        icon: "success",
                        timer: 1500,
                        showConfirmButton: false
                    });

                    // حذف کارت و آزاد شدن جای آن
                    studentRow.fadeOut(300, function () {
                        $(this).remove();
                    });

                } else {

                    Swal.fire({
                        title: "خطا",
                        text: "حذف دانش آموز انجام نشد.",
                        icon: "error",
                        confirmButtonText: "باشه"
                    });
                }
            },

            error: function (xhr, status, error) {

                console.error("Delete class error:", error);

                Swal.fire({
                    title: "خطا",
                    text: "در هنگام حذف کلاس مشکلی پیش آمد.",
                    icon: "error",
                    confirmButtonText: "باشه"
                });
            }
        });
    });
});

$(document).on("click", ".btnDeleteTeacher", function () {

    const button = $(this);
    const classId = button.data("class-id");
    const teacherUserId = button.data("teacher-user-id");
    const teacherRow = button.closest(".teacher-row");


    Swal.fire({
        title: "حذف کلاس؟",
        text: "آیا مطمئن هستید که می‌خواهید این دانش آموز را حذف کنید؟",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "بله، حذف کن",
        cancelButtonText: "لغو",
        reverseButtons: true
    }).then((result) => {

        if (!result.isConfirmed) {
            return;
        }

        $.ajax({
            url: "/SchoolManager/DeleteTeacher",
            type: "POST",
            data: {
                teacherUserId: teacherUserId,
                classId: classId
            },

            success: function (data) {

                if (data) {

                    Swal.fire({
                        title: "حذف شد",
                        text: "داننش آموز با موفقیت حذف شد.",
                        icon: "success",
                        timer: 1500,
                        showConfirmButton: false
                    });

                    // حذف کارت و آزاد شدن جای آن
                    teacherRow.fadeOut(300, function () {
                        $(this).remove();
                    });

                } else {

                    Swal.fire({
                        title: "خطا",
                        text: "حذف دانش آموز انجام نشد.",
                        icon: "error",
                        confirmButtonText: "باشه"
                    });
                }
            },

            error: function (xhr, status, error) {

                console.error("Delete class error:", error);

                Swal.fire({
                    title: "خطا",
                    text: "در هنگام حذف کلاس مشکلی پیش آمد.",
                    icon: "error",
                    confirmButtonText: "باشه"
                });
            }
        });
    });
});

$(document).on("click", ".btnUpdateTeacher", function () {

    const modal = new bootstrap.Modal($("#updateTeacherModal")[0]);
    $("#teacherUserId").val($(this).data("teacher-user-id"));
    SubjectsForUpdateSchool();
    modal.show();
});

function SubjectsForUpdateSchool() {
    let teacherUserId = $("#teacherUserId").val();
    let classId = $("#classId").val();
    $.ajax({
        url: "/SchoolManager/GetSubjectsForUpdateTeacher",
        type: "GET",
        data: {
            classId: classId,
            teacherUserId: teacherUserId
        },
        success: function (data) {

            // لیست حداقل یک آیتم دارد
            let select = $("#teacherSubjectsUpdate");
            select.empty();
       
            $.each(data.availableSubjects, function (index, item) {
                
                const isSelected = data.teacherSubjectIds.includes(item.id);

                select.append(
                    `<option value="${item.id}" ${isSelected ? "selected" : ""}>
                         ${item.title}
                    </option>`
                );
            });

        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}