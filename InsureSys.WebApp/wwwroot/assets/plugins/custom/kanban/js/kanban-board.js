"use strict";

// Class definition

var KTKanbanBoardDemo = function () {
   
    var LoadPartial = function () {
        var _url = '/' + '@culture' + '/ReportsConfig/GetReportsList';
        $.ajax({
            type: "GET",
            url: _url,
            cache: false,
            contentType: "application/html",
            error: function (error) {
            },
            success: function (data) {
                let div = document.getElementById("kt_kanban_4");
                div.replaceChildren();
                data.replace(/'/g, "\\'")
                getKanban(data);
            }
        });
    }

   
    return {
        init: function () {
            LoadPartial();
        }
    };
    
}();

jQuery(document).ready(function () {
    KTKanbanBoardDemo.init();
});


function getKanban(kboards) {
        var kanban = new jKanban({
            
            element: '#kt_kanban_4',
            gutter: '0',
            click: function (el) {
            },
          
            boards: eval(kboards)

        });

     
}




