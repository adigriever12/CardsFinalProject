var map;
var markers = [];

var currentLocation;
var geocoder;

var cardType;

var intializeMap = function () {
    var mapElement = document.getElementById('map');

    var mapOptions = {
        zoom: 12,
        disableDefaultUI: true,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        styles: [{ "featureType": "road", "elementType": "geometry", "stylers": [{ "lightness": 100 }, { "visibility": "simplified" }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "color": "#C6E2FF" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#C5E3BF" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "color": "#D1D1B8" }] }]
    };

    map = new google.maps.Map(mapElement, mapOptions);
    geocoder = new google.maps.Geocoder();
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

           
            //$("#load").load("Home/_List", { lat: position.coords.latitude, lng: position.coords.longitude }, function () {
                //intializeMap();
            //intializMarkers();
            currentLocation = {lat: position.coords.latitude, lng: position.coords.longitude};
                initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                map.setCenter(initialLocation);
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
            //});

            

        }, function () {
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

var chooseMarker = function (marker, infoWindow) {
    if (lastInfoWindow !== undefined) {
        lastInfoWindow.close();
    }

    lastInfoWindow = infoWindow;

    if (infoWindow.getContent().indexOf("img") == -1) {
        $.get("/Home/GetImage", { id: marker.id },
            function (data) {
                infoWindow.setContent("<span style='font-size:25px;font-weight:bold;'>" + marker.title + "</span>" + "<img style='float:left;width: 100px;height:50px;' src='data:image/png;base64," + data + "'><br><br>" + infoWindow.getContent());
            });
    }

    infoWindow.open(map, marker);
};

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

var markerContent = function (restaurant) {

    var accessability = restaurant.handicapAccessibility ? "קיימת" : "לא קיימת";
    var content = "";

    // Check card
    if (cardType == 1) {
        content = 
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

    }
    else if (cardType == 2) {
        content = 
                  createContentP("תיאור", restaurant.description) +
                  createContentP("תיאור קופון", restaurant.copunDescription) +
                  createContentP("טלפון", restaurant.phone) +
                  createContentP("כתובת", restaurant.address);
    }
    else if (cardType == 0) {
        content = 
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
var addMarker = function (myLatLng, restaurant) {

    var content = markerContent(restaurant);
    
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
        if ((!clickedItem.hasClass('green')) && (!clickedItem.hasClass('grey')) && (!clickedItem.hasClass('c-rating__item')))
        {
            var id = $(this).find('.hidden-id').attr('value');
            var marker = findMarkerById(id);
            var infoWindow = marker.info;

            chooseMarker(marker.marker, marker.info);
        }

        $("map").focus();
    });
};

var searchKeyup = function() {
    $("#searchBox").keyup(function () {

        var searchedContent = $("#searchBox").val().toLowerCase();

        //if (searchedContent != "") {
            $.each($(".list-group-item"), function (i, val) {
                var name = $(this).find('.item-name').text().toLowerCase();

                if (name.indexOf(searchedContent) == -1) {
                    $(this).attr('class', 'list-group-item hidden');
                }
                else {
                    $(this).attr('class', 'list-group-item');
                }
            });
        //}
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
        $.post("/Home/UpdateRank", { rank: value, restuarantId: $(this).closest("a").children(".hidden-id").attr('value'), restaurantType: cardType },
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
        e.stopPropagation();
    });

    $("#location-address-filter").keyup(function (e) {
        // pressed enter
        if (e.keyCode == 13) {

            geocoder.geocode({ 'address': $("#location-address-filter").val() }, function (results, status) {
                if (status === google.maps.GeocoderStatus.OK) {
                    map.setCenter(results[0].geometry.location);
                    
                    removeMarkersByDistance(results[0].geometry.location.lat(), results[0].geometry.location.lng());
                } else {
                    alert('Geocode was not successful for the following reason: ' + status);
                }
            });
        }
    });

    $("#currentlocation-filter").click(function () {

        if ($(this).attr('class').indexOf('active') == -1) {
            removeMarkersByDistance(currentLocation.lat, currentLocation.lng);
            map.setCenter(currentLocation);
        } else {
            makeAllMarkersVisible();
        }
    });

    $("#clear-location-filter").click(function () {
        makeAllMarkersVisible();
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
}

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

}
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
    selectRanking();
    ratings();
    locationFilters();
    changeCardEvent();
    mobileDisplay();

});