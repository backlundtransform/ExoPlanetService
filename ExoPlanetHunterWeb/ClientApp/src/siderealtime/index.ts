 const siderealtime = (longitude: number): string => {

	return timeformat(time(longitude));

}
const time = (longitude: number): number => {
	const now = new Date;
	var utc_timestamp = Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(),
		now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds(), now.getUTCMilliseconds());

	const min = now.getUTCMinutes()
	const start = Date.UTC(now.getUTCFullYear(), 0, 1);
	const diff = (utc_timestamp - start);
	const oneDay = 1000 * 60 * 60 * 24;
	const days = Math.floor(diff / oneDay);

	let startime = 0.0657098 * days - 17.41409 + ((now.getUTCHours() * 60 + now.getUTCMinutes() + 4 * longitude) / 60) * 1.002737909;

	if (startime < 0) {
		startime = startime + 24;
	}

	if (startime > 24) {
		startime = startime - 24;
	}

	return startime


}

const timeformat = (time: number) => {
	const decimal = Math.trunc(time);
	const fractional = (time - decimal);
	let numb = (fractional * 60).toString().substring(0, 2);
	if (fractional < 10 / 60) {
		numb = "0" + (fractional * 60).toString().substring(0, 1);
	}

	return decimal + ":" + numb;

}
export default siderealtime