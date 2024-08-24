"use strict";
// Class definition

var KTDatatableHtmlTableDemo1 = function () {
    // Private functions

    //  BindDynamicTable initializer
    var BindDynamicTable1 = function () {

        var datatable1 = $('#kt_datatableProduct').KTDatatable({
            data: {
                saveState: { cookie: false },
            },
            search: {
                input: $('#kt_datatable_search_query'),
                key: 'generalSearch'
            },
            columns: [
                {
                    field: 'Item Number',
                    title: 'Item Number',
                    textAlign: 'left'
                },
                {
                    field: 'Item Name',
                    title: 'Item Name',
                    textAlign: 'left'
                },
                {
                    field: 'Quantity',
                    title: 'Quantity',
                    textAlign: 'right'
                },
                {
                    field: 'Amount',
                    title: 'Amount',
                    textAlign: 'right'
                },
                {
                    field: 'Cost',
                    title: 'Cost',
                    textAlign: 'right'
                },
                {
                    field: 'Profit',
                    title: 'Profit',
                    textAlign: 'right'
                },
                {
                    field: 'Profit %',
                    title: 'Profit %',
                    textAlign: 'right'
                },
            ],
        });

        $('#kt_datatable_search_status').on('change', function () {
            datatable1.search($(this).val().toLowerCase(), 'Status');
        });

        $('#kt_datatable_search_status, #kt_datatable_search_type').selectpicker();



    };

    return {
        // Public functions
        init: function () {
            // init BindDynamicTable
            BindDynamicTable1();
        },
    };
}();

//jQuery(document).ready(function () {
//    KTDatatableHtmlTableDemo.init();
//});


