$(function () {
    $.molimentum = {};

    $.molimentum.loadPositions = function (map, options) {
        var markerIcon = new google.maps.MarkerImage(
            "https://chart.googleapis.com/chart?chst=d_simple_text_icon_below&chld=%7C12%7Cff0000%7Clocation%7C12%7Cff0000");

        var lastMarkerIcon = new google.maps.MarkerImage(
            "https://chart.googleapis.com/chart?chst=d_simple_text_icon_below&chld=%7C12%7Cff0000%7Csail%7C24%7C005544");

        var first = true;

        var latLngBounds = new google.maps.LatLngBounds();
        var path = new Array();

        var requestUrl;

        if (options.from != null && options.to != null) {
            requestUrl = "/positionreports/listbyperiod?" +
                "from=" + options.from +
                "&to=" + options.to +
                ((options.pageSize != null) ? "&pageSize=" + options.pageSize : "") +
                "&page=";
        } else {
            requestUrl =
                "/positionreports/list?" +
                ((options.pageSize != null) ? "pageSize=" + options.pageSize + "&" : "") +
                "page=";
        }

        var loadPositionsRecursive = function (options) {
            $.ajax({
                url: requestUrl + options.page,
                dataType: "text",
                success: function (rawData) {
                    var data = $.parseJSON(rawData, true);
                    $.each(data.Items, function (key, item) {
                        var latLng = new google.maps.LatLng(item.Position.Latitude, item.Position.Longitude);
                        latLngBounds.extend(latLng);

                        path.push(latLng);

                        new google.maps.Marker({
                            position: latLng,
                            map: map,
                            title: item.DateTime.toUTCString() + (item.Comment != null ? " " + item.Comment : ""),
                            icon: first ? lastMarkerIcon : markerIcon,
                            zIndex: first ? 100000 : 1
                        });

                        if (first) first = false;
                    });
                    if (options.page < options.maxPages && parseInt(data.Pager.Page) < parseInt(data.Pager.Pages)) {
                        loadPositionsRecursive({ page: options.page + 1, maxPages: options.maxPages });
                    } else {
                        var pathLine = new google.maps.Polyline({
                            path: path,
                            strokeColor: "#FF0000",
                            strokeOpacity: 1.0,
                            strokeWeight: 3,
                            map: map
                        });

                        google.maps.event.addListenerOnce(map, 'bounds_changed', function () {
                            if (map.getZoom() > 6) map.setZoom(6);
                        });

                        map.fitBounds(latLngBounds);
                    }
                }
            });
        };

        loadPositionsRecursive(options);
    };
});

/*!
* jQuery.parseJSON() extension (supports ISO & Asp.net date conversion)
*
* Version 1.0 (13 Jan 2011)
*
* Copyright (c) 2011 Robert Koritnik
* Licensed under the terms of the MIT license
* http://www.opensource.org/licenses/mit-license.php
*/
(function ($) {

    // JSON RegExp
    var rvalidchars = /^[\],:{}\s]*$/;
    var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
    var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
    var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
    var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
    var dateNet = /\/Date\((\d+)(?:-\d+)?\)\//i;

    // replacer RegExp
    var replaceISO = /"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:[.,](\d+))?Z"/i;
    var replaceNet = /"\\\/Date\((\d+)(?:-\d+)?\)\\\/"/i;

    // determine JSON native support
    var nativeJSON = (window.JSON && window.JSON.parse) ? true : false;
    var extendedJSON = nativeJSON && window.JSON.parse('{"x":9}', function (k, v) { return "Y"; }) === "Y";

    var jsonDateConverter = function (key, value) {
        if (typeof (value) === "string") {
            if (dateISO.test(value)) {
                return new Date(value);
            }
            if (dateNet.test(value)) {
                return new Date(parseInt(dateNet.exec(value)[1], 10));
            }
        }
        return value;
    };

    $.extend({
        parseJSON: function (data, convertDates) {
            /// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.</summary>
            /// <param name="data" type="String">The JSON string to parse.</param>
            /// <param name="convertDates" optional="true" type="Boolean">Set to true when you want ISO/Asp.net dates to be auto-converted to dates.</param>

            if (typeof data !== "string" || !data) {
                return null;
            }

            // Make sure leading/trailing whitespace is removed (IE can't handle it)
            data = $.trim(data);

            // Make sure the incoming data is actual JSON
            // Logic borrowed from http://json.org/json2.js
            if (rvalidchars.test(data
                .replace(rvalidescape, "@")
                .replace(rvalidtokens, "]")
                .replace(rvalidbraces, ""))) {
                // Try to use the native JSON parser
                if (extendedJSON || (nativeJSON && convertDates !== true)) {
                    return window.JSON.parse(data, convertDates === true ? jsonDateConverter : undefined);
                }
                else {
                    data = convertDates === true ?
                        data.replace(replaceISO, "new Date(parseInt('$1',10),parseInt('$2',10)-1,parseInt('$3',10),parseInt('$4',10),parseInt('$5',10),parseInt('$6',10),(function(s){return parseInt(s,10)||0;})('$7'))")
                            .replace(replaceNet, "new Date($1)") :
                        data;
                    return (new Function("return " + data))();
                }
            } else {
                $.error("Invalid JSON: " + data);
            }
        }
    });
})(jQuery);