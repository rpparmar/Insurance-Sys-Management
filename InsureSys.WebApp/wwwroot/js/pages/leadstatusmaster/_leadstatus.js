const columnsConfig = [
    {
        data: "LeadStatus",
        bSortable: true,
    },
    {
        data: "CreatedOn",
        bSortable: true,
        render: function (data, type, row) {
            return data ? moment(data).format("MM/DD/YYYY hh:mm A") : "";
        }
    },
    {
        data: "UpdatedOn",
        bSortable: true,
        render: function (data, type, row) {
            return data ? moment(data).format("MM/DD/YYYY hh:mm A") : "";
        }
    },
    {
        data: "IsActive",
        bSortable: true,
        defaultContent: '',
        className: "text-center",
        render: function (data, type) {
            if (!data)
                return '<span class="label font-weight-bold label-lg label-light-danger label-inline">Inactive</span>';
            else
                return '<span class="label font-weight-bold label-lg label-light-primary label-inline">Active</span>';
        }
    },
    {
        title: 'Actions',
        bSortable: false,
        width: '170px',
        defaultContent: "",
        className: "text-center"
        , render: function (data, type, full, meta) {
            let LeadStatusID = parseInt(full.LeadStatusID, 10);
            return '<a href="/LeadStatus/Edit/' + LeadStatusID + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
                '<i class="flaticon-edit"></i>' +
                '</a>' +
                '<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
                'onclick="DeleteConfirmationWithTypeDelete(' + LeadStatusID + ',\'LeadStatus\',\'LeadStatus\')">' +
                '<i class="flaticon-delete"></i>' +
                '</a>'
                ;
        }
    }
];

$(function () {
    if (!globalvar.IsformView) {
        initDataTable();
    }
    else {
        //additional script to be written for Add/Update form if required
    }
    if (parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (parseInt(globalvar.rowsaffected) == 0)
        toastr.error(globalvar.tostarMsg);
});
function initDataTable() {
    LoadGridData({
        tableId: "#dt_leadStatus",
        url: globalvar.listingURL,
        columns: columnsConfig,
        extraParams: [{ name: "searchText", value: $('#txtSearch').val() }]
    });
}
$("#txtSearch").on('keyup', function () {
    initDataTable();
});
