function DeleteConfirmation(ID, controllerName, strMessage) {
    Swal.fire({
        title: globalconst.deleteConfirmMsg,
        text: globalconst.deleteConscentMsg,
        icon: "warning",
        showCancelButton: true,
        cancelButtonText: globalconst.cancelBtnTitle,
        confirmButtonText: globalconst.cancelBtnInnerText
    }).then(
        function (result) {
            if (result.value) {
                let _dynamicURL = '/' + controllerName + '/Delete?Id=' + ID;
                $.ajax({
                    type: "POST",
                    url: _dynamicURL,
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {
                            Swal.fire({
                                position: "top-right",
                                icon: "success",
                                title: "" + strMessage + " " + globalconst.deleteSuccessMsg,
                                showConfirmButton: false,
                            });
                            setTimeout(function () {
                                window.location.reload();
                            }, 1500);
                    }
                });

            }
        });
}