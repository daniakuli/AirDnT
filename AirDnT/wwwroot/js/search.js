$(function () {
    $('.btnSearch').click(function () {
        var displayName = $('#displayName');
        $.ajax({
            url: "/Apartments/Search",
            data: { DisplayName: displayName.val() }
        }).done(function (result) {
            var body = $('#apartments');
            var template = $('#template').html();
            body.html('');
            $.each(result, function (key, value) {
                var temp = template;
                $.each(value, function (key, value) {
                    temp = temp.replaceAll('${' + key + '}', value);
                });
                body.append(temp);
            });
        });
    });

    $('.advBtnSearch').click(function () {
        $('.SearchBtns').css("display", "none");
        $('.AdvSearchOpt').css("display", "block");
    });

    $('.pSearch').click(function () {
        $('.AdvSearchOpt').css("display", "none");
        $('.FirstAdvSearch').css("display", "inline-block");
    });

    $('.cSearch').click(function () {
        $('.AdvSearchOpt').css("display", "none");
        $('.SecondAdvSearch').css("display", "inline-block");
    });

    $('.pFilter').click(function () {
        var rooms = $('#roomsNum');
        var price = $('#price');
        var strDate = $('#sDate');
        var endDate = $('#eDate');

        $.ajax({
            url: "/Apartments/PriceAdvSearch",
            data: {
                RoomsNumber: rooms.val(),
                Price: price.val(),
                sAvailability: strDate.val(),
                eAvailability: endDate.val()
            }
        }).done(function (result) {
            var body = $('#apartments');
            var template = $('#template').html();
            body.html('');
            $.each(result, function (key, value) {
                var temp = template;
                $.each(value, function (key, value) {
                    temp = temp.replaceAll('${' + key + '}', value);
                });
                body.append(temp);
            });
        });
    });

    $('.cFilter').click(function () {
        var country = $('#country');
        var city = $('#city');
        var strDate = $('#startDate');
        var endDate = $('#endDate');

        $.ajax({
            url: "/Apartments/CountryAdvSearch",
            data: {
                Country: country.val(),
                City: city.val(),
                sAvailability: strDate.val(),
                eAvailability: endDate.val()
            }
        }).done(function (result) {
            var body = $('#apartments');
            var template = $('#template').html();
            body.html('');
            $.each(result, function (key, value) {
                var temp = template;
                $.each(value, function (key, value) {
                    temp = temp.replaceAll('${' + key + '}', value);
                });
                body.append(temp);
            });
        });
    });
});