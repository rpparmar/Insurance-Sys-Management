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
		data: "Phone",
		bSortable: false,
	},	
	{
		data: "DOB",
		bSortable: true,
		render: function (data, type, row) {
			// Check if data is a valid date
			if (!data || data === 'NA' || !moment(data).isValid()) {
				return 'NA';
			}
			return moment(data).format("YYYY/MM/DD");
		}
	},	
	{
		data: "PolicyCount",
		title: 'No Of Policies',
		bSortable: true,		
		className: "text-right",
		defaultContent: "0",
		render: function (data, type, full) {
			var count = data != null && data !== '' ? parseInt(data, 10) : (full.PolicyCount != null ? parseInt(full.PolicyCount, 10) : (full.policyCount != null ? parseInt(full.policyCount, 10) : 0));
			if (isNaN(count)) count = 0;
			var cid = full.EncryptedCustomerId || full.encryptedCustomerId;
			if (!cid) {
				return '<span class="text-muted">' + count + '</span>';
			}
			return '<a href="/Customer/Policies?cid=' + encodeURIComponent(cid) + '" class="font-weight-bold text-primary">' + count + '</a>';
		}
	},
	{
		title: 'Actions',
		bSortable: false,
		width: '170px',
		defaultContent: "",
		className: "text-center"
		, render: function (data, type, full, meta) {
			var editRef = full.EncryptedCustomerId || full.encryptedCustomerId || '';
			var editHref = editRef ? '/Customer/Edit/' + encodeURIComponent(editRef) : '#';
			return '<a href="' + editHref + '" class="btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3">' +
				'<i class="flaticon-edit"></i>' +
				'</a>' +
				'<a href="javascript:;" class="btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger" ' +
				'onclick=\'DeleteConfirmationWithTypeDelete(' + JSON.stringify(editRef) + ', "Customer", "Customer")\'>' +
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
        tableId: "#dt_customers",
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