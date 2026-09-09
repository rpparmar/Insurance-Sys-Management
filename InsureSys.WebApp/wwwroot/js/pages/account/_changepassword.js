$(function () {
    if (Number.parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (Number.parseInt(globalvar.rowsaffected) === 0 && globalvar.tostarMsg)
        toastr.error(globalvar.tostarMsg);
});
