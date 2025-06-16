const columnsConfig = [
	{
        data: "CompanyName",
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
            let companyID = parseInt(full.CompanyID, 10);
            return '<a href="/Companies/Edit/' + companyID + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
				'<i class="flaticon-edit"></i>' +
				'</a>' +
				'<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
                'onclick="DeleteConfirmationWithTypeDelete(' + companyID + ',\'Company\',\'Company\')">' +
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
        tableId: "#dt_company",
        url: globalvar.listingURL,
        columns: columnsConfig,
        extraParams: [{ name: "searchText", value: $('#txtSearch').val() }]
    });
}
$("#txtSearch").on('keyup', function () {
    initDataTable();
});
/*
$('.switch_status').on('change', function (event) {
    let activerdb = $(this);
    activerdb.closest('label').toggleClass('btn-default btn-primary');
    let inactiverdb = $('input[type="radio"][name="status"]').not(':checked');
    inactiverdb.closest('label').toggleClass('btn-primary btn-default');
    LoadGridData();
});
*/

/* 
function StatusChangeConfirmation(ID) {
    let Title = "";
    if ($('#chkstatus_' + ID).is(':checked')) { Title = globalconst.activateRecord;}
    else { Title = globalconst.deactivateRecord; }
    Swal.fire({
        title: Title,
        text: '',
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes"
    }).then(function (result) {
        let IsActive = $('#chkstatus_' + ID).is(':checked') == "1";
        if (result.value) {
            $.ajax({
                type: "Get",
                url: "/Company/UpdateStatus/?id=" + ID + "&&status=" + IsActive + "",
                async: false,
                dataType: "json",
                contentType: "application/json",
                success: function (result) {
                    toastr.success(globalconst.statusChangeMsg);
                    LoadGridData();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {}
            });
        }
        else if (IsActive) { $('#chkstatus_' + ID).prop('checked', false); }
        else {$('#chkstatus_' + ID).prop('checked', true);}
    });
}
*/
/*
$(document).on('keypress', '#txt_search_query', function (e) {
    if (e.which == 13) {
        LoadGridData();
    }
});
*/
