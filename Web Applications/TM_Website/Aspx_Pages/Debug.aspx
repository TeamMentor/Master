<%@ Page Language="C#"  Trace="true" %>
<%@ Import Namespace="FluentSharp.CoreLib" %>
<%@ Import Namespace="FluentSharp.CoreLib.API" %>
<%@ Import Namespace="FluentSharp.WinForms" %>
<%@ Import Namespace="TeamMentor.CoreLib" %>

<%	

	var tmWebServices = new TM_WebServices();	
	tmWebServices.tmAuthentication.mapUserRoles(true);

    UserGroup.Admin.assert();
    UserRole.Admin.demand();
    
    //UserGroup.Admin.setThreadPrincipalWithRoles();           // Assert admin privs - only use for special debugging/deployment issues
	if (tmWebServices.RBAC_IsAdmin().isFalse())
	{
		Trace.IsEnabled = false;			//disable tracing or it will still show on the page
		Response.Redirect("/Login?LoginReferer=/debug");		
	}
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href ="/Css/default.css" type="text/css" />
    <title>TeamMentor Debug Page</title>
</head>
<body>    
       <h1> Debug page for TeamMentor</h1>
        <br />
        <h2>TM Config values</h2>
        <hr />
        <div>                                    
            <strong>TM_Xml_Database.Current.Path_XmlDatabase:</strong>  <%=TM_Xml_Database.Current.Path_XmlDatabase %><br />
            <strong>TM_Xml_Database.Current.Path_XmlLibraries:</strong> <%=TM_Xml_Database.Current.Path_XmlLibraries %><br />
            <strong>TM_Server.Location:</strong> <%=TM_Xml_Database.Current.get_Path_TMServer_Config() %><br />            
            <strong>TM_Config.Location:</strong> <%= TM_UserData.Current.tmConfig_Location() %><br />
            <strong>UserData.Path_UserData:</strong> <%= TM_UserData.Current.Path_UserData %><br />
            <hr />
            <strong>AppDomain.CurrentDomain.BaseDirectory:</strong> <%=AppDomain.CurrentDomain.BaseDirectory%><br />
            <strong>O2 Temp Dir:</strong> <%= PublicDI.config.O2TempDir%><br />
            <strong>TM_Status:</strong>
            <pre><%= TM_Status.Current.toXml().htmlEncode()%></pre>
        </div>
        
        <br /><h2>Session Values</h2>
        <hr />
        <ul>
        <% foreach (string key in Session.Keys)
               Response.Write("<li>{0}: {1}</li>".format(key,Session[key]));
        %>
            </ul>
        <br />
        <h2>Logs</h2>
        <hr />
        <pre>
<% 
        var logs = tmWebServices.GetLogs();
        foreach (var line in logs.lines())
        {
            var color = "black";
            if (line.starts("DEBUG:"))
                color = "green";
            else if (line.starts(new List<string> {"ERROR:", "Exception:"}))
                color = "red";
            Response.Write("<span style='color:{0}'>{1}<span><br/>".format(color,line.htmlEncode()));
        }
    %>
        </pre>        
        <br /><h2>Trace Data</h2>
        <hr />
        
</body>
</html>
