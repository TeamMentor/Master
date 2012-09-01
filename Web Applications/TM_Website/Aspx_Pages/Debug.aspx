<%@ Page Language="C#"  %>
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
            <strong>AppDomain.CurrentDomain.BaseDirectory:</strong> <%=AppDomain.CurrentDomain.BaseDirectory%><br />
        </div>
    
</body>
</html>
