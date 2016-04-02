var map;

var initMap = function () {

    var addresses = $("#mapAddresses").data("value");
    var mapElement = document.getElementById('map');
    var mapOptions = {
        zoom: 12,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        styles: [{ "featureType": "road", "elementType": "geometry", "stylers": [{ "lightness": 100 }, { "visibility": "simplified" }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "color": "#C6E2FF" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#C5E3BF" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "color": "#D1D1B8" }] }]
    }

    map = new google.maps.Map(mapElement, mapOptions);
    

    for (var i = 0; i < addresses.length; i++) {
        var myLatLng = new Object();

        myLatLng.lat = parseFloat(addresses[i].lat);
        myLatLng.lng = parseFloat(addresses[i].lng);
        addMarker(myLatLng, addresses[i]);

    }
    // Set map center to current location
    var initialLocation;

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            map.setCenter(initialLocation);
            var marker = new google.maps.Marker({
                map: map,
                position: initialLocation,
                title: 'מיקום נוכחי',
                zIndex: google.maps.Marker.MAX_ZINDEX + 1,
                icon: {
                    url: './Images/blue-marker.png',
                    scaledSize: new google.maps.Size(50, 50)
                }
            });

        }, function () {
        });
    }
        // Browser doesn't support Geolocation
    else {
        alert("Geolocation service failed.");
    }

}

var lastInfoWindow;

var addMarker = function (myLatLng, address) {

    var content = "<h4>" + address.name + "</h4>" +
        "<p><label>שעות פתיחה : </label> " + address.openingHours + "</p>" +
        "<p><label>תיאור : </label> " + address.description + "</p>" +
       // "<p><img src='data:image/png;base64," + address.image + "' alt='Red dot'></p>" + 
        "<p><label>סוג מטבח : </label> " + address.cuisine + "</p>" +
        "<p><label>קטגוריה : </label> " + address.category + "</p>" +
        "<p><label>כשרות : </label> " + address.kosher + "</p>" + // TODO : check if not empty
        "<p><label>טלפון : </label> " + address.phone + "</p>" +
        "<p><label>נגישות : </label> " + address.handicapAccessibility + "</p>"; // TODO

    var infoWindow = new google.maps.InfoWindow({
        content: content
    });

    var marker = new google.maps.Marker({
        id: address.id,
        map: map,
        position: myLatLng,
        title: address.name
    });

    marker.addListener('click', function () {

        if (lastInfoWindow !== undefined) {
            lastInfoWindow.close();
        }
        lastInfoWindow = infoWindow;


        $.get("/Home/GetImage", { id: marker.id },
            function (data) {
                infoWindow.setContent(infoWindow.getContent() + "<img style='width: 100px;height:50px;' src='data:image/png;base64," + data + "'>");
            });

        infoWindow.open(map, marker);
    });
};

$(document).ready(function () {
    initMap();

    //$("#searchBox").keyup(function (event) { TODO
    //
    //});
});