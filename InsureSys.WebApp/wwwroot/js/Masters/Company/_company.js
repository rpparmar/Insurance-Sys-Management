$(function () {
    if (!globalvar.IsformView) {
        LoadGridData();
    }
    else {
        //additional script to be written for Add/Update form if required
    }
    if (parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (parseInt(globalvar.rowsaffected) == 0)
        toastr.error(globalvar.tostarMsg);
});

function LoadGridData() {
    let searchText = encodeURIComponent($("#txt_search_query").val());
    let chr_status = $(".switch_status:checked").attr('data-val') == "1";
    let _url = globalvar.actionURL + '/?searchtxt=' + searchText + '&status=' + chr_status;
    getpaging(globalvar.divID, _url, 1);
}
$('.switch_status').on('change', function (event) {
    let activerdb = $(this);
    activerdb.closest('label').toggleClass('btn-default btn-primary');
    let inactiverdb = $('input[type="radio"][name="status"]').not(':checked');
    inactiverdb.closest('label').toggleClass('btn-primary btn-default');
    LoadGridData();
});


/* Add Prompt Alert Comfirmation when Click To Active Or InActive Toggle. */
function StatusChangeConfirmation(ID) {
    let Title = "";
    if ($('#chkstatus_' + ID).is(':checked')) { Title = globalconst.activateRecord;}
    else { Title = globalconst.deactivateRecord; }
    Swal.fire({
        title: Title,
        text: '',
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes"
    }).then(function (result) {
        let IsActive = $('#chkstatus_' + ID).is(':checked') == "1";
        if (result.value) {
            $.ajax({
                type: "Get",
                url: "/Company/UpdateStatus/?id=" + ID + "&&status=" + IsActive + "",
                async: false,
                dataType: "json",
                contentType: "application/json",
                success: function (result) {
                    toastr.success(globalconst.statusChangeMsg);
                    LoadGridData();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {}
            });
        }
        else if (IsActive) { $('#chkstatus_' + ID).prop('checked', false); }
        else {$('#chkstatus_' + ID).prop('checked', true);}
    });
}
$(document).on('keypress', '#txt_search_query', function (e) {
    if (e.which == 13) {
        LoadGridData();
    }
});