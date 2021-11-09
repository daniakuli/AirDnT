$(function () {
    $(document).click(function (event) {
        if ($(event.target).attr('type') != "submit") {
            event.preventDefault();
        }
    })
});