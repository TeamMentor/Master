
var jsPages = {
			title: 'TM WebServices ',				
			table: [					
							//{ name: 'JS Host'					                , url: '/Aspx_Pages/Tests/JS_Host.html' 							, params: '' } ,
							
                                { name: 'Libraries'			            , url: TM.configDir + 'WebServices_TM_Libraries.html' 			, params: '' } ,
                                { name: 'Users '                        , url: TM.configDir + 'WebServices_TM_Users.html', params: '' },
                                { name: 'Config '                       , url: TM.configDir + 'WebServices_TM_Config.html', params: '' },
                                { name: 'Session '                      , url: TM.configDir + 'WebServices_TM_Session.html', params: '' },
                                { name: 'OnlineStorage'                 , url: TM.configDir + 'WebServices_TM_OnlineStorage.html', params: '' },
                                { name: 'ActivityTracking'              , url: TM.configDir + 'WebServices_TM_ActivityTracking.html', params: '' },
                          	    { name: 'Util: View Activity '          , url: TM.configDir + 'View_Current_Activity.html', params: '' },                                
                                { name: 'Util: Invoke Javascript'       , url: TM.configDir + 'Invoke_Javascript.html', params: '' }

				]
			    };

$(document).ready(			
			function() {				
							jQuery('#menu').setTemplateURL('/javascript/jTemplates/LinksMenu.html?time='+ new Date().getTime(), null, {filter_data: true} );															
							jQuery('#menu').processTemplate(jsPages);
							
							TM.Events.onControlPanelViewLoaded();	
						});			    