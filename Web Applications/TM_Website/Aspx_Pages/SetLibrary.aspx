<%@ Page Language="C#"  %>
<%@ Import Namespace ="O2.DotNetWrappers.ExtensionMethods" %>
<%@ Import Namespace ="SecurityInnovation.TeamMentor.WebClient.WebServices" %>
<%
    //we have to do this since it is the only place where the session is avaiable    
    if (Request["Library"] == "C  ")
        Session["Library"] = "C++";
    else
        Session["Library"] = Server.HtmlEncode(Request["Library"]);    
        
    Response.Redirect("/");
%>        
