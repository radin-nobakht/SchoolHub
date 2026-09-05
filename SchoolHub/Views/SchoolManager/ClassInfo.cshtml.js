$(document).on("click", ".btnDeleteStudent", function () {

    const button = $(this);
    const studentId = button.data("id");
    const studentRow = button.closest(".student-row");


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
