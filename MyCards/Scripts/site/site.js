var map;
var markers = [];

var currentLocation;
var geocoder;

var cardType;

// set map intialization configuration
var intializeMap = function () {
    var mapElement = document.getElementById('map');

    var mapOptions = {
        zoom: 12,
        disableDefaultUI: true,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        styles: [{ "featureType": "road", "elementType": "geometry", "stylers": [{ "lightness": 100 }, { "visibility": "simplified" }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "color": "#C6E2FF" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#C5E3BF" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "color": "#D1D1B8" }] }]
    };

    // creating map object
    map = new google.maps.Map(mapElement, mapOptions);
    geocoder = new google.maps.Geocoder();
}

// add marker to each resturant. Restauratns data is calculated in HomeController, and parsed here
var intializMarkers = function () {
    var restaurants = $("#mapAddresses").data("value");

    // iterating all restaurants
    for (var i = 0; i < restaurants.length; i++) {
        var myLatLng = new Object();

        myLatLng.lat = parseFloat(restaurants[i].lat);
        myLatLng.lng = parseFloat(restaurants[i].lng);
        addMarker(myLatLng, restaurants[i]);
    }
};

// determining user current location, and setting in on map
var setCurrentLocation = function () {
    // Set map center to current location
    var initialLocation;

    if (navigator.geolocation) {
        // get the user current position
        navigator.geolocation.getCurrentPosition(function (position) {

            // builiding current location object for lat and lng
            currentLocation = {lat: position.coords.latitude, lng: position.coords.longitude};
            initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

            // focusing map on current location
            map.setCenter(initialLocation);

            // add a blue marker to current location
            var marker = new google.maps.Marker({
                map: map,
                position: initialLocation,
                title: 'מיקום נוכחי',
                zIndex: google.maps.Marker.MAX_ZINDEX + 1,
                icon: {
                    url: '../Images/blue-marker.png',
                    scaledSize: new google.maps.Size(50, 50)
                }
            });

        }, function (error) {
            console.log("error calculating user current position");
        });
    }
        // Browser doesn't support Geolocation
    else {
        alert("Geolocation service failed.");
    }
};

var initMap = function () {
    intializeMap();
    setCurrentLocation();
    intializMarkers();
}

var lastInfoWindow;

// handling event of clicking a marker
var chooseMarker = function (marker, infoWindow) {
    // closing the last opened info window in order to open only the new one
    if (lastInfoWindow !== undefined) {
        lastInfoWindow.close();
    }

    // saving current clicked markers info window
    lastInfoWindow = infoWindow;

    // gets the marker image
    if (infoWindow.getContent().indexOf("data:image") == -1) {
        $.get("/Home/GetImage", { id: marker.id, restaurantType: cardType },
            function (data) {
                infoWindow.setContent(infoWindow.getContent().replace("src=''", "src='data:image/png;base64," + data + "'"));
            });
    }

    // opening marker info window with it's image
    infoWindow.open(map, marker);
};

// returning current card type by url
var getCardType = function () {

    if (window.location.href.search('Groupon') != -1) {
        return 1;
    }
    if (window.location.href.search('Leumi') != -1) {
        return 2;
    }
    if (window.location.href.search('American') != -1) {
        return 3;
    }
    return 0;
}

var createContentP = function (text, value) {
    return value != "" ? "<p><label>" + text +" : </label> " + value + "</p>" : "";
}

// creating marker info window
var markerContent = function (restaurant) {

    var accessability = restaurant.handicapAccessibility ? "קיימת" : "לא קיימת";
    var content = "";

    // Check card
    if (cardType == 1) {
        content = "<span style='font-size:23px;font-weight:bold;margin-left:8px;'>" + restaurant.name + "</span>" +
                  "<img style='border:0;float:left;width: 100px;height:50px;' src=''><br><br><br>" +
                  createContentP("שעות פתיחה", restaurant.openingHours) +
                  createContentP("תיאור", restaurant.description) +
                  createContentP("תיאור קופון", restaurant.copunDescription) +
                  createContentP("קטגוריה", restaurant.category) +
                  createContentP("כשרות", restaurant.kosher) +
                  createContentP("טלפון", restaurant.phone) +
                  createContentP("כתובת", restaurant.address) +
                  createContentP("הגבלות", restaurant.expiration);
    }
    else if (cardType == 3) {
        content = "<span style='font-size:23px;font-weight:bold;margin-left:8px;'>" + restaurant.name + "</span>" +
                  "<img style='border:0;float:left;width: 100px;height:50px;' src=''><br><br><br>" +
                  createContentP("תיאור", restaurant.description) +
                  createContentP("תיאור קופון", restaurant.copunDescription) +
                  createContentP("טלפון", restaurant.phone) +
                  createContentP("כתובת", restaurant.address);
    }
    else if (cardType == 2) {
        content = "<span style='font-size:23px;font-weight:bold;margin-left:8px;'>" + restaurant.name + "</span>" +
                  "<img style='border:0;float:left;width: 100px;height:50px;' src=''><br><br><br>" +
                  createContentP("תיאור", restaurant.description) +
                  createContentP("תיאור קופון", restaurant.copunDescription) +
                  createContentP("טלפון", restaurant.phone) +
                  createContentP("כתובת", restaurant.address);
    }
    else if (cardType == 0) {
        content = "<span style='font-size:23px;font-weight:bold;margin-left:8px;'>" + restaurant.name + "</span>" +
                  "<img style='border:0;float:left;width: 100px;height:50px;' src=''><br><br><br>" +
                  createContentP("שעות פתיחה", restaurant.openingHours) +
                  createContentP("תיאור", restaurant.description) +
                  createContentP("סוג מטבח", restaurant.cuisine) +
                  createContentP("קטגוריה", restaurant.category) +
                  createContentP("כשרות", restaurant.kosher) +
                  createContentP("טלפון", restaurant.phone) +
                  createContentP("כתובת", restaurant.address) +
                  "<p><label>נגישות : </label> " + accessability + "</p>";
    }
    return content;
}

// create a Gmap marker object by given position
var addMarker = function (myLatLng, restaurant) {

    // creating marker content
    var content = markerContent(restaurant);
    
    // creating marker pop-up info window
    var infoWindow = new google.maps.InfoWindow({
        content: content,
        maxWidth: 450
    });

    var marker = new google.maps.Marker({
        id: restaurant.id,
        map: map,
        position: myLatLng,
        title: restaurant.name,
        icon: {
            url: '../Images/' + restaurant.score + '.png',
            scaledSize: new google.maps.Size(50, 50)
        }
    });

    // add click listen event to marker
    marker.addListener('click', function () {
        chooseMarker(marker, infoWindow);
    });

    // saving marker to an array containing all markers
    markers.push({
        id: restaurant.id,
        marker: marker,
        info: infoWindow
    });
};

// get marker by given id from markers array
var findMarkerById = function (id) {
    for (var i = 0; i < markers.length; i++) {
        if (markers[i].id == id) {
            return markers[i];
        }
    }
};

// listen to click event on list
var listListen = function () {
    $(".list-group-item").click(function () {

        var clickedItem = $(event.target).first();

        // Check if the clicked button was not the "rakning", then open the details
        if ((!clickedItem.hasClass('green')) && (!clickedItem.hasClass('grey')) && (!clickedItem.hasClass('c-rating__item')))
        {
            var id = $(this).find('.hidden-id').attr('value');
            var marker = findMarkerById(id);
            var infoWindow = marker.info;

            // open restaurant info window in map view
            chooseMarker(marker.marker, marker.info);
        }

        $("map").focus();
    });
};

// listen to key up event and filter list by typed letters
var searchKeyup = function () {
    $("#searchBox").keyup(function () {

        // get typed content
        var searchedContent = $("#searchBox").val().toLowerCase();

        // for each restaurant in the list
        $.each($(".list-group-item"), function (i, val) {

            // get the restaurant name
            var name = $(this).find('.item-name').text().toLowerCase();

            // in case the restuarnt name does not contain the typed letters
            if (name.indexOf(searchedContent) == -1) {
                $(this).attr('class', 'list-group-item hidden');
            }
            else {
                $(this).attr('class', 'list-group-item');
            }
        });
    });
};

// functionlity of toggles being selected
var filterByToogles = function () {
    var isAccessabilityChecked = $("#accessibility-toggle").prop('checked');
    var isKosherChecked = $("#kosher-toggle").prop('checked');

    $.each($(".list-group-item"), function (i, val) {
        var accessibility = $(this).find('.hidden-accessibility').attr('value').toLowerCase();
        var kosher = $(this).find('.hidden-kosher').attr('value').toLowerCase();

        if (isAccessabilityChecked && !isKosherChecked) {
            if (accessibility != isAccessabilityChecked.toString()) {
                $(this).attr('class', 'list-group-item hidden');
            } else {
                $(this).attr('class', 'list-group-item');
            }
        } else if (!isAccessabilityChecked && isKosherChecked) {
            if (kosher == "") {
                $(this).attr('class', 'list-group-item hidden');
            } else {
                $(this).attr('class', 'list-group-item');
            }
        } else if (isAccessabilityChecked && isKosherChecked) {
            if (accessibility != isAccessabilityChecked.toString() || kosher == "") {
                $(this).attr('class', 'list-group-item hidden');
            } else {
                $(this).attr('class', 'list-group-item');
            }
        } else {
            $(this).attr('class', 'list-group-item');
        }

    });
}

var filtersChanged = function () {
    $('#accessibility-toggle').change(function () {
        filterByToogles();
    });

    $('#kosher-toggle').change(function () {
        filterByToogles();
    });
};

// event click on new recommended restaurant
var recommendedClick = function () {
    $(".recommended-items").click(function () {
        var id = $(this).children(".recommended-ids.hidden").text();

        // find the restaurants marker in the map
        var marker = findMarkerById(id);
        var infoWindow = marker.info;

        // open the marker of the clicked restaurant in the map
        chooseMarker(marker.marker, marker.info);
    });
};

var ratings = function () {
    $(".c-rating").each(function () {

        var self = $(this);

        // callback to run after setting the rating
        var callback = function (rating) {
            var ratingverageText = self.closest("a").children(".ratingAvg");

            // Save ranking in db
            $.post("/Home/UpdateRank", { rank: rating, restuarantId: self.closest("a").children(".hidden-id").attr('value'), restaurantType: cardType },
                function (data) {
                    ratingverageText[0].innerHTML = data + "/5";
                });

            // Change been color to green
            var beenBtn = self.closest(".dropdown").children(".glyphicon-ok");
            beenBtn.removeClass("grey");
            beenBtn.addClass("green");

            // Close dropdown
            self.closest(".dropdown").removeClass("open");
        };

        var myRatingStars = self.closest("a").children('.hidden-rating')[0].getAttribute('value');
        
        // rating instance
        var myRating = rating($(this)[0], myRatingStars, 5, callback);
    });
};

var locationFilters = function () {

    $("#location-address-filter").click(function (e) {
        // stop dropdown from closing
        e.stopPropagation();
    });

    $("#location-address-filter").keyup(function (e) {
        // pressed enter
        if (e.keyCode == 13) {

            // focusing map on typed address
            geocoder.geocode({ 'address': $("#location-address-filter").val() }, function (results, status) {
                if (status === google.maps.GeocoderStatus.OK) {

                    // centering map on chosen location
                    map.setCenter(results[0].geometry.location);
                    
                    // clearing all markers, the keeping only onces within 5 kilometer raduis
                    removeMarkersByDistance(results[0].geometry.location.lat(), results[0].geometry.location.lng());
                } else {
                    alert('Geocode was not successful for the following reason: ' + status);
                }
            });
        }
    });

    // event for current location button click
    $("#currentlocation-filter").click(function () {

        // in case the button is pressed
        if ($(this).attr('class').indexOf('active') == -1) {
            removeMarkersByDistance(currentLocation.lat, currentLocation.lng);
            map.setCenter(currentLocation);
        } else {
            makeAllMarkersVisible();
        }
    });

    // clicking the cancel button
    $("#clear-location-filter").click(function () {
        makeAllMarkersVisible();
    });

    // address filtering for mobile
    $("#mobileSearchAdd").click(function () {
        geocoder.geocode({ 'address': $("#location-address-filter").val() }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);

                removeMarkersByDistance(results[0].geometry.location.lat(), results[0].geometry.location.lng());
            } else {
                alert('Geocode was not successful for the following reason: ' + status);
            }
        });
    });
};

