angular.module('tbot')
       .directive('topMenu', function()
            {
                return {
                            restrict    : 'E',
                            templateUrl : 'components/main/directives/topMenu.html'
                       };
            })
        .directive('views', function()
            {
                return {
                    restrict    : 'E',
                    templateUrl : 'components/main/directives/views.html'
                };
            });