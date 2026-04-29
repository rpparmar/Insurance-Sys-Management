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
            let id = parseInt(full.UserId, 10);
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
    else {
        //additional script to be written for Add/Update form if required
    }

    if (parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (parseInt(globalvar.rowsaffected) === 0 && globalvar.tostarMsg)
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

