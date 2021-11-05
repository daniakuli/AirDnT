$(function () {
        if ($(".resChangedDate").length) {
        var minDate = getResDate("startDate");
        var maxDate = getResDate("endDate");
        var result = [];

        var minDateFormated = changeFormatRes(minDate);
        var maxDateFormated = changeFormatRes(maxDate);
        var apartId = document.location.href.split('/')[5];

        $.ajax({
            async: false,
            url: "/Apartments/ReservationExist",
            data: { sdate: minDateFormated, edate: maxDateFormated, apartId: apartId }
        }).done(function (response) {
            var resStartDate;
            var resEndDate;
            if (response.length != 0) {
                for (var resLen = 0; resLen < response.length; resLen++) {
                    resStartDate = response[resLen].sAvailability.split('T')[0];
                    resEndDate = response[resLen].eAvailability.split('T')[0];
                    result.push({ sdate: resStartDate, edate: resEndDate });
                }
            }
        });
        $(".resChangedDate").datepicker({
            dateFormat: "dd-mm-yy",
            minDate: minDate,
            maxDate: maxDate,
            beforeShowDay: function (date) {
                var dateStatus = true;
                //console.log(date);
                $(result).each(function (index, element) {
                    //console.log(date);
                    if (date >= new Date(Date.parse(element.sdate)) && date <= new Date(Date.parse(element.edate))) {
                        //console.log("Date: " + curDate + "  |  Current Date is: " + Date.parse(curDate));
                        //console.log("Date: " + element.sdate + " |  Min Date is: " + Date.parse(element.sdate));
                        //console.log("Date: " + element.edate + "  |  Max Date is: " + Date.parse(element.edate));
                        //console.log("");
                        //console.log(date);
                        dateStatus = false;
                    }

                })
                return [dateStatus];
                /*var minDateParts = minDate.split("/");
                var formatedMinDate = minDateParts[1] + "/" + minDateParts[2] + "/" + minDateParts[0];

                var maxDateParts = maxDate.split("/");
                var formatedMaxDate = maxDateParts[1] + "/" + maxDateParts[2] + "/" + maxDateParts[0];

                var dateParts = new Date(date).toISOString().split("T")[0].split("-");
                var curDate = dateParts[1] + "/" + dateParts[2] + "/" + dateParts[0];

                if (Date.parse(curDate) >= Date.parse(formatedMinDate) && Date.parse(curDate) <= Date.parse(formatedMaxDate)) {
                    console.log("1");
                    return [false];
                }
                else {
                    return [true];
                }*/
            }
        })
            .change(function () {
                var sDatePart = $('#startDate').val().split("-");
                var eDatePart = $('#endDate').val().split("-");
                var sDate = sDatePart[1] + "-" + sDatePart[0] + "-" + sDatePart[2];
                var eDate = eDatePart[1] + "-" + eDatePart[0] + "-" + eDatePart[2];

                if (sDate != "" || eDate != "") {
                    if (Date.parse(sDate) >= Date.parse(eDate) || sDate == "") {
                        $('#startDate').val(changeFormatRes(eDate));
                        $('#endDate').val("");
                    }
                }
            })
    }
    if ($(".changedDate").length) {
        $(".changedDate")
            .attr('min', getStartDate())
            .change(function () {
                var sDate = $('#startDate').val();
                var eDate = $('#endDate').val();
                if (eDate != "" || sDate != "") {
                    if (Date.parse(sDate) >= Date.parse(eDate) || sDate == "") {
                        $('#startDate').val(eDate);
                        $('#endDate').val("");
                    }
                }
                else {
                    var sDate = $('#startDate').val();
                    var eDate = $('#endDate').val();
                    if (eDate != "" || sDate != "") {
                        if (Date.parse(sDate) >= Date.parse(eDate) || sDate == "") {
                            $('#startDate').val(eDate);
                            $('#endDate').val("");
                        }
                    }
                }
            });
    }

    $('#btnMkRes').click(function () {
        var displayName = $('#displayName').val();
        var roomNum = $('#rooms').val();
        var price = $('#price').val();
        var sDate = $('#startDate').val();
        var eDate = $('#endDate').val();
        var RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();

        if (eDate != "" || sDate != "") {
            $.ajax({
                url: "/Apartments/ReservationExist",
                data: { sdate: sDate, edate: eDate }
            }).done(function (response) {            
                    var data = {
                        DisplayName: displayName,
                        RoomsNumber: roomNum,
                        Price: price,
                        sAvailability: sDate,
                        eAvailability: eDate,
                        __RequestVerificationToken: RequestVerificationToken
                    };

                    $.ajax({
                        url: '/Apartments/MakeReservation',
                        data: data,
                        type: "POST",
                        success: function (response) {
                            alert("Reservation have been made to: " + sDate + " - " + eDate);
                        },
                        error: function (xhr, ajaxOptions, throwError) {
                            alert("error");
                        }
                    })
            });
        }
    }).get("/Apartments/Index");
})

function getStartDate() {
    var today = new Date();
    var day = ("0" + today.getDate()).slice(-2);
    var month = ("0" + (today.getMonth() + 1)).slice(-2);
    //var thisDate = today.getFullYear() + "-" + (month) + "-" + (day);
    var thisDate = (day) + "/" + (month) + "/" + today.getFullYear();
    return thisDate;
}

/*function getResStartDate() {
    var displayName = $('#displayName');
    var result = null;
    $.ajax({
        async: false,
        url: "/Apartments/Search",
        data: { DisplayName: displayName.val() },
        success: function (response) {
            var date = response[0].sAvailability.split("T")[0].split("-");
            result = (date[2] + "-" + date[1] + "-" + date[0]);
        }
    });

    return result;
}

function getResEndDate() {
    var displayName = $('#displayName');
    var result = null;
    $.ajax({
        async: false,
        url: "/Apartments/Search",
        data: { DisplayName: displayName.val() },
        success: function (response) {
            var date = response[0].eAvailability.split("T")[0].split("-");
            result = (date[2] + "-" + date[1] + "-" + date[0]);
        }
    });
    return result;
}*/

function getResDate(date) {
    var displayName = $('#displayName');
    var result = null;
    $.ajax({
        async: false,
        url: "/Apartments/Search",
        data: { DisplayName: displayName.val() },
        success: function (response) {
            if (date == "startDate") {
                date = response[0].sAvailability.split("T")[0].split("-");
                result = (date[2] + "-" + date[1] + "-" + date[0]);
            }
            if (date == "endDate") {
                date = response[0].eAvailability.split("T")[0].split("-");
                result = (date[2] + "-" + date[1] + "-" + date[0]);
            }
        }
    });
    return result;
}

function changeFormatRes(date) {
    var dateParts = date.split("-");
    return dateParts[1] + "-" + dateParts[0] + "-" + dateParts[2];
}