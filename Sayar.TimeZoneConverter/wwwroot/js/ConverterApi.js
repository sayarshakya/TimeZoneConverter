
$("#MyDate").datepicker();
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
        }, 
        failure: function (data) {
            alert(data.responseText);
        },
        error: function (data) {
            var obj = jQuery.parseJSON(data.responseText);
            $("#DisplayException").removeAttr('style');
            $("#ErrorZone").html("Error Code : " + obj.StatusCode + " <br/>Message : "+ obj.Message);
        } 
    });

    $("#MyForm").submit(function (e) {
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
                success: function (data) {
                    $("#AddCss").removeAttr('style');
                    $("#ResultTimeZone").html(data);
                },
                failure: function (data) {
                    alert(data.responseText);
                },
                error: function (data) {
                    var obj = jQuery.parseJSON(data.responseText);
                    $("#DisplayException").removeAttr('style');
                    $("#ErrorZone").html("Error Code : " + obj.StatusCode + " <br/>Message : " + obj.Message);
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
    if ($('#Time').val().trim() === "") {
        $('#Time').focus();
        $('#Time').siblings('.error').show();
        $('#Time').parents('.form-group').addClass('has-error');
        isAllValid = false;
    }
    return isAllValid;
}  