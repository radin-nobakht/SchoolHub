const { ajax } = require("jquery");

$(document).on("click", "#btnOpenAddSchool" ,function () { 
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
    Grades();
    modal.show();
});

function Grades() {
    let schoolId = $("#schoolId").val();

    $.ajax({
        url: "/SchoolManager/GetGrades",
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
            Major();
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function Major() {
    let generalGradeId = $("#gradeGeneral").val();

    $.ajax({
        url: "/SchoolManager/GetMajor",
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


