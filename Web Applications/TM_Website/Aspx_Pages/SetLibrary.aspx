<%@ Page Language="C#"  %>
<%
    //we have to do this since it is the only place where the session is avaiable    
    if (Request["Library"] == "C  ")
        Session["Library"] = "C++";
    else
        Session["Library"] = Server.HtmlEncode(Request["Library"]);    
        
    Response.Redirect("/");
%>        
