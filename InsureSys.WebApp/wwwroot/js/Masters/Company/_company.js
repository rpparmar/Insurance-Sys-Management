$(function () {
    if (!IsformView) {
        if (parseInt(rowsaffected) > 0) 
            toastr.success(tostarMsg);
        else if (parseInt(rowsaffected) == 0)
            toastr.error(tostarMsg);
        LoadGridData();
    }
    else {
        //script to be written for Add/Update form
    }
});
$('.switch_status').on('change', function (event) {
    let activerdb = $(this);
    activerdb.closest('label').toggleClass('btn-default btn-primary');
    let inactiverdb = $('input[type="radio"][name="status"]').not(':checked');
    inactiverdb.closest('label').toggleClass('btn-primary btn-default');
    LoadGridData();
});

function LoadGridData() {
    let searchText = encodeURIComponent('NA');
    let chr_status = $(".switch_status:checked").attr('data-val') == "1" ? true : false;
    let _url = actionURL + '/?searchtxt=' + searchText + '&status=' + chr_status;
    getpaging(divID, _url, 1);
}