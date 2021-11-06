$(function () {
    $('.custSearch').click(function () {
        var firstName = $('#firstName');
        $.ajax({
            url: "/Customers/Search",
            data: { FirstName: firstName.val() }
        }).done(function (result) {
            var tbody = $('tbody');
            var template = $('#custTemplate').html();
            tbody.html('');
            $.each(result, function (key, value) {
                var temp = template;
                $.each(value, function (key, value) {
                    temp = temp.replaceAll('${' + key + '}', value);
                });
                tbody.append(temp);
            });
        });
    });
});