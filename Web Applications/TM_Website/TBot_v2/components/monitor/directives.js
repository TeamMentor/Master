angular.module('tbot')
       .directive('subMenu', function()
            {
                return {
                            restrict    : 'E',
                            templateUrl : 'components/monitor/directives/subMenu.html'
                       };
            });