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
				templateUrl: 'Partials/Round.html'
			})
			.otherwise({ redirectTo: '/' });
	});

angular.module('peregrineUi.controllers', [
	
]);

angular.module('peregrineUi.directives', [

]);

//setup controllers

angular
.module('peregrineUi.controllers')
.controller('mainController', [
	'$scope', '$resource',
	function ($scope, $resource) {
		var Tournament = $resource('/api/tournaments/:tournamentKey', { tournamentKey: '@key' });
		$scope.tournamentKeys = Tournament.get();
	}
]);

angular
.module('peregrineUi.controllers')
.controller('roundController',[
	'$scope', '$route', '$routeParams', '$http',
	function ($scope, $route, $routeParams, $http) {
		$scope.$roundNumber = $routeParams.roundNumber;
		$scope.$tournamentKey = $routeParams.tournamentKey;
		if ($routeParams.tournamentKey && $routeParams.roundNumber) {
			//get existing tournament
			var url = '/api/tournaments/' + encodeURIComponent($routeParams.tournamentKey) + '/rounds/' + encodeURIComponent($routeParams.roundNumber) + '/pairings';
			$http.get(url)
				.success(function (data, status, headers, config) {
					$scope.error = '';
					console.log(data);
				})
				.error(function (data, status, headers, config) {
					$scope.error = 'Sorry We were unable to get the pairings.';
				});
		}
	}
]);

angular
.module('peregrineUi.controllers')
.controller('tournamentController',[
	'$scope', '$routeParams', '$http', '$resource',
	function ($scope, $routeParams, $http, $resource) {
		$scope.players = [];
		$scope.error = '';

		$scope.$watch('players', function (newValue) {
			if (newValue) {
				newValue.sort(comparePlayer);
			}
		}, true);

		if ($routeParams.tournamentKey) {
			//get existing tournament
			var url = '/api/tournaments/' + encodeURIComponent($routeParams.tournamentKey);
			$http.get(url)
				.success(function (data, status, headers, config) {
					$scope.error = '';
					$scope.tournament = data.tournament;
					//get players
					var playersLink = url + '/players';
					$http.get(playersLink)
					.success(function (data, status, headers, config) {
						$scope.error = '';
						$scope.players = data.names;
					})
					.error(function (data, status, headers, config) {
						$scope.error = 'Sorry =( We failed to load your players.';
					})
					$scope.roundOneLink = getRoundLink($scope.tournament.key, 1);
				})
				.error(function (data, status, headers, config) {
					$scope.error = 'Sorry =( We were unable to find your tournament.';
				});
		}
		else {
			//create a tournament
			$http.post('/api/tournaments')
				.success(function (data, status, headers, config) {
					$scope.error = '';
					$scope.tournament = data.tournament;
					$scope.roundOneLink = getRoundLink($scope.tournament.key, 1);
				})
				.error(function (data, status, headers, config) {
					$scope.error = 'We were unable to create a tournament.';
				});
		}

		$scope.addPlayer = function (playerName) {
			if (playerName) {
				$scope.newPlayerName = playerName;
			}
			if ($scope.newPlayerName && $scope.newPlayerName.length > 0 && $scope.tournament.key) {
				var url = '/api/tournaments/' + encodeURIComponent($scope.tournament.key) + '/players/' + encodeURIComponent($scope.newPlayerName);
				$http.put(url)
					.success(function (data, status, headers, config) {
						$scope.error = '';
						var newPlayer = data.player;
						$scope.players.push(newPlayer.name);
						$scope.newPlayerName = '';
					})
					.error(function (data, status, headers, config) {
						if (status === 409) {
							$scope.error = 'Sorry a player with the same name already exists.'
						}
						else {
							$scope.error = 'We were unable to add the player!';
						}
					});
			}
		};

		$scope.deletePlayer = function (playerName) {
			var deletePlayerLink = '/api/tournaments/' + $scope.tournament.key + '/players/' + playerName;
			$http.delete(deletePlayerLink)
				.success(function (data, status) {
					var otherPlayers = $scope.players.reduce(function (everyoneElse, deletedPlayer) {
						if (deletedPlayer === playerName) {
							return everyoneElse;
						}
						else {
							return everyoneElse.concat(deletedPlayer);
						}
					}, []);

					$scope.players = otherPlayers;
				})
				.error(function (data, status) {
					console.log(status);
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

function getRoundLink(tournamentCode, roundNumber) {
	return '#/Tournament/' + encodeURIComponent(tournamentCode) + '/Round/' + roundNumber;
}

//click to edit
angular
.module('peregrineUi.directives')
.directive('itemEditor', function () {
	return {
		restrict: 'A',
		replace: true,
		templateUrl: '/DirectiveTemplates/ItemEditor.html?1=1',
		scope: {
			itemIndex: '=itemIndex',
			itemList: '=itemList',
			itemDeleteCallback: '=itemDeleteCallback',
			itemEditCallback: '=itemEditCallback'
		},
		link: function ($scope, $element) {
			$scope.item = $scope.itemList[$scope.itemIndex];

			$scope.view = {
				editableValue: $scope.item,
				editorEnabled: false
			};

			$scope.enableEditor = function () {
				$scope.view.editorEnabled = true;
				$scope.view.editableValue = $scope.item;
				setTimeout(function () {
					$element.find('.editable-value').focus().select();
				}, 2);
			};

			$scope.disableEditor = function () {
				$scope.view.editorEnabled = false;
			};

			$scope.save = function () {
				$scope.newItem = {}; //remove this
				$scope.newItem = $scope.view.editableValue;
				$scope.itemEditCallback($scope.newItem, $scope.item);
				$scope.disableEditor();
			};

			$scope.deleteItem = function () {
				$scope.itemDeleteCallback($scope.item);
			};
		}
	};
});
