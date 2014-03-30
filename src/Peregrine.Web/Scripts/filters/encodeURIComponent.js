angular
.module('peregrineUi.filters')
.filter('encodeURIComponent', function() {
	return window.encodeURIComponent;
});