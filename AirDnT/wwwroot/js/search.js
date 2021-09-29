$(function () {
    $('.btnSearch').click(function () {
        var displayName = $('#displayName');
        $.ajax({
            url: "/Apartments/Search",
            data: { DisplayName: displayName.val() }
        }).done(function (result) {
            var tbody = $('tbody');
            var template = $('#template').html();
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