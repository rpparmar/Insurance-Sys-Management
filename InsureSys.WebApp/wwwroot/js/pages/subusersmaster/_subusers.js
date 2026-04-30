const columnsConfig = [
    { data: "UserName", bSortable: true },
    { data: "FirstName", bSortable: true },
    { data: "MiddleName", bSortable: true },
    { data: "LastName", bSortable: true },
    { data: "Email", bSortable: true },
    {
        data: "IsActive",
        bSortable: true,
        defaultContent: "",
        className: "text-center",
        render: function (data) {
            if (!data)
                return '<span class="label font-weight-bold label-lg label-light-danger label-inline">Inactive</span>';
            return '<span class="label font-weight-bold label-lg label-light-primary label-inline">Active</span>';
        }
    },
    {
        title: "Actions",
        bSortable: false,
        width: "170px",
        defaultContent: "",
        className: "text-center",
        render: function (data, type, full) {
            let id = Number.parseInt(full.UserId, 10);
            return '<a href="/SubUsers/Edit/' + id + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
                '<i class="flaticon-edit"></i>' +
                "</a>" +
                '<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
                'onclick="DeleteConfirmationWithTypeDelete(' + id + ',\'SubUsers\',\'User\')">' +
                '<i class="flaticon-delete"></i>' +
                "</a>";
        }
    }
];

$(function () {        
    if (!globalvar.IsformView) {
        initDataTable();
        $("#txtSearch").on("keyup", function () {
            initDataTable();
        });
    }
    else if (globalvar.IsformView && globalvar.IsEditMode=='False') {
        //additional script to be written for Add/Update form if required
        
        (function () {
            const $form = $("#frmSubUser");
            if (!$form.length) return;

            let $pwd = $form.find("input[name='Password']");
            let $cpwd = $form.find("input[name='ConfirmPassword']");

            // The password helper does not emit data-val-required; add it only on create.
            if ($pwd.length) {
                $pwd.attr("data-val", "true");
                $pwd.attr("data-val-required", "Enter password");
            }
            if ($cpwd.length) {
                $cpwd.attr("data-val", "true");
                $cpwd.attr("data-val-required", "Re-enter password");
            }

            // destroy the cached validator instance first,
            // then re-parse so unobtrusive picks up the new attributes
            $form.removeData("validator")
                .removeData("unobtrusiveValidation");

            $.validator.unobtrusive.parse($form);
        })();
        
    }

    if (Number.parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (Number.parseInt(globalvar.rowsaffected) === 0 && globalvar.tostarMsg)
        toastr.error(globalvar.tostarMsg);
});

function initDataTable() {
    LoadGridData({
        tableId: "#dt_subusers",
        url: globalvar.listingURL,
        columns: columnsConfig,
        extraParams: [{ name: "searchText", value: $("#txtSearch").val() }]
    });
}

