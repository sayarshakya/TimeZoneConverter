
$('#MyDate').datepicker();
$(document).ready(function () {
    $('.error').hide();
    $.ajax({
        type: "GET",
        url: commonUrl + "api/ConverterApi/GetTimeZone/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var timeZoneList = '<option value="-1" disabled>Please Select a Time Zone</option>';
            for (var i = 0; i < data.length; i++) {
                timeZoneList += '<option value="' + data[i].value + '">' + data[i].text + '</option>';
            }
            $("#TimeZoneList").html(timeZoneList);

        }, //End of AJAX Success function

        failure: function (data) {
            alert(data.responseText);
        }, //End of AJAX failure function
        error: function (data) {
            alert(data.responseText);
        } //End of AJAX error function

    });

    $('#frm1').submit(function (e) {
        e.preventDefault();
        var isOK = ValidateForm();
        if (isOK) {
            var model = {
                Time: $("#Time").val().trim(),
                MyDate: $("#MyDate").val().trim(),
                ConvertTo: $("#TimeZoneList").val().trim()
            };
            $.ajax({
                url: commonUrl + "api/ConverterApi/ConvertRequest",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(model),
                success: function (result, status, xhr) {
                    $("#AddCss").removeAttr('style');
                    $("#resultTimeZone").html(result);
                },
                error: function (xhr, status, error) {
                    console.log(xhr);
                }
            });
        }
    });
});


function ValidateForm() {
    var isAllValid = true;
    $('.error').hide();
    $('#error').empty();
    $('.form-group').removeClass('has-error');
    if ($('#MyDate').val().trim() === "") {
        $('#MyDate').focus();
        $('#MyDate').siblings('.error').show();
        $('#MyDate').parents('.form-group').addClass('has-error');
        isAllValid = false;
    }
    return isAllValid;
}  