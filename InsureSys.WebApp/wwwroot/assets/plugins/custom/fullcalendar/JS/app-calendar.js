/* Calendar */
/*-------- */

var eventsDisplay = [];
var todayDate = moment().startOf('day');
var YM = todayDate.format('YYYY-MM');
var TODAY = todayDate.format('YYYY-MM-DD');
var culture = 'en';

$(document).ready(function () {
    GetEmployeeSchedule('All');
})

function GetEmployeeSchedule(nvarJobcode) {
    
    eventsDisplay = [];
    var _url = '/' + culture + '/EmployeeScheduler/GetEmployeeSchedulerList';
    $.ajax({
        type: "GET",
        url: _url,
        cache: false,
        data: {
            nvarJobcode: nvarJobcode
        },
        contentType: "application/json",
        error: function (error) {
        },
        success: function (data) {
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    eventsDisplay.push({
                        //title: data[i].empnvarCashier_ID + ' - ' + data[i].strEmployeeName,
                        title: data[i].strEmployeeName,
                        start: data[i].date,
                        description: data[i].strStartTime + ' - ' + data[i].strEndTime + ' - ' + data[i].nvarJobCodeName,
                        unqRowID: data[i].unqRowID,
                        ScheduleStartDate: data[i].date,
                        className: "fc-event-success"
                    });
                }
            }
            GenerateCalender(eventsDisplay);
        }
    });
}

function GenerateCalender(eventsdisplay) {
    $("#kt_calendar").html('');
    var calendarEl = document.getElementById('kt_calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        plugins: ['interaction', 'dayGrid', 'timeGrid'],
        themeSystem: 'bootstrap',
        isRTL: KTUtil.isRTL(),
        header: {
            left: 'prev,next today',
            center: 'title',
            right: "dayGridMonth,timeGridWeek,timeGridDay"
        },
        height: 800,
        contentHeight: 780,
        aspectRatio: 3,
        nowIndicator: true,
        now: TODAY + 'T09:25:00', // just for demo

        views: {
            dayGridMonth: { buttonText: 'month' },
            timeGridWeek: { buttonText: 'week' },
            timeGridDay: { buttonText: 'day' }
        },

        defaultView: 'dayGridMonth',
        defaultDate: TODAY,

        editable: true,
        eventLimit: true, // allow "more" link when too many events
        navLinks: true,

        events: eventsdisplay,

        eventRender: function (info) {
            
            var unqRowID = info.event._def.extendedProps.unqRowID;
            var ScheduleStartDate = info.event._def.extendedProps.ScheduleStartDate;
            var EditEmployeeSchedule = "EditEmployeeSchedule('" + unqRowID + "','" + ScheduleStartDate + "')";
            var DeleteConfirmationEvent = "DeleteConfirmation('" + unqRowID + "','EmployeeSchedule','EmployeeScheduler','DeleteEmployeeSchedule')";
            var element = $(info.el);
            if (info.event.extendedProps && info.event.extendedProps.description) {
                if (element.hasClass('fc-day-grid-event')) {
                    element.data('content', info.event.extendedProps.description);
                    element.data('placement', 'top');
                    KTApp.initPopover(element);
                } else if (element.hasClass('fc-time-grid-event')) {
                    element.find('.fc-title').append('<div class="fc-description">' + info.event.extendedProps.description + '</div>');
                } else if (element.find('.fc-list-item-title').lenght !== 0) {
                    element.find('.fc-list-item-title').append('<div class="fc-description">' + info.event.extendedProps.description + '</div>');
                }
            }
            info.el.querySelector('.fc-title').innerHTML = "<i class='calendarli'>" + info.event.title + "   </i>" +
                "<a class='btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary' onclick=" + EditEmployeeSchedule + "><i class='flaticon-edit'></i></a>" +
                "<a class='btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger' onclick = " + DeleteConfirmationEvent + "><i class='flaticon-delete'></i></a ></td > ";
        }

        //eventClick: function (info) {
        //    debugger;
        //    var unqRowID = info.event._def.extendedProps.unqRowID;
        //    EditEmployeeSchedule(unqRowID);
        //}
    });
    calendar.render();
}

function SearchByJobCode() {
    var jobCode = $("#lstEmployeeSchedulerJobCodes").val();
    GetEmployeeSchedule(jobCode);
}
function EditEmployeeSchedule(unqRowID, ScheduleStartDate) {
    var todayDate = moment().startOf('day');
    var today = todayDate.format('MM/DD/YYYY');
    if (ScheduleStartDate >= today)
        window.location.href = '/' + culture + '/EmployeeScheduler/EditEmployeeScheduler?unqRowID=' + unqRowID;
    else
        toastr.error("Schedule start date should not be past date.");
}
