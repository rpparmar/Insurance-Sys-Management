"use strict";
// Class definition

let KTDatatableHtml = function () {
    // Private functions

    //  BindDynamicTable initializer
    let BindDynamicTable = function () {

        let datatable = $('#kt_datatable').KTDatatable({
            data: {
                saveState: { cookie: false },
            },
            search: {
                input: $('#kt_datatable_search_query'),
                key: 'generalSearch'
            },
            columns: [
                
                {
                    field: 'Action',
                    title: 'Action',
                    autoHide: false,
                    width: 125,
                    textAlign: 'center'
                },
                {
                    field: 'Active/InActive',
                    title: 'Active/InActive',
                    autoHide: false,
                    textAlign: 'center'
                },
                {
                    field: 'Delete',
                    title: 'Delete',
                    autoHide: false,
                    width: 75,
                    textAlign: 'center'
                },
                {
                    field: 'bitActive',
                    title: 'Status',
                    autoHide: false,
                    // callback function support for column rendering
                    template: function (row) {
                        let status = {
                            Active: {
                                'title': 'Active',
                                'class': ' label-light-warning'
                            },
                            InActive: {
                                'title': 'InActive',
                                'class': ' label-light-danger'
                            }
                        };
                        return '<span class="label font-weight-bold label-lg' + status[row.Status].class + ' label-inline">' + status[row.Status].title + '</span>';
                    },
                },
                
            ],
        });

        $('#kt_datatable_search_status').on('change', function () {
            datatable.search($(this).val().toLowerCase(), 'Status');
        });

        $('#kt_datatable_search_status, #kt_datatable_search_type').selectpicker();
    };

    return {
        // Public functions
        init: function () {
            // init BindDynamicTable
            BindDynamicTable();
        },
    };
}();


