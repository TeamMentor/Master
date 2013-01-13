TM.Tracking_Google_Analytics_ID = "UA-37594728-3";

var _gaq = _gaq || [];
_gaq.push(['_setAccount', TM.Tracking_Google_Analytics_ID]);
_gaq.push(['_trackPageview']);

(function() {
	console.log("Tracking code: " + TM.Tracking_Google_Analytics_ID);
var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
ga.src = "/javascript/gAnalytics/ga.js";
var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();