var makeAllMarkersVisible = function () {
    for (var i = 0; i < markers.length; i++) {
        markers[i].marker.setVisible(true);
    }

    $.each($(".list-group-item"), function (i, val) {
        $(this).attr('class', 'list-group-item');
    });
};

var removeMarkersByDistance = function (lat, lng) {

    var ids = [];

    for (var i = 0; i < markers.length; i++) {
        var distance = google.maps.geometry.spherical.computeDistanceBetween(
        new google.maps.LatLng(markers[i].marker.position.lat(), markers[i].marker.position.lng()),
        new google.maps.LatLng(lat, lng));

        if (distance / 1000 <= 5) {
            markers[i].marker.setVisible(true);

            ids.push(markers[i].id);
        } else {
            markers[i].marker.setVisible(false);
        }
    }

    $.each($(".list-group-item"), function (i, val) {
        var id = Number($(this).find('.hidden-id').attr('value'));

        if (ids.indexOf(id) == -1) {
            $(this).attr('class', 'list-group-item hidden');
        }
        else {
            $(this).attr('class', 'list-group-item');
        }
    });
};

// altering css for mobile
var mobileDisplay = function ()
{
    // Check if mobile
    if (document.getElementById('mobile') != null)
    {
        $('#side').css('visibility', 'visible');
        $('#map').css('height', window.innerHeight + 'px');
        $('#map').css('width', window.innerWidth + 'px');
        $('#map').css('order', '');
    }

    $(window).on("orientationchange", function () {
        location.reload();
    });
}

