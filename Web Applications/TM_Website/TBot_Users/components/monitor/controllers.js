var project = angular.module ('tbot');

project.controller('DebugCtrl'      , function($scope, fbDebugMsg, fbRequestUrl)
{
    $scope.fbDebugMsg   = fbDebugMsg;
    $scope.fbRequestUrl = fbRequestUrl;
});

project.controller('MessagesCtrl'   , function($scope, fbDebugMsg, tmUrl)
{
    //$scope.messages = fbaDebugMsg;
    $scope.serverUrl = tmUrl;
    $scope.messages = [".... Loading most recent 50 entries ..."];
    $scope.count = 0;
    fbDebugMsg = fbDebugMsg.limit(50);
    fbDebugMsg.on("child_added", function(childSnapshot, prevChildName)
    {
        //var text = "[#" + ++$scope.count + "] " + childSnapshot.val().text;
        //$scope.messages.splice(0,0,text);
        var message   = childSnapshot.val();
        message.Index = ++$scope.count;
        $scope.messages.splice(0,0,message);
        $scope.$apply();
    });

    $scope.rowClass = function(messsge)
    {
        if (messsge.Type === "ERROR")
            return "danger";
        if (messsge.Type === "DEBUG")
            return "success";
        if (messsge.Type === "INFO")
            return "active";
        return "warning";
    };
});

project.controller('RequestUrlsCtrl', function($scope, fbRequestUrl, tmUrl)
{
    $scope.serverUrl = tmUrl;
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

project.controller('ActivitiesCtrl', function($scope, fbActivity, tmUrl)
{
    $scope.serverUrl = tmUrl;
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