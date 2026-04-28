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
        data: "IsActive",
        bSortable: true,
        defaultContent: "",
        className: "text-center",
        render: function (data) {            
            let isActive = data === true || data === "true" || data === 1 || data === "1";
            if (!isActive)
                return '<span class="label font-weight-bold label-lg label-light-danger label-inline">InActive</span>';
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
            let agencyId = parseInt(full.AgencyId, 10);            
            return '<a href="/Agencies/Edit/' + agencyId + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
                '<i class="flaticon-edit"></i>' +
                '</a>' +
                '<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
                'onclick="DeleteConfirmationWithTypeDelete(' + agencyId + ',\'AgencyOnboarding\',\'Agency\')">' +
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
        initAgencyForm();
    }
    if (parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (parseInt(globalvar.rowsaffected) === 0 && globalvar.tostarMsg)
        toastr.error(globalvar.tostarMsg);
});

function initAgencyForm() {
    // Tooltips
    if (typeof $ !== 'undefined' && $.fn.tooltip) {
        $('[data-toggle="tooltip"]').tooltip();
    }

    // Enable admin username edit (SuperAdmin only button is rendered server-side)
    var $btn = $('#btnEnableAdminUsernameEdit');
    if ($btn.length) {
        $btn.off('click').on('click', function () {
            var $txt = $('#AdminUsername');
            $('#hdnIsAdminUsernameEditEnabled').val('true');
            $txt.removeAttr('readonly');
            $txt.focus();
        });
    }
}

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
