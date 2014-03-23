angular.module ('tbot')
       .config(function($routeProvider)
            {
                $routeProvider.when	    ('/'          , { templateUrl :'components/main/views/main.html'   , controller : 'MainController'           })
                              .when	    ('/tbot'      , { templateUrl :'/rest/tbot/render/Commands'        , controller : 'MainController'           })
                              .when	    ('/test'      , { templateUrl :'components/main/views/test.html'                                             })

                              .otherwise(               {templateUrl : 'components/main/views/404.html'    , controller : 'MainController'            });
                              //.otherwise(		   { redirectTo  :'/404'	  						                 });
            });