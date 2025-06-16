function LoadGridData({
	tableId,
	url,
	columns,
	extraParams,
	scrollY = '50vh',
	scrollX = true
}) {
	if ($.fn.DataTable.isDataTable(tableId)) {
		$(tableId).DataTable().clear().destroy();
	}

	$(tableId).dataTable({
		processing: true,
		serverSide: true,
		filter: false,
		scrollY: scrollY,
		scrollX: scrollX,
		scrollCollapse: false,
		autoWidth: false,
		sPaginationType: "full_numbers",
		sAjaxSource: url,
		dom: '<"top"iflp>rt<"bottom"iflp>',
		lengthMenu: [10, 30, 50, 100],
		language: {
			emptyTable: "No data available",
			info: "Showing _START_ - _END_ of _TOTAL_ ",
			infoEmpty: "Showing 0 - _END_ of _TOTAL_",
			infoFiltered: "(filtered from _MAX_ total entries)",
			lengthMenu: " _MENU_ ",
			sProcessing: '<div class="spinner spinner-primary spinner-lg mr-15"></div>'
		},
		aoColumns: columns,
		fnServerData: function (sSource, aoData, fnCallback, oSettings) {
			ConfigureServerSideSorting(oSettings, this.fnSettings().aoColumns, aoData);			
			// Ensure extra params are added like this
			if (Array.isArray(extraParams)) {				
				extraParams.forEach(p => aoData.push(p));
			}
			oSettings.jqXHR = $.ajax({
				dataType: 'json',
				type: "POST",
				url: sSource,
				data: aoData,
				async: false,
				success: fnCallback
			});
		},
		initComplete: function () {
			$(tableId).show();
			$(tableId).DataTable().columns.adjust().draw();
		},
		drawCallback: function () {
			// Hook for row interactions or button init
		}
	});

	// Optional spinner on processing
	$(tableId).on('processing.dt', function (e, settings, processing) {
		const $wrapper = $(this).closest('.table');
		$wrapper.toggleClass('table-blur', processing);
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
