angular.module ('tbot' )
    .factory('asmxService'   , function($http)
        {
            $http.defaults.headers.post['Content-Type'] = 'application/json;charset=UTF-8';

            var asmxService = {
                                    serviceUrl    : '/Aspx_Pages/TM_WebServices.asmx/',

                                    invokeService : function (method, parameters, onSuccess)
                                                        {
                                                            if (parameters === undefined)
                                                                parameters = {}
                                                            $http.post(asmxService.serviceUrl + method, parameters)
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
                                                            asmxService.invokeService("Current_User", {} , onSuccess);
                                                        },
                                    set_CSRF_Token : function(onSuccess)
                                                        {
                                                            asmxService.Current_User(function (data)
                                                                {
                                                                    if (data!= null)
                                                                    {
                                                                        $http.defaults.headers.post['CSRF-Token'] = data.CSRF_Token;
                                                                    }
                                                                    onSuccess(data);
                                                                });
                                                        }

                            };
            return asmxService
        });
