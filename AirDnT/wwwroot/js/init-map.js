// Initialize and add the map
function initMap() {
    var country = document.getElementById("curCountry").innerHTML.replace(/ /g, "");
    var city = document.getElementById("curCity").innerHTML.replace(/ /g, "");
    var street = document.getElementById("curStreet").innerHTML.replace(/ /g, "");

    var locAddr = street + " " + city + " " + country;

    locAddr.replace(/ /g, "");

    var loc = {
        "url": "https://geocode.search.hereapi.com/v1/geocode?q=" + locAddr + "&apikey=9RvHUis8568ZIyzEa6srJYep70nqSLNSympIczpqvso",
        "method": "GET",
        "timeout": 0,
    };

    $.ajax(loc).done(function (response) {
        lat = response.items[0].position.lat;
        lng = response.items[0].position.lng;

        // The location of Uluru
        const uluru = { lat: lat, lng: lng};
        // The map, centered at Uluru
        const map = new google.maps.Map(document.getElementById("map"), {
            zoom: 12,
            center: uluru,
        });
        // The marker, positioned at Uluru
        const marker = new google.maps.Marker({
            position: uluru,
            map: map,
        });
    });
}