const countryColumnsConfig = [
    { data: "CountryName", bSortable: true },
    { data: "CountryCode", bSortable: true },
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
            const countryId = parseInt(full.CountryID, 10);
            return '<a href="/Countries/Edit/' + countryId + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3"><i class="flaticon-edit"></i></a>' +
                '<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" onclick="DeleteConfirmationWithTypeDelete(' + countryId + ',\'Country\',\'Country\')"><i class="flaticon-delete"></i></a>';
        }
    }
];

$(function () {
    if (!globalvar.IsformView) {
        initCountryDataTable();
    }
    if (parseInt(globalvar.rowsaffected) > 0) {
        toastr.success(globalvar.tostarMsg);
    } else if (parseInt(globalvar.rowsaffected) === 0) {
        toastr.error(globalvar.tostarMsg);
    }
});

function initCountryDataTable() {
    LoadGridData({
        tableId: "#dt_country",
        url: globalvar.listingURL,
        columns: countryColumnsConfig,
        extraParams: [{ name: "searchText", value: $("#txtSearch").val() }]
    });
}

$("#txtSearch").on("keyup", function () {
    initCountryDataTable();
});
