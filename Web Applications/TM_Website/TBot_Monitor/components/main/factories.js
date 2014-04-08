angular.module ('tbot' )
    .factory('asmxService'   , function($http)
        {
            $http.defaults.headers.post['Content-Type'] = 'application/json;charset=UTF-8';

            var asmxService = {
                                    serviceUrl    : '/Aspx_Pages/TM_WebServices.asmx/',
                                    currentUser   : {},
                                    firebaseConfig: {},
                                    invokeService : function (method, parameters, onSuccess)
                                            {
                                                if (parameters === undefined)
                                                    parameters = {}
                                                return $http.post(asmxService.serviceUrl + method, parameters)
                                                            .success(function (data)
                                                                {
                                                                    if (onSuccess)
                                                                        onSuccess(data.d);
                                                                    else
                                                                        console.log(data.d);
                                                                });
                                            },
                                    Current_User  : function(onSuccess)
                                            {
                                                return asmxService.invokeService("Current_User", {} , onSuccess);
                                            },
                                    set_CSRF_Token : function(onSuccess)
                                            {
                                                return asmxService.Current_User(function (data)
                                                    {
                                                        if (data!= null)
                                                        {
                                                            asmxService.currentUser = data;
                                                            $http.defaults.headers.post['CSRF-Token'] = data.CSRF_Token;
                                                        }
                                                        if (onSuccess)
                                                            onSuccess(data);
                                                    });
                                            } ,
                                    Get_Firebase_ClientConfig  : function(onSuccess)
                                            {

                                                return asmxService.invokeService("Get_Firebase_ClientConfig");

                                                          /*  asmxService.set_CSRF_Token()
                                                                  .success(function()
                                                                    {
                                                                        return asmxService.invokeService("Get_Firebase_ClientConfig");
                                                                    });    */
                                            }

                            };
            return asmxService
        })
    .factory("firebaseConfig",function(asmxService)
        {
           return asmxService.Get_Firebase_ClientConfig();
        });
