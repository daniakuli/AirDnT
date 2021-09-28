$(function () {
    $('.btnSearch').click(function () {
        var displayName = $('#displayName');
        $.ajax({
            url: "/Apartments/Search",
            data: { DisplayName: displayName.val() }
        }).done(function (result) {
           
        });
    });
});