
TM.ControlPanel.WebServices.configDir   =   '/Html_Pages/ControlPanel/WebServices/';

TM.ControlPanel.WebServices.jsPages     =   {
			                                        title: 'TM WebServices'				
                                                ,   table: [												                
                                                                    { name: 'Libraries'			            , url: TM.ControlPanel.WebServices.configDir + 'WebServices_TM_Libraries.html' 			        , params: '' }
                                                                ,   { name: 'Users '                        , url: TM.ControlPanel.WebServices.configDir + 'WebServices_TM_Users.html'              , params: '' }
                                                                ,   { name: 'Config '                       , url: TM.ControlPanel.WebServices.configDir + 'WebServices_TM_Config.html'             , params: '' }
                                                                ,   { name: 'Session '                      , url: TM.ControlPanel.WebServices.configDir + 'WebServices_TM_Session.html'            , params: '' }
                                                                ,   { name: 'OnlineStorage'                 , url: TM.ControlPanel.WebServices.configDir + 'WebServices_TM_OnlineStorage.html'      , params: '' }
                                                                ,   { name: 'ActivityTracking'              , url: TM.ControlPanel.WebServices.configDir + 'WebServices_TM_ActivityTracking.html'   , params: '' }
                                                                ,   { name: 'Util: View Activity '          , url: TM.ControlPanel.WebServices.configDir + 'View_Current_Activity.html'             , params: '' }                                
                                                                ,   { name: 'Util: Invoke Javascript'       , url: TM.ControlPanel.WebServices.configDir + 'Invoke_Javascript.html'                 , params: '' }

				                                            ]
                                             }

TM.ControlPanel.WebServices.buildGui    = function()
                                            {
                                                jQuery('#menu').setTemplateURL('/javascript/jTemplates/LinksMenu.html?time='+ new Date().getTime(), null, {filter_data: true} );															
							                    jQuery('#menu').processTemplate(TM.ControlPanel.WebServices.jsPages);
							
							                    TM.Events.onControlPanelViewLoaded();	
                                            }          
;
	    