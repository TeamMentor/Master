//TM Settings
window.TM.tmVersion            = "TM 3.3.1 - Dev.3";
window.TM.ArticleTitle         = "TeamMentor 3.3";

window.TM.tmWebServices        = '/Aspx_Pages/TM_WebServices.asmx/';
window.TM.NotAuthorizedPage    = '/Html_Pages/Gui/Panels/AD_Non_Authorized_User.html';

window.TM.Tracking_Google_Analytics_ID                 = null;    // configure this in  TM_Custom_Settings.js
window.TM.Gui.showLibraryStructureToAnonymous          = true;
window.TM.Gui.LoadLibraryData                          = true;
window.TM.Gui.editMode                                 = false;

window.TM.Debug.addTimeStampToLoadedPages              = true;     // set true true during dev
window.TM.Debug.reuseLibraryMappingsObject             = true;
window.TM.Debug.load_HomePage                          = true;

window.TM.Debug.showExperimentalFeatures               = false;
window.TM.Debug.allow_start_checkUserLoop              = true;    // use to prevent the auto check of user settings


window.TM.Debug.show_DebugDiv                          = false;  // show small box with links to open trace view
window.TM.Debug.show_TraceView                         = false;  // show call trace popup window
window.TM.Debug.callTrace_LoadEnvironment              = false;  // open traceview popup and setup call trace hooks
window.TM.Debug.callTrace_LoadEnvironment_Before_Gui   = false;
window.TM.Debug.callTrace_ShowExecutionTime            = false;
window.TM.Debug.callTrace_LogToConsole                 = false;
window.TM.Debug.callTrace_ShowParamsInConsoleLog       = false;

window.TM.Debug.logEventsRaised                        = false;    // set to view the events fired in the console
window.TM.Debug.logEventsRaised_CallTrace              = false;    // will also show the CallTrace of the logged calls
window.TM.Debug.logLoadedPages                         = false;

//only put real passwords here in QA environments
window.TM.QUnit.defaultUser_Admin                      = "qa_admin";
window.TM.QUnit.defaultUser_Editor                     = "qa_editor";
window.TM.QUnit.defaultUser_Reader                     = "qa_reader";
window.TM.QUnit.defaultPassord_Admin                   = "!!tmbeta";
window.TM.QUnit.defaultPassord_Editor                  = "!!tmbeta";
window.TM.QUnit.defaultPassord_Reader                  = "!!TMbeta";

//Individual trace options
window.TM.WebServices.Helper.trace                     = false;
window.TM.Gui.AppliedFilters.trace                     = false;
window.TM.WebServices.trace                            = false;