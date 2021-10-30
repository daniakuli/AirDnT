$(function () {

    var today = new Date();

    var day = ("0" + today.getDate()).slice(-2);
    var month = ("0" + (today.getMonth() + 1)).slice(-2);

    var thisDate = today.getFullYear() + "-" + (month) + "-" + (day);

    $(".changedDate").attr('min', thisDate).change(function () {
        var sDate = document.getElementById("startDate").value;
        var eDate = document.getElementById("endDate").value;
        if (eDate != "" || sDate != "") {
            if (Date.parse(sDate) >= Date.parse(eDate) || sDate == "") {
                document.getElementById("startDate").value = eDate;
                document.getElementById("endDate").value = "";
            }
        }
        else {
            var sDate = document.getElementById("sDate").value;
            var eDate = document.getElementById("eDate").value;
            if (eDate != "" || sDate != "") {
                if (Date.parse(sDate) >= Date.parse(eDate) || sDate == "") {
                    document.getElementById("sDate").value = eDate;
                    document.getElementById("eDate").value = "";
                }
            }
        }
    });
})