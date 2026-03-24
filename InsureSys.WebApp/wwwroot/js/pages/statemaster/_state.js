const stateColumnsConfig = [
    { data: "StateName", bSortable: true },
    { data: "StateCode", bSortable: true },
    { data: "CountryName", bSortable: true },
    {
        data: "CreatedOn",
        bSortable: true,
        render: function (data) { return data ? moment(data).format("MM/DD/YYYY hh:mm A") : ""; }
    },
    {
        data: "UpdatedOn",
        bSortable: true,
        render: function (data) { return data ? moment(data).format("MM/DD/YYYY hh:mm A") : ""; }
    },
    {
        data: "IsActive",
        bSortable: true,
        className: "text-center",
        render: function (data) {
            return data
                ? '<span class="label font-weight-bold label-lg label-light-primary label-inline">Active</span>'
                : '<span class="label font-weight-bold label-lg label-light-danger label-inline">Inactive</span>';
        }
    },
    {
        title: "Actions",
        bSortable: false,
        width: "170px",
        className: "text-center",
        render: function (data, type, full) {
            const stateId = parseInt(full.StateID, 10);
            return '<a href="/States/Edit/' + stateId + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3"><i class="flaticon-edit"></i></a>' +
                '<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" onclick="DeleteConfirmationWithTypeDelete(' + stateId + ',\'State\',\'State\')"><i class="flaticon-delete"></i></a>';
        }
    }
];

$(function () {
    if (!globalvar.IsformView) {
        initStateDataTable();
    }
    if (parseInt(globalvar.rowsaffected) > 0) {
        toastr.success(globalvar.tostarMsg);
    } else if (parseInt(globalvar.rowsaffected) === 0) {
        toastr.error(globalvar.tostarMsg);
    }
});

function initStateDataTable() {
    LoadGridData({
        tableId: "#dt_state",
        url: globalvar.listingURL,
        columns: stateColumnsConfig,
        extraParams: [{ name: "searchText", value: $("#txtSearch").val() }]
    });
}

$("#txtSearch").on("keyup", function () {
    initStateDataTable();
});
