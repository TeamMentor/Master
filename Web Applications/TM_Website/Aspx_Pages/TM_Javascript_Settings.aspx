<%@ Page Language="C#" %>
<%@ Import Namespace="TeamMentor.CoreLib" %>
 
<%
    HttpContextFactory.Response.ContentType = "application/javascript";
    var tmConfig = TMConfig.Current;
    
    //tmConfig.TMSecurity.Show_LibraryToAnonymousUsers = false;
    //tmConfig.WindowsAuthentication.Enabled           = false;
    
    var showLibraryStructureToAnonymous = "true";    
    var windowsAuthentication           = "false";
    var notAuthorizedPage               = "/Html_Pages/Gui/Panels/User_Not_Logged.html";
    
    if (tmConfig.TMSecurity.Show_LibraryToAnonymousUsers == false)
    {
        showLibraryStructureToAnonymous = "false";                  
    }
    if (tmConfig.WindowsAuthentication.Enabled)
    {
        notAuthorizedPage               = "/Html_Pages/Gui/Panels/AD_Non_Authorized_User.html";
        windowsAuthentication           = "true";
    }
    
        
%>        
<!-- dynamic Javascript settings -->

window.TM.Gui.showLibraryStructureToAnonymous =  <%= showLibraryStructureToAnonymous%>;
window.TM.WindowsAuthentication               =  <%= windowsAuthentication%>;
window.TM.UserNotLoggedInPage                 = "<%= notAuthorizedPage %>"; 
window.TM.NotAuthorizedPage                   = "<%= notAuthorizedPage %>";

<!-- static mappings -->
window.TM.Debug.addTimeStampToLoadedPages     = false; 