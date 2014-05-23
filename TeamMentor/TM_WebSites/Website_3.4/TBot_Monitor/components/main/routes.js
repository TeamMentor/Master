angular.module ('tbot')
       .config(function($routeProvider)
            {
                $routeProvider.when	    ('/'          , { templateUrl :'components/main/views/main.html'           })
                              .when	    ('/tbot'      , { templateUrl :'/rest/tbot/render/Commands'                })

                              .otherwise(               {templateUrl : 'components/main/views/404.html'            });
                              //.otherwise(		   { redirectTo  :'/404'	  						                 });
            });