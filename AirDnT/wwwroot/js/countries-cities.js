    var countries = {
        "url": "https://countriesnow.space/api/v0.1/countries",
        "method": "GET",
        "timeout": 0,
    };

    $.ajax(countries).done(function (response) {
        var num = 0;
        var listCountries = [];
        while (num < response.data.length) {
            listCountries.push(response.data[num]["country"]);
            num++;
        }

        $("#country").autocomplete({
            source: listCountries,
        });
    });

$("#country").on('autocompletechange', function () {
    console.log(this.value);
    var body = {
        "country": this.value
    };

    var cities = {
        "url": "https://countriesnow.space/api/v0.1/countries/cities",
        "method": "POST",
        "contentType": "application/json; charset=utf-8",
        "timeout": 0,
        "data": JSON.stringify(body)
    };

    $.ajax(cities).done(function (response) {
        $("#city").autocomplete({
            source: response.data,
        });
    })
        .fail(function (response) {
            console.log(response);
        });
}).change();
