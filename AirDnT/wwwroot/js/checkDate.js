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
                    console.log()
                    resStartDate = new Date(response[resLen].sAvailability);
                    resEndDate = new Date(response[resLen].eAvailability);
                    result.push({ sdate: resStartDate, edate: resEndDate });
                }
            }
        });
        $(".resChangedDate").datepicker({
            dateFormat: "dd-mm-yy",
            minDate: minDate,
            maxDate: maxDate,
            beforeShowDay: function (date) {
                var today = new Date();
                if (date.getTime() < today.getTime()) {
                    return [false];
                }
                var dateStatus = true;
                $(result).each(function (index, element) {
                    if (date.getTime() >= element.sdate.getTime() && date.getTime() <= element.edate.getTime()) {
                        dateStatus = false;
                    }
                })
                return [dateStatus];
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
                        console.log(changeFormat(eDate));
                        $('#startDate').val(changeFormat(eDate));
                        $('#endDate').val("");
                    }
                }
                else {
                    var sDate = $('#startDate').val();
                    var eDate = $('#endDate').val();
                    if (eDate != "" || sDate != "") {
                        if (Date.parse(sDate) >= Date.parse(eDate) || sDate == "") {
                            console.log(changeFormat(eDate));
                            $('#startDate').val(changeFormat(eDate));
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

        var data = {
            DisplayName: displayName,
            RoomsNumber: roomNum,
            Price: price,
            sAvailability: sDate,
            eAvailability: eDate,
            __RequestVerificationToken: RequestVerificationToken
        };

        if (eDate != "" || sDate != "") {
            $.ajax({
                url: '/Apartments/MakeReservation',
                data: data,
                type: "POST",
                success: function (response) {
                    alert("Reservation have been made from: " + sDate + " to: " + eDate);
                },
                error: function (xhr, ajaxOptions, throwError) {
                    alert("error");
                }
            });
        }
        location.href("/Apartments/Index")
    })
})

function getStartDate() {
    var today = new Date();
    var thisDate = today.toISOString().split("T")[0];
    return thisDate;
}


function getResDate(date) {
    var displayName = $('#displayName');
    var result = null;
    $.ajax({
        async: false,
        url: "/Apartments/Search",
        data: { DisplayName: displayName.val() },
        success: function (response) {
            var apartment;
            for (var i = 0; i < response.length; i++) {
                if (displayName.val().localeCompare(response[i].apartment.displayName) == 0) {
                    apartment = response[i].apartment;
                }
            }
            if (date == "startDate") {
                date = apartment.sAvailability.split("T")[0].split("-");
                result = (date[2] + "-" + date[1] + "-" + date[0]);
            }
            if (date == "endDate") {
                date = apartment.eAvailability.split("T")[0].split("-");
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