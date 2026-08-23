const columnsConfig = [
    {
        data: "InsuranceType",
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
            let InsuranceTypeId = parseInt(full.InsuranceTypeId, 10);
            return '<a href="/InsuranceType/Edit/' + InsuranceTypeId + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
                '<i class="flaticon-edit"></i>' +
                '</a>' +
                '<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
                'onclick="DeleteConfirmationWithTypeDelete(' + InsuranceTypeId + ',\'InsuranceType\',\'Insurance Type\')">' +
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
        bindInsuranceTypeIconPreview();
    }
    if (parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (parseInt(globalvar.rowsaffected) == 0)
        toastr.error(globalvar.tostarMsg);
});
function initDataTable() {
    LoadGridData({
        tableId: "#dt_insurancetype",
        url: globalvar.listingURL,
        columns: columnsConfig,
        extraParams: [{ name: "searchText", value: $('#txtSearch').val() }]
    });
}
$("#txtSearch").on('keyup', function () {
    initDataTable();
});
function SetIDs() {    
    var stationID = $('#drpCompanies').val();
    $('#AssociationWithCompanyIDs').val(stationID)
}

function bindInsuranceTypeIconPreview() {
    var $icon = $('#drpIconClass');
    var $preview = $('.insurance-type-icon-preview i');
    if (!$icon.length || !$preview.length) {
        return;
    }

    var applyPreview = function () {
        var cls = $icon.val() || 'fas fa-file-alt';
        $preview.attr('class', cls);
    };

    $icon.on('changed.bs.select change', applyPreview);
    applyPreview();
}