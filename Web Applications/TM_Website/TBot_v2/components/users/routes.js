angular.module ('tbot')
       .config(function($routeProvider)
            {
                $routeProvider.when	    ('/'           , { templateUrl :'components/users/views/main.html'           })
                              .when	    ('/users/list' , { templateUrl :'/rest/tbot/render/Current_Users'            })
                              .when	    ('/users/sso'  , { templateUrl :'/rest/tbot/render/SSO_Token'                })
                              .when	    ('/users/main' , { templateUrl :'components/users/views/main.html'           })
                              .when	    ('/test'       , { templateUrl :'components/main/views/test.html'            })
            });