angular.module ('tbot' )
       .factory('firebase_Configured', function(asmxService)
            {
                return asmxService.Get_Firebase_ClientConfig()
                    .then(function(firebaseConfig)
                    {
                        return  {
                            url            : "https://" + firebaseConfig.Firebase_Site + ".firebaseio.com/" + firebaseConfig.Firebase_RootArea,
                            authToken      :  firebaseConfig.Firebase_AuthToken
                        };
                    });
            })
       .factory('firebase_RequestUrl', function(firebase_Configured)
            {
                return firebase_Configured.then(function(urlAndToken)
                {

                    firebase = new Firebase(urlAndToken.url  + "/requestUrls");
                    firebase.auth(urlAndToken.authToken);
                    return firebase;
                });
            })
       .factory('firebase_DebugLogs', function(firebase_Configured)
            {
                return firebase_Configured.then(function(urlAndToken)
                {
                    firebase = new Firebase(urlAndToken.url  + "/debugMsgs");
                    firebase.auth(urlAndToken.authToken);
                    return firebase;
                });
            })
        .factory('firebase_Activities', function(firebase_Configured)
        {
            return firebase_Configured.then(function(urlAndToken)
            {
                firebase = new Firebase(urlAndToken.url  + "/activities");
                firebase.auth(urlAndToken.authToken);
                return firebase;
            });
        })
/*.factory("firebaseConfig",function(asmxService)
 {
 return  asmxService.set_CSRF_Token().then(function (data)
 {
 return asmxService.Get_Firebase_ClientConfig()
 })
 .then(function (data)
 {
 return data.data.d;
 });
 //return asmxService.set_CSRF_Token()
 ;
 return asmxService.Get_Firebase_ClientConfig();
 });               */


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


