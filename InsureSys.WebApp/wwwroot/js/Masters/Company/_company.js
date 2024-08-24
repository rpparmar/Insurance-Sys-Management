$(document).ready(function () {
    if (!IsformView)
        LoadGridData();
    else {

    }
});
function LoadGridData() {
    var _url = actionURL;
    getpaging(divID, _url, 1);

    //$.ajax({
    //    type: "GET",
    //    url: _url,
    //    cache: false,
    //    contentType: "application/html",
    //    error: function (request, error) {
    //        alert(error);
    //    },
    //    success: function (data) {
    //        $('#CompanyList').html('');
    //        $('#CompanyList').html(data);

    //        KTDatatableHtml.init();

    //    }
    //});
}

//$(function () {
//    if (showtoastr)
//        toastr.success(message);
//});
//$("#chkIsActive").change(function () {
//    $("#hdnIsActive").val($(this).is(':checked'));
//});