// set selected card image
var selectCard = function (card) {

    if (document.getElementById('mobile') == null) {

        switch (card) {
            case 0:
                {
                    $("#selectedCard").attr("src", "../Images/hever.jpg");
                    break;
                }
            case 1:
                {
                    $("#selectedCard").attr("src", "../Images/groupon.png");
                    break;
                }
            case 2:
                {
                    $("#selectedCard").attr("src", "../Images/laumi.jpg");
                    break;
                }
            case 3:
                {
                    $("#selectedCard").attr("src", "../Images/american.png");
                    break;
                }
            default:

        }
    }
    else {
        $.each($("#cards .image_cards"), function (i, curr) {
            $(curr).attr('class', 'image_cards');
        });

        $($("#cards .image_cards")[card]).attr('class', 'image_cards selected_mobile');

    }
}

// change card image when clicking on a different card
var changeCardEvent = function () {

    $("#hever").click(function () {
        $("#selectedCard").attr("src", "../Images/hever.jpg");
    });

    $("#leumi").click(function () {
        $("#selectedCard").attr("src", "../Images/laumi.jpg");
    });

    $("#american").click(function () {
        $("#selectedCard").attr("src", "../Images/american.png");
    });

    $("#groupon").click(function () {
        $("#selectedCard").attr("src", "../Images/groupon.png");
    });
}

$(document).ready(function () {
    cardType = getCardType();

    initMap();
    selectCard(cardType);
    listListen();
    searchKeyup();
    filtersChanged();
    recommendedClick();
    ratings();
    locationFilters();
    changeCardEvent();
    mobileDisplay();
});