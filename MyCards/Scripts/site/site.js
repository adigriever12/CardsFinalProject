﻿var map;
var markers = [];

var currentLocation;

var intializeMap = function () {
    var mapElement = document.getElementById('map');

    var mapOptions = {
        zoom: 12,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        styles: [{ "featureType": "road", "elementType": "geometry", "stylers": [{ "lightness": 100 }, { "visibility": "simplified" }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "color": "#C6E2FF" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#C5E3BF" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "color": "#D1D1B8" }] }]
    }

    map = new google.maps.Map(mapElement, mapOptions);
}

var intializMarkers = function () {
    var restaurants = $("#mapAddresses").data("value");

    for (var i = 0; i < restaurants.length; i++) {
        var myLatLng = new Object();

        myLatLng.lat = parseFloat(restaurants[i].lat);
        myLatLng.lng = parseFloat(restaurants[i].lng);
        addMarker(myLatLng, restaurants[i]);
    }
};

var setCurrentLocation = function () {
    // Set map center to current location
    var initialLocation;

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {

           
            $("#load").load("Home/_List", { lat: position.coords.latitude, lng: position.coords.longitude }, function () {
                intializeMap();
                intializMarkers();
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
            });

            

        }, function () {
        });
    }
        // Browser doesn't support Geolocation
    else {
        alert("Geolocation service failed.");
    }
};

var initMap = function () {
    //intializeMap();
    setCurrentLocation();
}

var lastInfoWindow;

var chooseMarker = function (marker, infoWindow) {
    if (lastInfoWindow !== undefined) {
        lastInfoWindow.close();
    }

    lastInfoWindow = infoWindow;

    if (infoWindow.getContent().indexOf("img") == -1) {
        $.get("/Home/GetImage", { id: marker.id },
            function (data) {
                infoWindow.setContent(infoWindow.getContent() + "<img style='width: 100px;height:50px;' src='data:image/png;base64," + data + "'>");
            });
    }

    infoWindow.open(map, marker);
};

var addMarker = function (myLatLng, restaurant) {

    var accessability = restaurant.handicapAccessibility ? "קיימת" : "לא קיימת";
    var kosher = restaurant.kosher != "" ? "<p><label>כשרות : </label> " + restaurant.kosher + "</p>" : "";

    var content = "<h4>" + restaurant.name + "</h4>" +
        "<p><label>שעות פתיחה : </label> " + restaurant.openingHours + "</p>" +
        "<p><label>תיאור : </label> " + restaurant.description + "</p>" +
       // "<p><img src='data:image/png;base64," + restaurant.image + "' alt='Red dot'></p>" + 
        "<p><label>סוג מטבח : </label> " + restaurant.cuisine + "</p>" +
        "<p><label>קטגוריה : </label> " + restaurant.category + "</p>" +
        kosher +
        "<p><label>טלפון : </label> " + restaurant.phone + "</p>" +
        "<p><label>כתובת : </label> " + restaurant.address + "</p>" +
        "<p><label>נגישות : </label> " + accessability + "</p>"; // TODO

    var infoWindow = new google.maps.InfoWindow({
        content: content
    });

    var marker = new google.maps.Marker({
        id: restaurant.id,
        map: map,
        position: myLatLng,
        title: restaurant.name,
        icon: {
            url: './Images/' + restaurant.score + '.png',
            //scaledSize: new google.maps.Size(50, 50)
        }
    });

    marker.addListener('click', function () {
        chooseMarker(marker, infoWindow);
    });

    markers.push({
        id: restaurant.id,
        marker: marker,
        info: infoWindow
    });
};

var findMarkerById = function (id) {
    for (var i = 0; i < markers.length; i++) {
        if (markers[i].id == id) {
            return markers[i];
        }
    }
};

var listListen = function () {
    $(".list-group-item").click(function () {

        var clickedItem = $(event.target).first();
        // Check if not the "been" btn then open the details
        if ((!clickedItem.hasClass('green')) && (!clickedItem.hasClass('grey')))
        {
            var id = $(this).find('.hidden-id').attr('value');
            var marker = findMarkerById(id);
            var infoWindow = marker.info;

            chooseMarker(marker.marker, marker.info);
        }
    });
};

var searchKeyup = function() {
    $("#searchBox").keyup(function () {

        var searchedContent = $("#searchBox").val().toLowerCase();

        if (searchedContent != "") {
            $.each($(".list-group-item"), function (i, val) {
                var name = $(this).find('.item-name').text().toLowerCase();

                if (name.indexOf(searchedContent) == -1) {
                    $(this).attr('class', 'list-group-item hidden');
                }
                else {
                    $(this).attr('class', 'list-group-item');
                }
            });
        }
    });
};

var filtersChanged = function () {
    $('#accessibility-toggle').change(function () {
        var isChecked = $(this).prop('checked').toString();

        $.each($(".list-group-item"), function (i, val) {
            var accessibility = $(this).find('.hidden-accessibility').attr('value').toLowerCase();

            if (accessibility != isChecked) {
                $(this).attr('class', 'list-group-item hidden');
            }
            else {
                $(this).attr('class', 'list-group-item');
            }

        });
    });

    $('#kosher-toggle').change(function () {
        var isChecked = $(this).prop('checked');

        $.each($(".list-group-item"), function (i, val) {
            var kosher = $(this).find('.hidden-kosher').attr('value').toLowerCase();

            if (kosher == "" && isChecked) {
                $(this).attr('class', 'list-group-item hidden');
            }
            else {
                $(this).attr('class', 'list-group-item');
            }

        });
    });
};

var recommendedClick = function () {
    $(".recommended-items").click(function () {
        var id = $(this).children(".recommended-ids.hidden").text();
        var marker = findMarkerById(id);
        var infoWindow = marker.info;
        chooseMarker(marker.marker, marker.info);
    });
};

var selectRanking = function () {
    $('.inputStar').on('rating.change', function (event, value, caption) {

        var ratingverageText = $(this).closest("a").children(".ratingAvg");
        
        // Save ranking in db
        $.post("/Home/UpdateRank", { rank: value, restuarantId: $(this).closest("a").children(".hidden-id").attr('value') },
            function (data) {
                ratingverageText[0].innerHTML = data + "/5";
                });

        // Change been color to green
        var beenBtn = $(this).closest(".dropdown").children(".glyphicon-ok");
        beenBtn.removeClass("grey");
        beenBtn.addClass("green");

        // Close dropdown
        $(this).closest(".dropdown").removeClass("open");
        
    });
};

$(document).ready(function () {
    listListen();
    searchKeyup();
    filtersChanged();
    recommendedClick();
    selectRanking();
});