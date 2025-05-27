$(function () {
    if (!globalvar.IsformView) {
		LoadData();		
    }
    else {
        //additional script to be written for Add/Update form if required
    }
    if (parseInt(globalvar.rowsaffected) > 0)
        toastr.success(globalvar.tostarMsg);
    else if (parseInt(globalvar.rowsaffected) == 0)
        toastr.error(globalvar.tostarMsg);
});


function LoadData() {
	
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
			,render: function (data, type, full, meta) {
				return `
					<a href="javascript:;" class="btn btn-sm btn-clean btn-icon mr-2" title="Edit details">
					    <i class="flaticon-edit"></i>
					</a>
					<a href="javascript:;" class="btn btn-sm btn-clean btn-icon" title="Delete">
					    <i class="flaticon-delete"></i>
					</a>
				`;
			}
		}
	];

	const _url = globalvar.actionURL;
	if ($.fn.DataTable.isDataTable('#dt_leads')) {
		$('#dt_leads').DataTable().clear().destroy();
	}
	$('#dt_leads').dataTable({
		"processing": true, // for show progress bar
		oLanguage: {
			sProcessing: '<div class="spinner spinner-primary spinner-lg mr-15"></div>'
		},
		language: {
			emptyTable: "", // Leave this empty, we'll handle the message ourselves
			info: "Showing _START_ - _END_ of _TOTAL_ ", // Customize text
			infoEmpty: "Showing  0 - _END_ of _TOTAL_", // Text for empty tables
			infoFiltered: "(filtered from _MAX_ total entries)", // Text for filtered tables
			lengthMenu: " _MENU_ ",
		},		
		"serverSide": true, // for process server side
		"filter": false, // for hide search box
		scrollY: '50vh',
		scrollX: true,
		"scrollCollapse": false, // when true scroll shown within tbody area
		"sPaginationType": "full_numbers",		
		"autoWidth": false,		
		"sAjaxSource": _url,
		"dom": '<"top"iflp>rt<"bottom"iflp>', // hide page size dropdown at top
		"lengthMenu": [5, 10, 20, 30, 50, 100], // Options for the "Show Entries" dropdown
		"buttons": [],
		"fnServerData": function (sSource, aoData, fnCallback, oSettings) {
			// Add blur effect when data fetching starts
			$('#dt_leads').addClass('table-blur');
			ConfigureServerSideSorting(oSettings, this.fnSettings().aoColumns, aoData);			
			aoData.push({ "name": "searchText", "value": $('#txtSearch').val() });
			oSettings.jqXHR = $.ajax({				
				"dataType": 'json',
				"type": "POST",
				"url": sSource,
				"data": aoData,
				"async": false,
				"success": function (data) {
					// Call the fnCallback to populate the table
					fnCallback(data);
				},
				"complete": function () {
					// Remove blur effect when data fetching completes
					$('#dt_leads').removeClass('table-blur');
				}
			});
		},
		"aoColumns": columnsConfig,
		"initComplete": () => {			
			$("#dt_leads").show();					
			$('#dt_leads').DataTable().columns.adjust().draw();
			//setTimeout(function () {
			//	$('#dt_leads').DataTable().columns.adjust().draw();
			//}, 100); // or 200ms if needed			
		},
		"footerCallback": function (row, data, start, end, display) {			
		},
		"fnDrawCallback": function (oSettings) {						
			// Ensure blur is removed on redraw
			$('#dt_leads').removeClass('table-blur');
		}
	});
}
function ConfigureServerSideSorting(oSettings, objCols, aoData) {
	if (oSettings.aaSorting.length !== 0) {
		const arrCols = objCols.map(col => col.mData);

		const sColumnsItem = aoData.find(item => item.name === "sColumns");
		if (sColumnsItem) {
			sColumnsItem.value = arrCols.join("|");
		}

		const sortIndex = oSettings.aaSorting[0][0];
		aoData.push({
			name: "SortingField",
			value: arrCols[sortIndex]
		});
	} else {
		aoData.push({
			name: "SortingField",
			value: "0"
		});
	}
}
$("#txtSearch").on('keyup', function () {
	LoadData();
});


