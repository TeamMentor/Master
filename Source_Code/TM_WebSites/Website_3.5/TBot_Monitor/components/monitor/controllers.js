var project = angular.module ('tbot');

project.controller('DebugCtrl'      , function($scope, fbDebugMsg, fbRequestUrl)
{
    $scope.fbDebugMsg   = fbDebugMsg;
    $scope.fbRequestUrl = fbRequestUrl;
});

project.controller('MessagesCtrl'   , function($scope, firebase_DebugLogs)
{
    firebase_DebugLogs.then(function(fbDebugMsg)
        {
            $scope.serverUrl = "...";
            $scope.messages = [".... Loading most recent 50 entries ..."];
            $scope.count = 0;
            fbDebugMsg = fbDebugMsg.limit(50);
            fbDebugMsg.on("child_added", function(childSnapshot, prevChildName)
            {
                var message   = childSnapshot.val();
                message.Index = ++$scope.count;
                $scope.messages.splice(0,0,message);
                $scope.$apply();
            });
        });
    $scope.rowClass = function(messsage)
    {
        if (messsage.Type === "ERROR" || messsage.Type === "EXCEPTION")
            return "danger";
        if (messsage.Type === "DEBUG")
            return "success";
        if (messsage.Type === "INFO")
            return "active";
        return "warning";
    };
});

project.controller('RequestUrlsCtrl', function($scope , firebase_RequestUrl)//fbRequestUrl, tmUrl)
{
    firebase_RequestUrl.then(function(fbRequestUrl)
        {
            _fbRequestUrl  = fbRequestUrl;
            $scope.serverUrl = "...";
            $scope.urls = [".... Loading most recent 50 entries ..."];
            $scope.count = 0;

            fbRequestUrl = fbRequestUrl.limit(50);
            fbRequestUrl.on("child_added", function(childSnapshot, prevChildName)
            {
                requestUrl = childSnapshot.val();
                requestUrl.Index = ++$scope.count;
                $scope.urls.splice(0,0,requestUrl);
                $scope.$apply();

            });
        });
});

project.controller('ActivitiesCtrl', function($scope, firebase_Activities)
{
    firebase_Activities.then(function(fbActivity)
        {
            $scope.serverUrl = "...";
            $scope.activities = [".... Loading most recent 50 entries ..."];
            $scope.count = 0;
            fbActivity = fbActivity.limit(50);
            fbActivity.on("child_added", function(childSnapshot, prevChildName)
            {
                activity = childSnapshot.val();
                activity.Index = ++$scope.count;
                $scope.activities.splice(0,0,activity);
                $scope.$apply();

            });
        });

    $scope.rowClass = function(activity)
    {
        if (activity.Action === "User Login")
            return "success";
        if (activity.Action === "Access Denied" || activity.Action === "404" ||  activity.Action === "Login Fail" ||
            activity.Action === "Account Expired" || activity.Action ==="SessionId Not Accepted")
        {
            return "danger";
        }

        if (activity.Who === "TBot")
            return "active";
        //if (activity.Who === "admin")
        //    return "active";

        return "";
    };
});

/*project.controller('UserDataCtrl'  , function($scope, $http, $cacheFactory)
{
    $http.get('data/userData_1.js?123')
        .success(function(data)
        {
            $scope.userData = angular.fromJson(data);
        })
        .error  (function()
    {
        $scope.userData = "error";
    });
    $scope.userData = "...loading...";
    scope = $scope;
    cache = $cacheFactory;
});        */