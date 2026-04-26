const columnsConfig = [
    {
        data: "AgencyName",
        bSortable: true,
    },
    {
        data: "Email",
        bSortable: true,
    },
    {
        data: "ContactNo",
        bSortable: true,
    },
    {
        title: "Actions",
        bSortable: false,
        width: "170px",
        defaultContent: "",
        className: "text-center",
        render: function (data, type, full) {
            let agencyId = parseInt(full.AgencyId, 10);
            let editBtn = '<a href="/Agencies/Edit/' + agencyId + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
                '<i class="flaticon-edit"></i>' +
                '</a>';
            let isActive = full.IsActive === true || full.IsActive === "true" || full.IsActive === 1 || full.IsActive === "1";
            let deleteBtn = isActive
                ? '<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
                'onclick="DeleteConfirmationWithTypeDelete(' + agencyId + ',\'AgencyOnboarding\',\'Agency\')">' +
                '<i class="flaticon-delete"></i></a>'
                : '<span class="text-muted font-size-sm">—</span>';
            return editBtn + deleteBtn;
        }
    }
];

$(function () {
    if (!globalvar.IsformView) {
        initDataTable();
    }
    if (parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (parseInt(globalvar.rowsaffected) === 0 && globalvar.tostarMsg)
        toastr.error(globalvar.tostarMsg);
});

function initDataTable() {
    LoadGridData({
        tableId: "#dt_agencies",
        url: globalvar.listingURL,
        columns: columnsConfig,
        extraParams: [{ name: "searchText", value: $("#txtSearch").val() }]
    });
}

$("#txtSearch").on("keyup", function () {
    initDataTable();
});
