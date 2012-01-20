TM.ControlPanel.AdminTasks =
		{
		        baseUrl                     : document.location.origin + '/_test_tm_libraries/'
		    ,   showResult                  : function (result)
		                                        {
		                                            //var formatedResult = js_beautify(JSON.stringify(result));			
		                                            var formatedResult = js_beautify(JSON.stringify(result));
		                                            $("#jsonResult").html(formatedResult);
		                                            prettyPrint();
		                                        }

		    ,   viewConfigFile               : function ()
		                                        {
		                                            showResult('');
		                                        }

		    ,   addLinkWithWebServiceCall   : function (text, method, url, autoExecute)
		                                        {
		                                            var that = TM.ControlPanel.AdminTasks;
		                                            var link = $("#adminTasksLinks").add_Link(text)
													                                        .click(function ()
													                                        {
													                                            that.setInvocationValues(method, url, autoExecute);
													                                        })
													                                        .append("<br/>");
		                                            return link;
		                                        }
		    ,   invokeWebService            : function ()
		                                        {
		                                            var method = $("#webMethodName").val();
		                                            var params = $("#webMethodParams").val();
		                                            _params = params;
		                                            $("#jsonResult").html('...invoking web service...');

		                                            var showResult = TM.ControlPanel.AdminTasks.showResult;
		                                            TM.WebServices.Helper.invoke_TM_WebService(method, params, showResult, showResult);
		                                        }

		    ,   setInvocationValues         : function (webMethodName, webMethodParams, autoExecute)
		                                        {
		                                            if (typeof (webMethodParams) == "object");
		                                            webMethodParams = JSON.stringify(webMethodParams);
		                                            $("#jsonResult").html('...click invoke to execute...');
		                                            $("#webMethodName").val(webMethodName);
		                                            $("#webMethodParams").val(webMethodParams);
		                                            if (autoExecute)
		                                                this.invokeWebService();
		                                        }

		    ,   addLibrary                  : function (name, zipFile)
		                                        {
		                                            this.addLinkWithWebServiceCall(name, 'XmlDatabase_ImportLibrary_fromZipFile',
														                                         { pathToZipFile: this.baseUrl + zipFile },
														                                         true);
		                                        }

		    ,   applyCss                    : function ()
		                                        {
		                                            $("#adminTasksLinksExecutionResult").absolute().width("79%")
														                                           .top(0).right(0).bottom(0);
		                                            $("#adminTasksLinksExecutionResult").css("padding", "10px");


		                                            $("#executionParameters table").width($("#adminTasksLinksExecutionResult").width());
		                                            $("#webMethodName").css({ width: "98%" });
		                                            $("#webMethodParams").css({ width: "98%" });
		                                            $("#invoke").css({ width: "100", height: "45px" }).button();
		                                        }

		    ,   createGui                   : function ()
                                                {
                                                    var adminTasks = TM.ControlPanel.AdminTasks;
                                                    $("#invoke").click(adminTasks.invokeWebService);
                                                    $('#webMethodName').keypress(function (e) { if (e.which == 13) { adminTasks.invokeWebService() } });
                                                    $('#webMethodParams').keypress(function (e) { if (e.which == 13) { adminTasks.invokeWebService() } });

                                                    adminTasks.addLinkWithWebServiceCall('Get time', 'GetTime', {}, true).click();
                                                    adminTasks.addLinkWithWebServiceCall('View Config File', 'TMConfigFile', {}, true);
                                                    adminTasks.addLinkWithWebServiceCall('Get Library Path', 'XmlDatabase_GetLibraryPath', {}, true);
                                                    adminTasks.addLinkWithWebServiceCall('Set Library Path (default)', 'XmlDatabase_SetLibraryPath', { libraryPath: 'TM_Libraries' }, false);
                                                    adminTasks.addLinkWithWebServiceCall('Set Library Path (temp)', 'XmlDatabase_SetLibraryPath', { libraryPath: 'C:\\O2\\_tempDir\\_TeamMentor_Temp_Libraries' }, false);
                                                    adminTasks.addLinkWithWebServiceCall('Get Library Zips Path', 'Get_Libraries_Zip_Folder', {}, true);
                                                    adminTasks.addLinkWithWebServiceCall('Set Library Zips Path (default)', 'Set_Libraries_Zip_Folder', { folder: '..//Library_Data//TM_Library_Zips' }, false);


                                                    adminTasks.addLinkWithWebServiceCall('Get Libraries', 'GetLibraries', {}, true);
                                                    adminTasks.addLinkWithWebServiceCall('Reload Server Cache', 'XmlDatabase_ReloadData', {}, true);

                                                    adminTasks.showResult('executing result will be shown here');

                                                    adminTasks.applyCss();

                                                    TM.Events.onControlPanelViewLoaded();
                                                }
        }