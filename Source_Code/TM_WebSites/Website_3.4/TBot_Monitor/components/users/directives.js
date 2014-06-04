angular.module('tbot')
       .directive('usersMenu', function()
            {
                return {
                            restrict    : 'E',
                            templateUrl : 'components/users/directives/usersMenu.html'
                       };
            });