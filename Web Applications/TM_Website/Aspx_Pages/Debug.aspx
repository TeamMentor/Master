<%@ Page Language="C#"  %>
<%@ Import Namespace ="O2.DotNetWrappers.ExtensionMethods" %>
<%@ Import Namespace ="SecurityInnovation.TeamMentor.WebClient.WebServices" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href ="/Css/default.css" type="text/css" />
    <title>TeamMentor Debug Page</title>
</head>
<body>    
       <h1> Debug page for TeamMentor</h1>
        <br />
        <div>
            <strong>Admins only
            </strong>
            <br />
            <br />
            <strong>TM_Xml_Database.Path_XmlDatabase:</strong> <%=TM_Xml_Database.Path_XmlDatabase %><br />
            <strong>TM_Xml_Database.Path_XmlLibraries:</strong> <%=TM_Xml_Database.Path_XmlLibraries %>
            <strong>AppDomain.CurrentDomain.BaseDirectory:</strong> <%=AppDomain.CurrentDomain.BaseDirectory%><br />
        </div>
        <hr />
        <h1>Session Values</h1>
        <ul>
        <% foreach (string key in Session.Keys)
               Response.Write("<li>{0}: {1}</li>".format(key,Session[key]));
        %>
            </ul>
        <h1>Logs</h1>
        <pre>
<%= new TM_WebServices().GetLogs() %>
        </pre>
        
</body>
</html>
