using FluentSharp.Watin;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_WatiN
    {
        
        /// <summary>
        /// calls <code>tmProxy.cache_Reload__Data();</code> before watinIe.reload_LibraryTree()
        /// </summary>
        /// <param name="tmProxy"></param>
        /// <param name="ie"></param>
        /// <returns></returns>
        public static TM_Proxy                  reload_LibraryTree(this TM_Proxy tmProxy,  WatiN_IE ie)
        {
            tmProxy.cache_Reload__Data();
            ie.reload_LibraryTree();
            return tmProxy;
        }
        public static WatiN_IE                  reload_LibraryTree(this WatiN_IE ie)
        {            
            ie.invokeEval(@"TM.Gui.Dialog.alertUser('Refreshing Library Tree ','TM QA script');

                            $('#gui_West_bottom').load('/Html_Pages/Gui/Panels/Left_LibraryTree.html',function()
                                {
                                    TM.Debug.reuseLibraryMappingsObject = false;
                                    TM.Events.onGuiObjectsLoaded();                             
                                    window.TM.Gui.LibraryTree.showTree()
                                    window.TM.Gui.LibraryTree.selectFirstNode();            
                                });
                           ");  
            return ie;
        }
    }
}