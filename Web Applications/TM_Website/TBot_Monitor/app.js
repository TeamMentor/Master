var app = angular.module('tbot', ['ngRoute','firebase'])
                 //.value  ('fbAuthToken', '6Q8NwnKwm3DEo5gr0jS9HgryWD1QiBriqJTRYepB')
                 //.value  ('fbURL'      , 'https://tm-admin-test.firebaseio.com/QA_Server_Dev')
                 .value  ('tmUrl'      , 'https://tm-34-qa.azurewebsites.net');
/*
app.run(function($rootScope, $location)
    {
        $rootScope.$on('$routeChangeSuccess', function(event, current)
            {
                //fix links on /rest/tbot/run/Current_Users
                if (current.loadedTemplateUrl == '/rest/tbot/run/Current_Users')
                {
                    template = current.locals.$template.replace(/href="User/g,'href="/rest/tbot/run/User');
                    current.locals.$template =template;
                }
            });
    }) ;



app.controller('MainController', function($scope, $route, $routeParams, $location) {
    console.log("in MainController")
    $scope.$route = $route;
    $scope.$location = $location;
    $scope.$routeParams = $routeParams;
    scope =  $scope;


});
    */