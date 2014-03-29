angular
.module('peregrineUi.directives')
.directive('upDown', function() {
	return {
		restrict: 'A',
		replace: true,
		templateUrl: '/DirectiveTemplates/UpDown.html?1=1',
		scope: {
			item: '=upDown',
			attribute: '=upDownAttribute',
			updateFunction: '=upDownChange'
		},
		link: function($scope, element, attrs) {
			$scope.value = $scope.item[$scope.attribute];
			$scope.increment = function() {
				$scope.item[$scope.attribute]++;
				$scope.updateFunction($scope.item, $scope.attribute);
			};

			$scope.decrement = function() {
				$scope.item[$scope.attribute]--;
				$scope.updateFunction($scope.item, $scope.attribute);
			};
		}
	};
});