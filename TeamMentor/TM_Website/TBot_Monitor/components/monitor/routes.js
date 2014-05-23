angular.module ('tbot')
       .config(function($routeProvider)
            {
                $routeProvider.when	    ('/'                    , { templateUrl : 'components/monitor/views/main.html'                                          })
                              .when	    ('/monitor/logs'        , { templateUrl : 'components/monitor/views/logs.html'        , controller  : 'MessagesCtrl'    })
                              .when	    ('/monitor/urlRequests' , { templateUrl : 'components/monitor/views/urlRequests.html' , controller  : 'RequestUrlsCtrl' })
                              .when	    ('/monitor/activities'  , { templateUrl : 'components/monitor/views/activities.html'  , controller  : 'ActivitiesCtrl' })
            });