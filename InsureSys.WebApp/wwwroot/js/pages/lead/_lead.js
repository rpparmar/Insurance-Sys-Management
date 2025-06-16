const columnsConfig = [
		{
			data: "FirstName",
			bSortable: true,						
		},
		{
			data: "LastName",
			bSortable: true,						
		},
		{
			data: "Email",
			bSortable: true,			
		},
		{
			data: "PhoneNumber",
			bSortable: false,			
		},
		{
			data: "CompanyID",
			bSortable: false,			
		},
		{
			data: "PolicyTypeID",
			bSortable: false,			
		},
		{
			data: "InquiryDate",
			bSortable: true,			
			render: function (data, type, row) {				
				return data ? moment(data).format("MM/DD/YYYY hh:mm A") : "";
			}
		},
		{
			data: "NextFollowUpDate",
			bSortable: true,
			render: function (data, type, row) {
				return data ? moment(data).format("MM/DD/YYYY hh:mm A") : "";
			}
		},
		{
			data: "LeadSource",
			bSortable: false,			
		},		
		{
			data: "LeadStatus",
			bSortable: false,			
		},
		{			
			title: 'Actions',
			bSortable: false,
			width: '170px',
			defaultContent: "",
			className: "text-center"
			, render: function (data, type, full, meta) {				
				let leadId = parseInt(full.LeadID, 10);
				return '<a href="/Leads/Edit/' + leadId +'" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
					'<i class="flaticon-edit"></i>' +
					'</a>'+
					'<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
					'onclick="DeleteConfirmationWithTypeDelete(' + leadId + ',\'Lead\',\'Lead\')">' +
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
		tableId: "#dt_leads",
		url: globalvar.listingURL,
		columns: columnsConfig,
		extraParams: [{ name: "searchText", value: $('#txtSearch').val() }]
	});
}

$("#txtSearch").on('keyup', function () {
	initDataTable();
});


