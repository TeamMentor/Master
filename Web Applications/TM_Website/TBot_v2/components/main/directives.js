angular.module('tbot')
       .directive('topMenu', function()
            {
                return {
                            restrict    : 'E',
                            templateUrl : 'components/main/directives/topMenu.html'
                       };
            });