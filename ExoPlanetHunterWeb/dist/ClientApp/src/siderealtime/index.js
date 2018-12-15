var siderealtime = function (longitude) {
    return timeformat(time(longitude));
};
var time = function (longitude) {
    var now = new Date;
    var utc_timestamp = Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds(), now.getUTCMilliseconds());
    var min = now.getUTCMinutes();
    var start = Date.UTC(now.getUTCFullYear(), 0, 1);
    var diff = (utc_timestamp - start);
    var oneDay = 1000 * 60 * 60 * 24;
    var days = Math.floor(diff / oneDay);
    var startime = 0.0657098 * days - 17.41409 + ((now.getUTCHours() * 60 + now.getUTCMinutes() + 4 * longitude) / 60) * 1.002737909;
    if (startime < 0) {
        startime = startime + 24;
    }
    if (startime > 24) {
        startime = startime - 24;
    }
    return startime;
};
var timeformat = function (time) {
    var decimal = Math.trunc(time);
    var fractional = (time - decimal);
    var numb = (fractional * 60).toString().substring(0, 2);
    if (fractional < 10 / 60) {
        numb = "0" + (fractional * 60).toString().substring(0, 1);
    }
    return decimal + ":" + numb;
};
export default siderealtime;
