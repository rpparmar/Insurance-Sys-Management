"use strict";
var KTDatatablesAdvancedRowGrouping = function () {

    var ServerDepartmentinit = function () {
        var table = $('#kt_datatable');

        // begin first table
        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        var Total_Count = 0;
                        var Total_CostPer = 0;
                        var Total_Sum = 0;
                        var displaySumValue;
                        var cashierName = $('tbody').find('.hdnCahierName_' + group + '').val();
                        $('tbody').find('.item_count_' + group + '').each(function () {
                            Total_Count = Total_Count + parseFloat($(this).attr('data-count'));
                            Total_Sum = Total_Sum + parseFloat($(this).closest('tr').find('.total_sum_' + group + '').attr('data-total'));
                        });

                        //$('tbody').find('.total_sum_' + group + '').each(function () {
                        //	Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        //});
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';
                        $(rows).eq(i).before(
                            '<tr class="group"><td>' + cashierName + '</td><td></td><td class="text-right">' + Total_Count + '</td><td class="text-right">' + displaySumValue + '</td></tr>',
                        );
                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var ReceivedStockinit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    group = group.replace(/\//g, "-");
                    if (last !== group) {
                        var Total_Count = 0;
                        var Total_CostPer = 0;
                        var Total_Sum = 0;
                        var displaySumValue;
                        var displayCostPer;
                        $('tbody').find('.item_count_' + group + '').each(function () {
                            Total_Count = Total_Count + parseFloat($(this).attr('data-count'));
                        });
                        $('tbody').find('.cost_per_' + group + '').each(function () {
                            Total_CostPer = Total_CostPer + parseFloat($(this).attr('data-costper'));
                        });
                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum + '';

                        if (Total_CostPer < 0) {
                            Total_CostPer = parseFloat(Total_CostPer.toString().replace('-', '')); //(Cost Per) remove minus sign and append start and end brackets
                            displayCostPer = '$(' + Total_CostPer + ')';
                        }
                        else
                            displayCostPer = '$' + Total_CostPer + '';
                        if ($('#GroupByDate').text() != '' && $('#GroupByDate').text() != undefined) {
                            $(rows).eq(i).before(
                                '<tr class="group"><td>' + group + '</td><td colspan="2"></td><td class="text-right">' + Total_Count + '</td><td class="text-right">' + displayCostPer + '</td><td class="text-right">' + displaySumValue + '</td><td colspan="2"></td></tr>',
                            );
                        }
                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var CCPaymentDetailinit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    group = group.replace(/\//g, "-");
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group"><td>Station ID : ' + group + '</td><td colspan="12"></td></tr>',
                        );

                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
                {
                    targets: [0],
                    width: 115,
                },
                {
                    targets: [4],
                    width: 75,
                },
            ],
        });
    };

    var SalesByServerCategoryinit = function () {
        var table = $('#kt_datatable');

        // begin first table
        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        var Total_Count = 0;
                        var Total_Sum = 0;
                        var displaySumValue;
                        var cashierName = $('tbody').find('.hdnCahierName_' + group + '').val();
                        $('tbody').find('.item_count_' + group + '').each(function () {
                            Total_Count = Total_Count + parseFloat($(this).attr('data-count'));
                            Total_Sum = Total_Sum + parseFloat($(this).closest('tr').find('.total_sum_' + group + '').attr('data-total'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';
                        $(rows).eq(i).before(
                            '<tr class="group"><td>' + cashierName + '</td><td></td><td class="text-right">' + Total_Count + '</td><td class="text-right">' + displaySumValue + '</td></tr>',
                        );
                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var CCDetailByCashierinit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    group = group.replace(/\//g, "-");
                    if (last !== group) {
                        var Total_Sum = 0;
                        var displaySumValue;
                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';

                        $(rows).eq(i).before(
                            '<tr class="group"><td>Date : ' + group + '</td><td colspan="2"></td><td class="text-right">' + displaySumValue + '</td><td colspan="3"></td></tr>',
                        );

                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var ItemListByPreferredVendorinit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;

                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    group = group.replace(/\//g, "-");
                    if (last !== group) {
                        var Total_CostPer = 0, CostPer = 0, InStock = 0, totalValue = 0, pricePerItem = 0;
                        var displayVendorCostPer, displayCostPer, displayTotalValue, displayPricePerItem;

                        $('tbody').find('.vendor_cost_per_' + group + '').each(function () {
                            Total_CostPer = Total_CostPer + parseFloat($(this).attr('vendor-data-costper'));
                        });
                        $('tbody').find('.cost_per_' + group + '').each(function () {
                            CostPer = CostPer + parseFloat($(this).attr('data-costper'));
                        });
                        $('tbody').find('.in_stock_' + group + '').each(function () {
                            InStock = InStock + parseFloat($(this).attr('data-instock'));
                        });
                        $('tbody').find('.total_value_' + group + '').each(function () {
                            totalValue = totalValue + parseFloat($(this).attr('data-totalvalue'));
                        });
                        $('tbody').find('.price_per_item_' + group + '').each(function () {
                            pricePerItem = pricePerItem + parseFloat($(this).attr('data-priceperitem'));
                        });
                        if (Total_CostPer < 0) {
                            Total_CostPer = parseFloat(Total_CostPer.toString().replace('-', ''));
                            displayVendorCostPer = '$(' + Total_CostPer.toFixed(2) + ')';
                        }
                        else
                            displayVendorCostPer = '$' + Total_CostPer.toFixed(2) + '';
                        if (CostPer < 0) {
                            CostPer = parseFloat(CostPer.toString().replace('-', ''));
                            displayCostPer = '$(' + CostPer.toFixed(2) + ')';
                        }
                        else
                            displayCostPer = '$' + CostPer.toFixed(2) + '';
                        if (totalValue < 0) {
                            totalValue = parseFloat(totalValue.toString().replace('-', ''));
                            displayTotalValue = '$(' + totalValue.toFixed(2) + ')';
                        }
                        else
                            displayTotalValue = '$' + totalValue.toFixed(2) + '';
                        if (pricePerItem < 0) {
                            pricePerItem = parseFloat(pricePerItem.toString().replace('-', ''));
                            displayPricePerItem = '$(' + pricePerItem.toFixed(2) + ')';
                        }
                        else
                            displayPricePerItem = '$' + pricePerItem.toFixed(2) + '';
                        $(rows).eq(i).before(
                            '<tr class="group"><td>Vendor# : ' + group.replace("_", " - ") + '</td><td colspan="3"></td><td class="text-right">' + displayVendorCostPer + '</td><td class="text-right">' + displayCostPer + '</td><td>' + InStock + '</td><td>' + displayTotalValue + '</td><td>' + displayPricePerItem + '</td></tr>',
                        );

                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var CustomerAccountSummaryinit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;
                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    group = group.replace(/\//g, "-");
                    var splitComma = group.split(',');
                    if (last !== group) {
                        var Total_Sum = 0;
                        var displaySumValue;
                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '($' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';
                        $(rows).eq(i).before(
                            '<tr class="group"><td>Customer # : ' + splitComma[0] + '<br/> Customer Name : ' + splitComma[1] + '</td><td colspan="3"></td><td class="text-right">' + displaySumValue + '</td></tr>',
                        );
                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var WastageItemReportInit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'desc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    var str = group.replaceAll('-','/');
                    var date = new Date(str);
                    var dayName = GetDayName(date.getDay());
                    group = group.replace(/\//g, "-");
                    var DisaplyName = group + ' (' + dayName + ')';

                    if (last !== group) {
                        var Total_Sum = 0;
                        var displaySumValue;
                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';

                        $(rows).eq(i).before(
                            '<tr class="group"><td class="text-dark">' + DisaplyName + '</td><td colspan="3"></td><td class="text-right text-dark">' + displaySumValue + '</td></tr>',
                        );

                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var ComplimentaryByCashierinit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'desc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    var str = group.replaceAll('-', '/');
                    var date = new Date(str);
                    var dayName = GetDayName(date.getDay());
                    group = group.replace(/\//g, "-");
                    var DisaplyName = group + ' (' + dayName + ')';

                    if (last !== group) {
                        var Total_Sum = 0;
                        var displaySumValue;
                        $('tbody').find('.total_amount_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';

                        $(rows).eq(i).before(
                            '<tr class="group"><td class="text-dark">' + DisaplyName + '</td><td colspan="5"></td><td class="text-right text-dark">' + displaySumValue + '</td></tr>',
                        );

                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var ItemDiscountDetailinit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'desc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;

                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    group = group.replace(/\//g, "-");
                    var DisaplyName = group;

                    if (last !== group) {
                        var Total_Sum = 0, DiscountPer = 0;
                        var displaySumValue, displayDiscountPer;

                        $('tbody').find('.total_amount_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });
                        $('tbody').find('.total_discamount_' + group + '').each(function () {
                            DiscountPer = DiscountPer + parseFloat($(this).attr('data-disc'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';
                        
                        if (DiscountPer < 0) {
                            DiscountPer = parseFloat(DiscountPer.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displayDiscountPer = '$(' + DiscountPer.toFixed(2) + ')';
                        }
                        else
                            displayDiscountPer = '$' + DiscountPer.toFixed(2) + '';

                            $(rows).eq(i).before(
                                '<tr class="group"><td class="text-dark">Order # :' + DisaplyName + '</td><td colspan="2"></td><td class="text-right text-dark">' + displaySumValue + '</td><td class="text-right text-dark">' + displayDiscountPer + '</td></tr>',
                            );
                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var ReservationDetailsReportInit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'desc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    var str = group.replaceAll('-', '/');
                    var date = new Date(str);
                    var dayName = GetDayName(date.getDay());
                    group = group.replace(/\//g, "-");
                    var DisaplyName = group + ' (' + dayName + ')';

                    if (last !== group) {
                        var Total_Sum = 0;
                        var displaySumValue;
                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '(' + Total_Sum.toFixed(0) + ')';
                        }
                        else
                            displaySumValue = Total_Sum.toFixed(0);

                        $(rows).eq(i).before(
                            '<tr class="group"><td class="text-dark">' + DisaplyName + '</td><td colspan="2"></td><td class="text-right text-dark">' + displaySumValue + '</td></tr>',
                        );

                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var StockRegisterReport = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[2, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    group = group.replace(/\//g, "-");
                    var DisaplyName = group;
                    group = group.replace(" ", "_").replace(" ", "_").replace(" ", "_").replace(" ", "_").replace(/[&\/\\#, +()$~%.'":*?<>{}]/g, "_");
                    if (last !== group) {
                        var Total_Sum = 0;
                        var Total_ClosingSum = 0;
                        var displaySumValue;
                        var displayClosingSumValue;

                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_Sum =  parseFloat($(this).attr('data-total'));
                        });

                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = Total_Sum.toFixed(2);
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = Total_Sum.toFixed(2);

                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_ClosingSum = parseFloat($(this).attr('data-totalClosing'));
                        });

                        if (Total_ClosingSum < 0) {
                            Total_ClosingSum = parseFloat(Total_ClosingSum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displayClosingSumValue = Total_ClosingSum.toFixed(2);
                            displayClosingSumValue = '$(' + Total_ClosingSum.toFixed(2) + ')';
                        }
                        else
                            displayClosingSumValue = Total_ClosingSum.toFixed(2);

                        $(rows).eq(i).before(
                            '<tr class="group"><td colspan="3" class="text-dark">' + DisaplyName + '</td>< td colspan = "2" ></td ></td ><td class="text-dark"> Opening Stock: </td><td class="text-right" style="color: green;">' + displaySumValue + '</td><td class="text-dark"> Closing Stock: </td><td class="text-right" style="color: red;">' + displayClosingSumValue + '</td></tr>',
                        );

                        //$(rows).eq(i).after(
                        //    '<tr class="group"><td colspan="5"></td><td class="text-dark"> Closing Stock: </td><td class="text-right text-dark">' + displaySumValue + '</td></tr>',
                        //);

                        last = group;

                    }

                });
            },
            columnDefs: [
                {
                    // Hide columns by index number
                    targets: [2],
                    visible: true,
                },
            ],
        });
    };

    var DayToDayComparisonReportInit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[4, 'desc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(2, { page: 'current' }).data().each(function (group, i) {
                    debugger
                    group = group.replace(/\//g, "-");
                    var DisaplyName = group;

                    if (last !== group) {
                        var Total_Sum = 0;
                        var Total_TransCount = 0;
                        var displaySumValue;
                        $('tbody').find('.total_sum_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                            Total_TransCount = Total_TransCount + parseFloat($(this).attr('data-totalTrans'));
                        });
                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = '$(' + Total_Sum.toFixed(2) + ')';
                        }
                        else
                            displaySumValue = '$' + Total_Sum.toFixed(2) + '';

                        $(rows).eq(i).before(
                            '<tr class="group"><td class="text-dark">' + DisaplyName + '</td><td class="text-right text-dark">' + Total_TransCount + '</td><td class="text-right text-dark">' + displaySumValue + '</td></tr>',
                        );

                        last = group;
                    }
                });
            },
            columnDefs: [
                {
                    // hide columns by index number
                    targets: [2],
                    visible: false,
                },
            ],
        });
    };

    var CustomerSalesHistoryReportDetailInit = function () {
        var table = $('#kt_datatable');

        table.DataTable({
            responsive: true,
            pageLength: 25,
            order: [[1, 'asc']],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;


                api.column(1, { page: 'current' }).data().each(function (group, i) {
                    debugger;
                    group = group.replace(/\//g, "-");
                    var DisaplyName = group;
                    //group = group.replace(" ", "_").replace(" ", "_").replace(" ", "_").replace(" ", "_").replace(/[&\/\\#,&+()$~%.'":*?<>{}]/g, "_");
                    if (last !== group) {
                        var Total_Sum = 0;
                        var displaySumValue;

                        $('tbody').find('.total_amount_' + group + '').each(function () {
                            Total_Sum = Total_Sum + parseFloat($(this).attr('data-total'));
                        });

                        if (Total_Sum < 0) {
                            Total_Sum = parseFloat(Total_Sum.toString().replace('-', '')); //remove minus sign and append start and end brackets
                            displaySumValue = Total_Sum.toFixed(2);
                        }
                        else
                            displaySumValue = Total_Sum.toFixed(2);


                        $(rows).eq(i).before(
                            '<tr class="group"><td>Order # : ' + DisaplyName + '</td ><td colspan="3"></td><td class="text-right">' + displaySumValue + '</td></tr > ',
                        );

                        //$(rows).eq(i).after(
                        //    '<tr class="group"><td colspan="5"></td><td class="text-dark"> Closing Stock: </td><td class="text-right text-dark">' + displaySumValue + '</td></tr>',
                        //);

                        last = group;

                    }

                });
            },
            columnDefs: [
                {
                    // Hide columns by index number
                    targets: [2],
                    visible: true,
                },
            ],
        });
    };

    return {

        //main function to initiate the module
        init: function () {
            init();
        },
        ServerDepartmentinit: function () {
            ServerDepartmentinit();
        },
        ReceivedStockinit: function () {
            ReceivedStockinit();
        },
        CCPaymentDetailinit: function () {
            CCPaymentDetailinit();
        },
        SalesByServerCategoryinit: function () {
            SalesByServerCategoryinit();
        },
        CCDetailByCashierinit: function () {
            CCDetailByCashierinit();
        },
        ItemListByPreferredVendorinit: function () {
            ItemListByPreferredVendorinit();
        },
        CustomerAccountSummaryinit: function () {
            CustomerAccountSummaryinit();
        },
        WastageItemReportInit: function () {
            WastageItemReportInit();
        },
        ComplimentaryByCashierinit: function () {
            ComplimentaryByCashierinit();
        },
        ItemDiscountDetailinit: function () {
            ItemDiscountDetailinit();
        },
        ReservationDetailsReportInit: function () {
            ReservationDetailsReportInit();
        },
        StockRegisterReport: function () {
            StockRegisterReport();
        },
        DayToDayComparisonReportInit: function () {
            DayToDayComparisonReportInit();
        },
        CustomerSalesHistoryReportDetailInit: function () {
            CustomerSalesHistoryReportDetailInit();
        },
    };

}();

function GetDayName(day) {
    if (day == 0) return 'Sunday';
    if (day == 1) return 'Monday';
    if (day == 2) return 'Tuesday';
    if (day == 3) return 'Wednesday';
    if (day == 4) return 'Thursday';
    if (day == 5) return 'Friday';
    if (day == 6) return 'Saturday';
}
//jQuery(document).ready(function() {
//	KTDatatablesAdvancedRowGrouping.init();
//});
