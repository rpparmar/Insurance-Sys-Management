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
			data: "CompanyName",
			bSortable: false,			
		},
		{
			data: "PolicyType",
			bSortable: false,			
		},
		{
			data: "InquiryDate",
			bSortable: true,			
			render: function (data, type, row) {				
				// Check if data is a valid date
				if (!data || data === 'NA' || !moment(data).isValid()) {
					return 'NA';
				}
				return moment(data).format("MM/DD/YYYY hh:mm A");
			}
		},
		{
			data: "NextFollowUpDate",
			bSortable: true,
			render: function (data, type, row) {				
				// Check if data is a valid date
				if (!data || data === 'NA' || !moment(data).isValid()) {
					return 'NA';
				}
				return moment(data).format("MM/DD/YYYY hh:mm A");
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
		extraParams: [
			{ name: "searchText", value: $('#txtSearch').val() }
			//,{ name: "StatusId", value: 'active' } //extra parameter
		]
	});
}

$("#txtSearch").on('keyup', function () {
	initDataTable();
});
$("#drpPolicyTypeID").on('change', function () {
	/*
	const policyTypeId = $(this).val();
	let $companiesDropdown = $('#drpCompanyID');
	$companiesDropdown.empty();	
	$companiesDropdown.append(
		$('<option>', {
			value: '',
			text: 'Select'
		})
	);
	if (policyTypeId) {
		$.ajax({
			url: '/Lead/GetCompaniesByPolicyType',
			type: 'POST',
			data: { insuranceTypeId: policyTypeId },
			success: function (response) {
				if (response && response.length > 0) {
					$.each(response, function (i, item) {
						$companiesDropdown.append(
							$('<option>', {
								value: item.value,
								text: item.text
							})
						);
					});
					$companiesDropdown.selectpicker('refresh'); // if you're using Bootstrap SelectPicker
				} else {
					$companiesDropdown.selectpicker('refresh');
				}
			},
			error: function () {
				toastr.error('Error occurred while fetching companies.');
			}
		});
	}
	else 
		$companiesDropdown.selectpicker('refresh'); // if you're using Bootstrap SelectPicker

	*/
	bindDropdownByDependency({
		sourceSelector: this,
		targetSelector: "#drpCompanyID",
		endpointUrl: "/Lead/GetCompaniesByPolicyType",
		paramName: "insuranceTypeId",
		includeDefaultOption: true,
		defaultOptionText: 'Select',
		useSelectPicker: true
	});
});


