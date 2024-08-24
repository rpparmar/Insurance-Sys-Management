"use strict";

// Class definition
var KTamChartsChartsDemo = function () {
    // private functions
    var Nooflicensebyreseller = function (arr_licensebyreseller) {
        var series = [];
        var labels = [];
        for (var i = 0; i < arr_licensebyreseller.length; i++) {
            series.push(arr_licensebyreseller[i].NoOfLicense);
            labels.push(arr_licensebyreseller[i].ResellerCompany);
        }
        const apexChart = "#kt_amcharts_licensebyreseller";
        var options = {
            series: series,
            chart: {
                width: 380,
                type: 'donut',
            },
            labels: labels,
            datasets: [
                {
                    data: [labels],
                },
            ],

            responsive: [{
                breakpoint: 480,
                options: {
                    chart: {
                        width: 350
                    },
                }
            }],
            legend: {
                position: 'bottom'
            },
            colors: [primary, success, warning, danger, info]
        };

        var chart = new ApexCharts(document.querySelector(apexChart), options);
        chart.render();
    };

    var Nooflicensebyproduct = function (arr_licensebyproduct) {
        var chart = AmCharts.makeChart("kt_amcharts_licensebyproduct", {
            "type": "pie",
            "theme": "light",
            "dataProvider": arr_licensebyproduct,
            "pullOutRadius": 20,
            "marginTop": 10,
            "valueField": "NoOfLicense",
            "titleField": "ProductName",
            "labelText": "[[title]]: [[value]]",
            "balloon": {
                "fixedPosition": true
            },
            //"responsive": {
            //    "enabled": true
            //},
            //"legend": {
            //    "position": "bottom",
            //    "markerSize": 12,
            //    "valueWidth": 10,
            //},
            //"export": {
            //    "enabled": true
            //},
        });
    }

    //var Nooflicensebyclient = function (arr_licensebyclient) {
    //    var chartData = {
    //        "1995": arr_licensebyclient
    //    };
    //    var currentYear = 1995;
    //    var chart = AmCharts.makeChart("kt_amcharts_licensebyclient", {
    //        "type": "pie",
    //        "theme": "light",
    //        "dataProvider": arr_licensebyclient,
    //        "valueField": "NoOfLicense",
    //        "titleField": "Client",
    //        "startDuration": 0,
    //        "innerRadius": 30,
    //        "pullOutRadius": 0,
    //        "marginTop": 0,
    //        "labelText": "[[title]]: [[value]]",
    //        "titles": [{
    //            "text": ""
    //        }],
    //        "allLabels": [{
    //            "y": "54%",
    //            "align": "center",
    //            "size": 25,
    //            "bold": true,
    //            "text": "",
    //            "color": "#555"
    //        }, {
    //            "y": "49%",
    //            "align": "center",
    //            "size": 15,
    //            "text": "",
    //            "color": "#555"
    //        }],
    //        "listeners": [{
    //            "event": "init",
    //            "method": function (e) {
    //                var chart = e.chart;

    //                function getCurrentData() {
    //                    var data = chartData[currentYear];
    //                    currentYear++;
    //                    if (currentYear > 2014)
    //                        currentYear = 1995;
    //                    return data;
    //                }

    //                function loop() {
    //                    //chart.allLabels[0].text = currentYear;
    //                    var data = getCurrentData();
    //                    chart.animateData(data, {
    //                        duration: 1000
    //                        //complete: function () {
    //                        //    setTimeout(loop, 3000);
    //                        //}
    //                    });
    //                }

    //                loop();


    //            }
    //        }],
    //        "export": {
    //            "enabled": true
    //        },
    //        "responsive": {
    //            "enabled": true
    //        }
    //    });
    //};

    var Nooflicensebyclient = function (arr_licensebyclient) {
        var chart = AmCharts.makeChart("kt_amcharts_licensebyclient", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},
            "dataProvider": arr_licensebyclient,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "itemname",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "NoOfLicense"
            }
            ],
            "rotate": true,
            "categoryField": "Client",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    var Noofsalesbypaymenttype = function (arr_salesbypaymenttype) {
        var chart = AmCharts.makeChart("kt_amcharts_salesbypaymenttype", {
            "type": "pie",
            "theme": "light",
            "dataProvider": arr_salesbypaymenttype,
            "pullOutRadius": 20,
            "marginTop": 10,
            "valueField": "amount",
            "titleField": "display",
            "balloon": {
                "fixedPosition": true
            },
            //"responsive": {
            //    "enabled": true
            //},
            //"legend": {
            //    "position": "bottom",
            //    "markerSize": 12,
            //    "valueWidth": 10,
            //},
            "balloonText": "[[title]]: <b>$[[value]]</b>",
            //"export": {
            //    "enabled": false
            //},

        });
    }

    var TopSellingItems = function (arr_topsellingitem) {
        var chart = AmCharts.makeChart("kt_amcharts_sellingitems", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},
            "dataProvider": arr_topsellingitem,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "itemname",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "quantity",
                "fillColors": ["rgba(27, 197, 189, 0.85)", "rgba(246, 78, 96, 0.85)"]
            }
            ],
            "rotate": true,
            "categoryField": "Item_Name",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    var Noofsalesbyordertype = function (arr_salesbyordertype) {

        var chart = AmCharts.makeChart("kt_amcharts_salesbyordertype", {
            "rtl": KTUtil.isRTL(),
            "type": "serial",
            "theme": "light",
            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},
            "dataProvider": arr_salesbyordertype,
            "valueAxes": [{
                "gridColor": "#FFFFFF",
                "gridAlpha": 0.2,
                "dashLength": 0
            }],
            "gridAboveGraphs": true,
            "startDuration": 1,
            "graphs": [{

                "balloonText": "Net Sales in [[category]]:<br>$[[value]]</b>",
                "fillAlphas": 0.8,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "incomeammount",
                "title": "name",
                "fillColors": ["rgba(27, 197, 189, 0.85)", "rgba(246, 78, 96, 0.85)"]
            }],
            "chartCursor": {
                "categoryBalloonEnabled": false,
                "cursorAlpha": 0,
                "zoomable": false
            },
            "categoryField": "name",
            "categoryAxis": {
                "gridPosition": "start",
                "gridAlpha": 0,
                "tickPosition": "start",
                "tickLength": 20,
                "labelRotation": 20
            },
            "export": {
                "enabled": false
            }

        });

    }

    var Last10DaySalesSummary = function (arr_10DaySalesSummary) {

        var chart = AmCharts.makeChart("kt_amcharts_salessummary", {
            "rtl": KTUtil.isRTL(),
            "type": "serial",
            "theme": "light",

            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},

            "dataProvider": arr_10DaySalesSummary,
            "valueAxes": [{
                "gridColor": "#FFFFFF",
                "gridAlpha": 0.2,
                "dashLength": 0,
            }],
            "gridAboveGraphs": true,
            "startDuration": 1,
            "graphs": [{
                "balloonText": "Gross Sales in [[category]]: <b>$[[value]]</b>",
                "fillAlphas": 0.8,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "GrossSales",
                "title": "Date",
            },
            {
                "id": "graph2",
                "balloonText": "<span style='font-size:12px;color:black;'>Net Sales in [[category]]:<br><span style='font-size:20px;'>$[[value]]</span></span>",
                "bullet": "round",
                "dashLengthField": "dashLengthColumn",
                "lineThickness": 3,
                "bulletSize": 7,
                "bulletBorderAlpha": 1,
                "bulletColor": "#FFFFFF",
                "useLineColorForBulletBorder": true,
                "bulletBorderThickness": 3,
                "fillAlphas": 0,
                "lineAlpha": 1,
                "title": "NetSales",
                "valueField": "NetSales",
            }
            ],
            //"chartCursor": {
            //    "categoryBalloonEnabled": false,
            //    "cursorAlpha": 0,
            //    "zoomable": false
            //},
            "categoryField": "Date",
            "categoryAxis": {
                "gridPosition": "start",
                "gridAlpha": 0,
                "tickPosition": "start",
                "tickLength": 20,
                "labelRotation": 20,
            },
            "export": {
                "enabled": false
            }

        });

    }

    var SalesByCategory = function (arr_salesByCategory) {
        var chart = AmCharts.makeChart("kt_amcharts_salesByCategory", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},
            "dataProvider": arr_salesByCategory,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "Category",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "quantity"
            }
            ],
            "rotate": true,
            "categoryField": "Category",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    var CouponSalesSummary = function (arr_couponSalesSummary) {
        var chart = AmCharts.makeChart("kt_amcharts_couponSalesSummary", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},
            "dataProvider": arr_couponSalesSummary,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0,
                "title": "$ (Discounted Amount)"
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>$[[value]]</b>",
                "title": "CouponName",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "Amount"
            }
            ],
            "rotate": true,
            "categoryField": "CouponName",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    var TopSellingDepartments = function (arr_topsellingdepartment) {
        var chart = AmCharts.makeChart("kt_amcharts_sellingDepartment", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},
            "dataProvider": arr_topsellingdepartment,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "Department",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "quantity",
                "fillColors": ['rgba(255, 168, 0, 0.85)', 'rgba(246, 78, 96, 0.85)']
            }
            ],
            "rotate": true,
            "categoryField": "Department",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }
    var TopPayoutByCashier = function (arr_TopPayoutByCashier) {
        var chart = AmCharts.makeChart("kt_amcharts_TopPayoutByCashier", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            "dataProvider": arr_TopPayoutByCashier,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "Cashier",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "Amount"
            }
            ],
            "rotate": true,
            "categoryField": "Cashier",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }
    //var TopSellingBrand = function (arr_topsellingbrand) {
    //    var chart = AmCharts.makeChart("kt_amcharts_sellingbrand", {
    //        "type": "serial",
    //        "theme": "light",
    //        "handDrawn": false,
    //        "handDrawScatter": 3,
    //        "dataProvider": arr_topsellingbrand,
    //        "color": "#1BC5BD",
    //        "valueAxes": [{
    //            "minorGridAlpha": 0.08,
    //            "minorGridEnabled": false,
    //            "position": "top",
    //            "axisAlpha": 0
    //        }],
    //        "startDuration": 1,
    //        "graphs": [{
    //            "balloonText": "[[category]]: <b>[[value]]</b>",
    //            "title": "itemname",
    //            "type": "column",
    //            "fillAlphas": 0.8,
    //            "valueField": "quantity",
    //            "lineColor": "#FFA800",
    //        }
    //        ],
    //        "rotate": true,
    //        "categoryField": "Description",
    //        "categoryAxis": {
    //            "gridPosition": "start"
    //        },
    //        "export": {
    //            "enabled": false
    //        }

    //    });
    //}

    var TopSellingBrand = function (arr_topsellingbrand) {
        var chart = AmCharts.makeChart("kt_amcharts_sellingbrand", {
            "type": "pie",
            "theme": "light",
            "dataProvider": arr_topsellingbrand,
            "pullOutRadius": 20,
            "marginTop": 10,
            "valueField": "quantity",
            "titleField": "Description",
            "balloon": {
                "fixedPosition": true
            },
            "balloonText": "[[Description]]: <b>[[quantity]]</b>",
        });
    }

    //var Noofclientbybusinesstype = function (arr_clientbybusinesstype) {
    //    const apexChart = "#kt_amcharts_clientbybusinesstype";
    //    var series = [];
    //    var labels = [];
    //    for (var i = 0; i < arr_clientbybusinesstype.length; i++) {
    //        series.push(arr_clientbybusinesstype[i].NoOfClient);
    //        labels.push(arr_clientbybusinesstype[i].BusinessType);
    //    }
    //    var options = {
    //        series: series,
    //        chart: {
    //            width: 380,
    //            type: 'pie',
    //        },
    //        labels: labels,
    //        responsive: [{
    //            breakpoint: 480,
    //            options: {
    //                chart: {
    //                    width: 350
    //                },
    //            }
    //        }],
    //        legend: {
    //            position: 'bottom'
    //        },
    //        colors: ["rgb(204, 71, 72)", "rgb(205, 130, 173)", "rgb(103, 183, 220)", "rgb(253, 212, 0)", "rgb(132, 183, 97)"]
    //    };

    //    var chart = new ApexCharts(document.querySelector(apexChart), options);
    //    chart.render();
    //}

    //var Noofclientbyreseller = function (arr_clientbyreseller) {
    //    const apexChart1 = "#kt_amcharts_clientbyreseller";
    //    var series = [];
    //    var labels = [];
    //    for (var i = 0; i < arr_clientbyreseller.length; i++) {
    //        if (arr_clientbyreseller[i].NoOfClient > 0)
    //            series.push(arr_clientbyreseller[i].NoOfClient);
    //        if (arr_clientbyreseller[i].ResellerCompany != null && arr_clientbyreseller[i].ResellerCompany != '')
    //            labels.push(arr_clientbyreseller[i].ResellerCompany);
    //    }
    //    var options = {
    //        series: series,
    //        chart: {
    //            width: 380,
    //            type: 'pie',
    //        },
    //        labels: labels,
    //        responsive: [{
    //            breakpoint: 480,
    //            options: {
    //                chart: {
    //                    width: 350
    //                },
    //            }
    //        }],
    //        legend: {
    //            position: 'bottom'
    //        },
    //        colors: ["rgb(204, 71, 72)", "rgb(205, 130, 173)", "rgb(103, 183, 220)", "rgb(253, 212, 0)", "rgb(132, 183, 97)"]
    //    };

    //    var chart = new ApexCharts(document.querySelector(apexChart1), options);
    //    chart.render();
    //}

    var TopCustomerBySales = function (arr_TopCustomerBySales) {
        var chart = AmCharts.makeChart("kt_amcharts_TopSalesCustomer", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            "dataProvider": arr_TopCustomerBySales,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>$[[value]]</b>",
                "title": "Customer",
                "type": "column",
                "fillAlphas": 0.8,
                "fillColors": ['rgba(137, 80, 252, 0.85)', 'rgba(255, 168, 0, 0.85)'],
                "valueField": "Amount"
            }
            ],
            "rotate": true,
            "categoryField": "CustomerName",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    var Last10DayCreditCardSalesSummary = function (arr_10DayCreditCardSalesSummary) {

        var chart = AmCharts.makeChart("kt_amcharts_creditcardsalessummary", {
            "rtl": KTUtil.isRTL(),
            "type": "serial",
            "theme": "light",

            //"legend": {
            //    "useGraphSettings": true,
            //    "markerSize": 12,
            //    "valueWidth": 0,
            //    "verticalGap": 0
            //},

            "dataProvider": arr_10DayCreditCardSalesSummary,
            "valueAxes": [{
                "gridColor": "#FFFFFF",
                "gridAlpha": 0.2,
                "dashLength": 0,
            }],
            "gridAboveGraphs": true,
            "startDuration": 1,
            "graphs": [{
                "balloonText": "Gross Sales in [[category]]: <b>$[[value]]</b>",
                "fillAlphas": 0.8,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "GrossSales",
                "title": "Date",
            },
            {
                "id": "graph2",
                "balloonText": "<span style='font-size:12px;color:black;'>Net Sales in [[category]]:<br><span style='font-size:20px;'>$[[value]]</span></span>",
                "bullet": "round",
                "dashLengthField": "dashLengthColumn",
                "lineThickness": 3,
                "bulletSize": 7,
                "bulletBorderAlpha": 1,
                "bulletColor": "#FFFFFF",
                "useLineColorForBulletBorder": true,
                "bulletBorderThickness": 3,
                "fillAlphas": 0,
                "lineAlpha": 1,
                "title": "NetSales",
                "valueField": "NetSales",
            }
            ],
            //"chartCursor": {
            //    "categoryBalloonEnabled": false,
            //    "cursorAlpha": 0,
            //    "zoomable": false
            //},
            "categoryField": "Date",
            "categoryAxis": {
                "gridPosition": "start",
                "gridAlpha": 0,
                "tickPosition": "start",
                "tickLength": 20,
                "labelRotation": 20,
            },
            "export": {
                "enabled": false
            }

        });

    }

    var WeekDayDaySalesSummary = function (arr_WeekdaySalesSummary) {
        var chart = AmCharts.makeChart("kt_amcharts_weekdaysalessummary", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            "dataProvider": arr_WeekdaySalesSummary,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "bottom",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "Net Sales in [[category]]: <b>$[[value]]</b>",
                "title": "NetSales",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "NetSales",
                "lineColor": "#1bc5bd",
            }
            ],
            "rotate": true,
            "categoryField": "Day",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }
        });

    }

    var Last12MonthSalesSummary = function (arr_12MonthsSalesSummary) {
        var chart = AmCharts.makeChart("kt_amcharts_monthlysales", {
            "rtl": KTUtil.isRTL(),
            "type": "serial",
            "theme": "light",

            "dataProvider": arr_12MonthsSalesSummary,
            "valueAxes": [{
                "gridColor": "#FFFFFF",
                "gridAlpha": 0.2,
                "dashLength": 0,
            }],
            "gridAboveGraphs": true,
            "startDuration": 1,
            "graphs": [{
                "balloonText": "Gross Sales in [[category]]: <b>$[[value]]</b>",
                "fillAlphas": 0.8,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "GrossSales",
                "title": "GrossSales",
                "fillColors": ['rgba(137, 80, 252, 0.85)', 'rgba(255, 168, 0, 0.85)'],
            },
            {
                "id": "graph2",
                "balloonText": "<span style='font-size:12px;color:black;'>Net Sales in [[category]]:<br><span style='font-size:20px;'>$[[value]]</span></span>",
                "bullet": "round",
                "dashLengthField": "dashLengthColumn",
                "lineThickness": 3,
                "bulletSize": 7,
                "bulletBorderAlpha": 1,
                "bulletColor": "#FFFFFF",
                "useLineColorForBulletBorder": true,
                "bulletBorderThickness": 3,
                "fillAlphas": 0,
                "lineAlpha": 1,
                "title": "NetSales",
                "valueField": "NetSales",
            }
            ],
            "categoryField": "Xaxis_Category",
            "categoryAxis": {
                "gridPosition": "start",
                "gridAlpha": 0,
                "tickPosition": "start",
                "tickLength": 20,
                "labelRotation": 20,
            },
            "export": {
                "enabled": false
            }

        });

    }

    var Last12WeekSalesSummary = function (arr_12WeeksSalesSummary) {
        var chart = AmCharts.makeChart("kt_amcharts_weeklysales", {
            "rtl": KTUtil.isRTL(),
            "type": "serial",
            "theme": "light",

            "dataProvider": arr_12WeeksSalesSummary,
            "valueAxes": [{
                "gridColor": "#FFFFFF",
                "gridAlpha": 0.2,
                "dashLength": 0,
            }],
            "gridAboveGraphs": true,
            "startDuration": 1,
            "graphs": [{
                "balloonText": "Gross Sales in [[category]]: <b>$[[value]]</b>",
                "fillAlphas": 0.8,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "GrossSales",
                "title": "GrossSales",
                "fillColors": ['rgba(137, 80, 252, 0.85)', 'rgba(255, 168, 0, 0.85)'],
            },
            {
                "id": "graph2",
                "balloonText": "<span style='font-size:12px;color:black;'>Net Sales in [[category]]:<br><span style='font-size:20px;'>$[[value]]</span></span>",
                "bullet": "round",
                "dashLengthField": "dashLengthColumn",
                "lineThickness": 3,
                "bulletSize": 7,
                "bulletBorderAlpha": 1,
                "bulletColor": "#FFFFFF",
                "useLineColorForBulletBorder": true,
                "bulletBorderThickness": 3,
                "fillAlphas": 0,
                "lineAlpha": 1,
                "title": "NetSales",
                "valueField": "NetSales",
            }
            ],
            "categoryField": "Xaxis_Category",
            "categoryAxis": {
                "gridPosition": "start",
                "gridAlpha": 0,
                "tickPosition": "start",
                "tickLength": 20,
                "labelRotation": 20,
            },
            "export": {
                "enabled": false
            }

        });

    }

    var HourlyTopSellingItems = function (arr_hourlytopsellingitem) {
        var chart = AmCharts.makeChart("kt_amcharts_hourlysellingitems", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            "dataProvider": arr_hourlytopsellingitem,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "itemname",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "quantity",
                "fillColors": ['rgba(246, 78, 96, 1)', 'rgba(54, 153, 255, 1)']
            }
            ],
            "rotate": true,
            "categoryField": "Item_Name",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    var _DayToDayComparison = function (arr_DayToDayComparison) {
        var chart = AmCharts.makeChart("kt_apexcharts_DayToDayComparison", {
            "theme": "light",
            "type": "serial",
            "dataProvider": arr_DayToDayComparison,
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[dtDate1]]: <b>$[[value]]</b>",
                "title": "nvarDayName",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "dtSale1",
                "fillColors": ["#3699FF","#F64E60"]
            }, {
                "balloonText": "[[dtDate2]]: <b>$[[value]]</b>",
                "title": "nvarDayName",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "dtSale2",
                "fillColors": ["#FFA800","#F64E60"]
            }, {
                "balloonText": "[[dtDate3]]: <b>$[[value]]</b>",
                "title": "nvarDayName",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "dtSale3",
                "fillColors": ["#1BC5BD","#F64E60"]
            }],
            "plotAreaFillAlphas": 0.1,
            "categoryField": "nvarDayName",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": true
            }
        });
    }

    //Super Admin Dashboard charts
    var TopAccountsByReseller = function (arr_AccountByReseller) {
        var chart = AmCharts.makeChart("kt_amcharts_AccountByReseller", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,            
            "dataProvider": arr_AccountByReseller,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "Reseller",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "NoOfClient",
                "fillColors": ['rgba(255, 168, 0, 0.85)', 'rgba(246, 78, 96, 0.85)']
            }
            ],
            "rotate": true,
            "categoryField": "Reseller_Company",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    var TopAccountByBusinessType = function (arr_AccountByBusinessType) {
        var chart = AmCharts.makeChart("kt_amcharts_clientbybusinesstype", {
            "type": "serial",
            "theme": "light",
            "handDrawn": false,
            "handDrawScatter": 3,
            "dataProvider": arr_AccountByBusinessType,
            "valueAxes": [{
                "minorGridAlpha": 0.08,
                "minorGridEnabled": false,
                "position": "top",
                "axisAlpha": 0
            }],
            "startDuration": 1,
            "graphs": [{
                "balloonText": "[[category]]: <b>[[value]]</b>",
                "title": "BusinessType",
                "type": "column",
                "fillAlphas": 0.8,
                "valueField": "NoOfClient",
                "fillColors": ['rgba(255, 168, 0, 0.85)', 'rgba(246, 78, 96, 0.85)']
            }
            ],
            "rotate": true,
            "categoryField": "BusinessType",
            "categoryAxis": {
                "gridPosition": "start"
            },
            "export": {
                "enabled": false
            }

        });
    }

    return {
        // public functions
        initnooflicensebyreseller: function (arr_licensebyreseller) {
            if (typeof arr_licensebyreseller === "undefined" || arr_licensebyreseller === null || arr_licensebyreseller.length === 0 || arr_licensebyreseller === "NaN") {
                arr_licensebyreseller = null;
            }
            Nooflicensebyreseller(arr_licensebyreseller);
        },
        initlicensebyproduct: function (arr_licensebyproduct) {
            if (typeof arr_licensebyproduct === "undefined" || arr_licensebyproduct === null || arr_licensebyproduct.length === 0 || arr_licensebyproduct === "NaN") {
                arr_licensebyproduct = null;
            }
            Nooflicensebyproduct(arr_licensebyproduct);
        },
        initclientbybusinesstype: function (arr_clientbybusinesstype) {

            if (typeof arr_clientbybusinesstype === "undefined" || arr_clientbybusinesstype === null || arr_clientbybusinesstype.length === 0 || arr_clientbybusinesstype === "NaN") {
                arr_clientbybusinesstype = null;
            }
            Noofclientbybusinesstype(arr_clientbybusinesstype);
        },
        initlicensebyclient: function (arr_licensebyclient) {
            if (typeof arr_licensebyclient === "undefined" || arr_licensebyclient === null || arr_licensebyclient.length === 0 || arr_licensebyclient === "NaN") {
                arr_licensebyclient = null;
            }
            Nooflicensebyclient(arr_licensebyclient);
        },
        initsalesbyordertype: function (arr_salesbyordertype) {
            if (typeof arr_salesbyordertype === "undefined" || arr_salesbyordertype === null || arr_salesbyordertype.length === 0 || arr_salesbyordertype === "NaN") {
                arr_salesbyordertype = null;
            }
            Noofsalesbyordertype(arr_salesbyordertype);
        },
        initsalesbypaymenttype: function (arr_salesbypaymenttype) {
            if (typeof arr_salesbypaymenttype === "undefined" || arr_salesbypaymenttype === null || arr_salesbypaymenttype.length === 0 || arr_salesbypaymenttype === "NaN") {
                arr_salesbypaymenttype = null;
            }
            Noofsalesbypaymenttype(arr_salesbypaymenttype);
        },
        inittopsellingitem: function (arr_topsellingitem) {
            if (typeof arr_topsellingitem === "undefined" || arr_topsellingitem === null || arr_topsellingitem.length === 0 || arr_topsellingitem === "NaN") {
                arr_topsellingitem = null;
            }
            TopSellingItems(arr_topsellingitem);
        },
        initsalessummary: function (arr_10DaySalesSummary) {
            if (typeof arr_10DaySalesSummary === "undefined" || arr_10DaySalesSummary === null || arr_10DaySalesSummary.length === 0 || arr_10DaySalesSummary === "NaN") {
                arr_10DaySalesSummary = null;
            }
            Last10DaySalesSummary(arr_10DaySalesSummary);
        },
        inittopsellingdepartments: function (arr_topsellingdepartment) {
            if (typeof arr_topsellingdepartment === "undefined" || arr_topsellingdepartment === null || arr_topsellingdepartment.length === 0 || arr_topsellingdepartment === "NaN") {
                arr_topsellingdepartment = null;
            }
            TopSellingDepartments(arr_topsellingdepartment);
        },
        initTopPayoutByCashier: function (arr_TopPayoutByCashier) {
            if (typeof arr_TopPayoutByCashier === "undefined" || arr_TopPayoutByCashier === null || arr_TopPayoutByCashier === 0 || arr_TopPayoutByCashier === "NaN") {
                arr_TopPayoutByCashier = null;
            }
            TopPayoutByCashier(arr_TopPayoutByCashier);
        },
        initsalesbycategory: function (arr_salesbycategory) {
            if (typeof arr_salesbycategory === "undefined" || arr_salesbycategory === null || arr_salesbycategory.length === 0 || arr_salesbycategory === "NaN") {
                arr_salesbycategory = null;
            }
            SalesByCategory(arr_salesbycategory);
        },
        initcouponsalesummary: function (arr_couponSalesSummary) {
            if (typeof arr_couponSalesSummary === "undefined" || arr_couponSalesSummary === null || arr_couponSalesSummary.length === 0 || arr_couponSalesSummary === "NaN") {
                arr_couponSalesSummary = null;
            }
            CouponSalesSummary(arr_couponSalesSummary);
        },
        inittopsellingbrand: function (arr_topsellingbrand) {
            if (typeof arr_topsellingbrand === "undefined" || arr_topsellingbrand === null || arr_topsellingbrand.length === 0 || arr_topsellingbrand === "NaN") {
                arr_topsellingbrand = null;
            }
            TopSellingBrand(arr_topsellingbrand);
        },
        //initclientbyreseller: function (arr_clientbyreseller) {
        //    if (typeof arr_clientbyreseller === "undefined" || arr_clientbyreseller === null || arr_clientbyreseller.length === 0 || arr_clientbyreseller === "NaN") {
        //        arr_clientbyreseller = null;
        //    }
        //    Noofclientbyreseller(arr_clientbyreseller);
        //},
        initTopCustomerBySales: function (arr_TopCustomerBySales) {
            if (typeof arr_TopCustomerBySales === "undefined" || arr_TopCustomerBySales === null || arr_TopCustomerBySales === 0 || arr_TopCustomerBySales === "NaN") {
                arr_TopCustomerBySales = null;
            }
            TopCustomerBySales(arr_TopCustomerBySales);
        },
        initcreditcardsalessummary: function (arr_10DayCreditCardSalesSummary) {
            if (typeof arr_10DayCreditCardSalesSummary === "undefined" || arr_10DayCreditCardSalesSummary === null || arr_10DayCreditCardSalesSummary.length === 0 || arr_10DayCreditCardSalesSummary === "NaN") {
                arr_10DayCreditCardSalesSummary = null;
            }
            Last10DayCreditCardSalesSummary(arr_10DayCreditCardSalesSummary);
        },
        initweekdaysalessummary: function (arr_WeekdaySalesSummary) {
            if (typeof arr_WeekdaySalesSummary === "undefined" || arr_WeekdaySalesSummary === null || arr_WeekdaySalesSummary.length === 0 || arr_WeekdaySalesSummary === "NaN") {
                arr_WeekdaySalesSummary = null;
            }
            WeekDayDaySalesSummary(arr_WeekdaySalesSummary);
        },
        initmonthlysalessummary: function (arr_12MonthsSalesSummary) {
            if (typeof arr_12MonthsSalesSummary === "undefined" || arr_12MonthsSalesSummary === null || arr_12MonthsSalesSummary.length === 0 || arr_12MonthsSalesSummary === "NaN") {
                arr_12MonthsSalesSummary = null;
            }
            Last12MonthSalesSummary(arr_12MonthsSalesSummary);
        },
        initweeklysalessummary: function (arr_12WeeksSalesSummary) {
            if (typeof arr_12WeeksSalesSummary === "undefined" || arr_12WeeksSalesSummary === null || arr_12WeeksSalesSummary.length === 0 || arr_12WeeksSalesSummary === "NaN") {
                arr_12WeeksSalesSummary = null;
            }
            Last12WeekSalesSummary(arr_12WeeksSalesSummary);
        },
        initHourlyTopSellingItems: function (arr_hourlytopsellingitem) {
            if (typeof arr_hourlytopsellingitem === "undefined" || arr_hourlytopsellingitem === null || arr_hourlytopsellingitem.length === 0 || arr_hourlytopsellingitem === "NaN") {
                arr_hourlytopsellingitem = null;
            }
            HourlyTopSellingItems(arr_hourlytopsellingitem);
        },
        initDayToDayComparison: function (arr_dayToDayComparison) {
            if (typeof arr_dayToDayComparison === "undefined" || arr_dayToDayComparison === null || arr_dayToDayComparison.length === 0 || arr_dayToDayComparison === "NaN") {
                arr_dayToDayComparison = null;
            }
            _DayToDayComparison(arr_dayToDayComparison);
        },
        initAccountByReseller: function (arr_AccountByReseller) {
            if (typeof arr_AccountByReseller === "undefined" || arr_AccountByReseller === null || arr_AccountByReseller.length === 0 || arr_AccountByReseller === "NaN") {
                arr_AccountByReseller = null;
            }
            TopAccountsByReseller(arr_AccountByReseller);
        },
        initAccountByBusinessType: function (arr_AccountByBusinessType) {
            if (typeof arr_AccountByBusinessType === "undefined" || arr_AccountByBusinessType === null || arr_AccountByBusinessType.length === 0 || arr_AccountByBusinessType === "NaN") {
                arr_AccountByBusinessType = null;
            }
            TopAccountByBusinessType(arr_AccountByBusinessType);
        },
    };
}();

//jQuery(document).ready(function () {
//    KTamChartsChartsDemo.init();
//});