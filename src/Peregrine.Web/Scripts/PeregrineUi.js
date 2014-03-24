//define the main module
angular.module('peregrineUi', [
	'ngRoute',
	'ngResource',
	'peregrineUi.controllers',
	'peregrineUi.directives'
]).config(function ($routeProvider, $locationProvider) {
		$routeProvider
			.when('/', {
				controller: 'mainController',
				templateUrl: 'Partials/Home.html'
			})
			.when('/Tournament', {
				controller: 'tournamentController',
				templateUrl: 'Partials/TournamentDetail.html'
			})
			.when('/Tournament/:tournamentKey', {
				controller: 'tournamentController',
				templateUrl: 'Partials/TournamentDetail.html'
			})
			.when('/Tournament/:tournamentKey/Round/:roundNumber', {
				controller: 'roundController',
				templateUrl: 'Partials/RoundDetail.html'
			})
			.otherwise({ redirectTo: '/' });
	});

//modules
angular.module('peregrineUi.resources', [
]);

angular.module('peregrineUi.controllers', [
	'peregrineUi.resources'
]);

angular.module('peregrineUi.directives', [
]);

//resources
angular
.module('peregrineUi.resources')
.factory('tournamentResource', [
	'$resource',
	function ($resource) {
		return $resource('/api/tournaments/:tournamentKey', { tournamentKey: '@key' }, {
			create: {
				method: 'post'
			}
		});
	}
])

angular
.module('peregrineUi.resources')
.factory('playerResource', [
	'$resource',
	function ($resource) {
		return $resource('/api/tournaments/:tournamentKey/players/:playerName', { playerName: '@name' }, {
			save: {
				method: 'put'
			}
		});
	}
])

angular
.module('peregrineUi.resources')
.factory('roundResource', [
	'$resource',
	function ($resource) {
		return $resource('/api/tournaments/:tournamentKey/rounds/:roundNumber', { roundNumber: '@number' }, {
			save: {
				method: 'put'
			}
		});
	}
])

//controllers
angular
.module('peregrineUi.controllers')
.controller('mainController', [
	'$scope', '$resource', 'tournamentResource',
	function ($scope, $resource, tournamentResource) {
		$scope.tournaments = tournamentResource.query();
	}
]);

angular
.module('peregrineUi.controllers')
.controller('roundController',[
	'$scope', '$route', '$routeParams', '$http', 'roundResource',
	function ($scope, $route, $routeParams, $http, roundResource) {
		$scope.error = '';
		if (!$routeParams.tournamentKey) {
			$scope.error = 'Whoa there! Wheres your tournament key? Put your browser in reverse and take another run at it.';
		}
		roundResource.get({ tournamentKey: $routeParams.tournamentKey, roundNumber: $routeParams.roundNumber }, 
			function (round) {
				$scope.error = '';
				$scope.round = round;
			},
			function () {
				$scope.error = 'We couldn\'t get your round';
			});
	}
]);

angular
.module('peregrineUi.controllers')
.controller('tournamentController',[
	'$scope', '$routeParams', '$http', '$resource', 'tournamentResource', 'playerResource',
	function ($scope, $routeParams, $http, $resource, tournamentResource, playerResource) {
		$scope.players = []; //necessary?
		$scope.error = '';

		$scope.$watch('players', function (newValue) {
			if (newValue) {
				newValue.sort(comparePlayer);
			}
		}, true);

		if ($routeParams.tournamentKey) {
			//get existing tournament
			tournamentResource.get(
				{ tournamentKey: $routeParams.tournamentKey },
				function (tournament) {
					$scope.error = '';
					$scope.tournament = tournament;
					//get players
					playerResource.query({ tournamentKey: tournament.key },
						function (players) {
							$scope.error = '';
							$scope.players = players;
						},
						function (players) {
							$scope.error = 'Sorry =( We failed to load your players.';
						});
				},
				function () {
					$scope.error = 'Sorry =( We were unable to find your tournament.';
				});
		}
		else {
			//create a tournament
			$scope.tournament = tournamentResource.create(
				{},
				function () {
					$scope.error = '';
				}, 
				function () {
					$scope.error = 'We were unable to create a tournament.';
				});
		}

		$scope.addPlayer = function (player) {
			if (player) {
				$scope.newPlayer = player;
			}
			if ($scope.newPlayer.name && $scope.newPlayer.name.length > 0 && $scope.tournament.key) {
				playerResource.save(
					{ tournamentKey: $scope.tournament.key },
					$scope.newPlayer,
					function () {
						$scope.error = '';
						//get players
						playerResource.query({ tournamentKey: $scope.tournament.key },
							function (players) {
								$scope.error = '';
								$scope.players = players;
							},
							function (players) {
								$scope.error = 'Sorry =( We failed to load your players.';
							});
						$scope.newPlayer = {};
					},
					function () {
						if (status === 409) {
							$scope.error = 'Sorry a player with the same name already exists.'
						}
						else {
							$scope.error = 'We were unable to add the player!';
						}
					});
			}
		};

		$scope.deletePlayer = function (player) {
			playerResource.delete(
				{ tournamentKey: $scope.tournament.key, playerName: player.name },
				function () {
					$scope.error = '';
					//get players
					playerResource.query({ tournamentKey: $scope.tournament.key },
						function (players) {
							$scope.error = '';
							$scope.players = players;
						},
						function (players) {
							$scope.error = 'Sorry =( We failed to load your players.';
						});
				},
				function () {
					$scope.error = 'We were unable to delete the player!';
				});
		};

		//you cannot really update a player in the api so we'll add a new one and delete the old one.
		$scope.updatePlayer = function (newPlayer, oldPlayer) {
			//got a bug here. if the add fails the delete still happens. The check below fixes the worst problem. It would probably be best to add a real update method to the api and add real keys to the players.
			if (newPlayer != oldPlayer) {
				$scope.addPlayer(newPlayer);
				$scope.deletePlayer(oldPlayer);
			}
		};
	}
]);

function comparePlayer(playerOne, playerTwo) {
	if (playerOne.name < playerTwo.name)
		return -1;
	if (playerOne.name > playerTwo.name)
		return 1;
	return 0;
}

//click to edit item list
angular
.module('peregrineUi.directives')
.directive('itemEditor', function () {
	return {
		restrict: 'A',
		replace: true,
		templateUrl: '/DirectiveTemplates/ItemEditor.html?1=1',
		scope: {
			itemAttribute: '=itemValue',
			itemIndex: '=itemIndex',
			itemList: '=itemList',
			itemDeleteCallback: '=itemDeleteCallback',
			itemEditCallback: '=itemEditCallback'
		},
		link: function ($scope, $element) {
			$scope.item = $scope.itemList[$scope.itemIndex];
			$scope.itemValue = $scope.item[$scope.itemAttribute];

			$scope.view = {
				editableValue: $scope.itemValue,
				editorEnabled: false
			};

			$scope.enableEditor = function () {
				$scope.view.editorEnabled = true;
				$scope.view.editableValue = $scope.itemValue;
				setTimeout(function () {
					$element.find('.editable-value').focus().select();
				}, 2);
			};

			$scope.disableEditor = function () {
				$scope.view.editorEnabled = false;
			};

			$scope.saveItem = function () {
				$scope.newItem = {};
				$scope.newItem[$scope.itemAttribute] = $scope.view.editableValue;
				$scope.itemEditCallback($scope.newItem, $scope.item);
				$scope.disableEditor();
			};

			$scope.deleteItem = function () {
				$scope.itemDeleteCallback($scope.item);
			};
		}
	};
});
