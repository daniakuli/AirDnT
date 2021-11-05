$(function(){
    if ($("#OwnerId").length) {
        $.ajax({
            url: "/Owners/DelCheck",
            data: { OID: $("#OwnerId").val() }
        }).done(function (response) {
            if (response.length) {
                $("#delBtn").prop('disabled', true);
                $("#warTxt").css('display', 'block');
            }
        });
    }
    if ($("#Id").length) {
        $.ajax({
            url: "/Customers/DelCheck",
            data: { CID: $("#Id").val() }
        }).done(function (response) {
            if (response.length) {
                $("#delBtn").prop('disabled', true);
                $("#warTxt").css('display', 'block');
            }
        });
    }
    if ($("#ApartmentId").length) {
        $.ajax({
            url: "/Apartments/DelCheck",
            data: { AID: $("#ApartmentId").val() }
        }).done(function (response) {
            if (response.length) {
                $("#delBtn").prop('disabled', true);
                $("#warTxt").css('display', 'block');
            }
        });
    }
})