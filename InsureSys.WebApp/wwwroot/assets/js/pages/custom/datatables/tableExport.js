(function ($) {
    $.fn.tableExport = function (options) {
        var options = $.extend(
            {
                filename: 'table',
                format: 'csv',
                cols: '',
                excludeCols: '',
                head_delimiter: ',',
                column_delimiter: ',',
                quote: true,
                onBefore: function (t) { },
                onAfter: function (t) { },
            },
            options
        );

        var $this = $(this);

        var cols = options.cols ? options.cols.split(',') : [];
        var excludeCols = options.excludeCols ? options.excludeCols.split(',') : [];

        var result = '';

        var data_type = {
            csv: 'text/csv',
            txt: 'text/plain',
            xls: 'application/vnd.ms-excel',
            json: 'application/json',
        };

        if (
            typeof options.onBefore != 'function' ||
            typeof options.onAfter != 'function' ||
            !options.format ||
            !options.head_delimiter ||
            !options.column_delimiter ||
            !options.filename
        ) {
            console.error('One of the parameters is incorrect.');
            return false;
        }

        function getHeaders() {
            var th = $this.find('thead th').filter('.export');
            var arr = [];

            th.each(function (i, e) {
                if (excludeCols.indexOf((i + 1).toString()) == -1) {
                    arr.push(e.innerText);
                }
            });

            return arr;
        }

        function getxlsHeaders() {
            var th = $this.find('thead th').filter('.export');
            var thead = '';
            th.each(function (i, e) {
                if (excludeCols.indexOf((i + 1).toString()) == -1) {
                    var align = 'left';
                    if ($(this).hasClass('datatable-cell-right')) {
                        align = 'right';
                    }
                    thead += '<th align="' + align + '">' + e.innerText + '</th>'
                }
            });
            return thead;
        }

        function getItems() {
            var tr = $this.find('tbody tr');
            var arr = [];

            tr.each(function (i, e) {
                // stop expanded rows being exported separately
                var isCurrentRowExpanded = $(e).closest('.datatable-row-detail').length;
                if (isCurrentRowExpanded == 0) {
                    var s = [];
                    var td = $(e).find('td');
                    td.each(function (i, t) {
                        if ($(this).hasClass('export')) {
                            if (excludeCols.indexOf((i + 1).toString()) == -1) {
                                if (t.innerText.length == 0) {
                                    s.push('NA');
                                }
                                else {
                                    s.push(t.innerText);
                                }
                            }
                        }
                    });
                    arr.push(s);
                }
            });

            return arr;
        }


        function getxlsItems() {
            var tr = $this.find('tbody tr');
            var tbody = '';
            tr.each(function (i, e) {
                // stop expanded rows being exported separately
                var isCurrentRowExpanded = $(e).closest('.datatable-row-detail').length;
                if (isCurrentRowExpanded == 0) {
                    tbody += '<tr>';
                    var td = $(e).find('td');
                    td.each(function (i, t) {
                        if ($(this).hasClass('export')) {
                            if (excludeCols.indexOf((i + 1).toString()) == -1) {
                                var align = 'left';
                                if ($(this).hasClass('datatable-cell-right')) {
                                    align = 'right';
                                }
                                if (t.innerText.length == 0) {
                                    tbody += '<td align="' + align + '">NA</td>'
                                }
                                else {
                                    tbody += '<td align="' + align + '">' + t.innerText + '</td>'
                                }
                            }
                        }
                    });
                    tbody += '</tr>';
                }
            });
            return tbody;
        }

        function download(data, filename, format) {
            var a = document.createElement('a');
            var blob = new Blob(['\ufeff', data], { type: data_type[format] });
            a.href = URL.createObjectURL(blob);

            var now = new Date();
            var time_arr = [
                'DD:' + now.getDate(),
                'MM:' + (now.getMonth() + 1),
                'YY:' + now.getFullYear(),
                'hh:' + now.getHours(),
                'mm:' + now.getMinutes(),
                'ss:' + now.getSeconds(),
            ];

            time_arr.forEach(function (item) {
                var key = item.split(':')[0];
                var val = item.split(':')[1];
                var regexp = new RegExp('%' + key + '%', 'g');
                filename = filename.replace(regexp, val);
            });

            a.download = filename + '.' + format;
            a.click();

            if (!navigator.userAgent.toLowerCase().match(/firefox/)) {
                a.remove();
            }
        }

        options.onBefore($this);

        switch (options.format) {
            case 'csv':
                var headers = getHeaders();
                var items = getItems();

                if (options.quote === true) {
                    var quote = options.quote === true ? "\"" : null;

                    headers.forEach(function (item, i) {
                        headers[i] = quote + item + quote;
                    });

                    items.forEach(function (item, i) {
                        item.forEach(function (cell, j) {
                            item[j] = quote + cell + quote;
                        });
                    });
                }
                result += headers.join(options.head_delimiter) + '\n';

                items.forEach(function (item, i) {
                    result += item.join(options.column_delimiter) + '\n';
                });

                break;

            case 'txt':
                var headers = getHeaders();
                var items = getItems();

                result += headers.join(options.head_delimiter) + '\n';

                items.forEach(function (item, i) {
                    result += item.join(options.column_delimiter) + '\n';
                });

                break;

            case 'xls':
                var headers = getxlsHeaders();
                var items = getxlsItems();
                template =
                    '<table><thead>%thead%</thead><tbody>%tbody%</tbody></table>';

                template = template.replace('%thead%', headers);

                template = template.replace('%tbody%', items);
                result = template;
                break;

            case 'sql':
                var headers = getHeaders();
                var items = getItems();

                items.forEach(function (item, i) {
                    result +=
                        'INSERT INTO table (' +
                        headers.join(',') +
                        ") VALUES ('" +
                        item.join("','") +
                        "');\n";
                });

                break;

            case 'json':
                var headers = getHeaders();
                var items = getItems();

                result = JSON.stringify({ header: headers, items: items });

                break;
        }

        download(result, options.filename, options.format);

        options.onAfter($this);
    };
})(jQuery);
