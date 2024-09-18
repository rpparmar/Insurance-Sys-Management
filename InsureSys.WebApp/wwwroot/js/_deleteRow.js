function DeleteConfirmation(ID, controllerName, strMessage) {
    Swal.fire({
        title: "Are you sure want to delete this " + strMessage + " permanently ?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        cancelButtonText: "Cancel",
        confirmButtonText: "Yes, delete it!"
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
                                title: "" + strMessage + " Deleted Successfully!",
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