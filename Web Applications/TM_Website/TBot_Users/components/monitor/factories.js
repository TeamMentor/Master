angular.module ('tbot' )
       .factory('fbDebugMsg'   , function($firebase, fbURL, fbAuthToken)
            {
                var firebase = new Firebase(fbURL  + "/testLogs");//"/debugMsg");
                firebase.auth(fbAuthToken);
                return firebase;
            })
       .factory('fbRequestUrl'   , function($firebase, fbURL, fbAuthToken)
            {
                var firebase = new Firebase(fbURL  + "/requestUrls");
                firebase.auth(fbAuthToken);
                return firebase;
            })
        .factory('fbActivity'   , function($firebase, fbURL, fbAuthToken)
            {
                var firebase = new Firebase(fbURL  + "/activities");
                firebase.auth(fbAuthToken);
                return firebase;
            })
       .factory('fbaDebugMsg'   , function($firebase, fbDebugMsg)
            {
                return $firebase(fbDebugMsg);
            });


