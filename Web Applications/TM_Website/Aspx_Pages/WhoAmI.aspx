<%@ Page Language="C#" %>

<%@ Import Namespace="FluentSharp.CoreLib" %>
<%@ Import Namespace="FluentSharp.WinForms" %>
<%@ Import Namespace="TeamMentor.CoreLib" %>
 
<%
    var whoAmI = "Annonymous User";
    var tmAuthentication = new TM_Authentication(null);
    var currentUser      = tmAuthentication.whoAmI(); 
    if (currentUser.notNull())
    {
        whoAmI = 
@"UserName:{0}
UserId:{1}
GroupName:{2}
UserRoles:{3}".format("a","b","c","d");
    }    
        
%>


<%= whoAmI %>