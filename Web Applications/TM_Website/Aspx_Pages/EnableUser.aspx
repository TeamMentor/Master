<%@ Page Language="C#" %>

<%@ Import Namespace="FluentSharp.CoreLib" %>
<%@ Import Namespace="FluentSharp.WinForms" %>
<%@ Import Namespace="TeamMentor.CoreLib" %>
 
<%
    
    var enableToken = Request.QueryString["token"].urlEncode();
    var tmUser = enableToken.guid().enableUser_UsingToken();
    var message = (tmUser.notNull())
        ?  "Thanks for enabling an TeamMentor Account<br/><br/>The user '{0}' will be able to login now.".format(tmUser.UserName.htmlEncode())
        :  "Sorry could not enable the user. The Token is invalid or it has been used already";
         
    {}
    
    
%>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml"> 
 
<head> 
	<meta http-equiv="Content-type" content="text/html; charset=utf-8" /> 
	<title>TeamMentor | Error</title> 
	
	<link rel="stylesheet" href="/css/ThemeForest_ReadyMade/css/reset.css"      type="text/css" media="screen" title="no title" />
	<link rel="stylesheet" href="/css/ThemeForest_ReadyMade/css/text.css"       type="text/css" media="screen" title="no title" />
	<link rel="stylesheet" href="/css/ThemeForest_ReadyMade/css/buttons.css"    type="text/css" media="screen" title="no title" />
	<link rel="stylesheet" href="/css/ThemeForest_ReadyMade/css/login.css"      type="text/css" media="screen" title="no title" />
	<link rel="stylesheet" href="/css/ThemeForest_ReadyMade/css/forms.css"      type="text/css" media="screen" title="no title" />
	
	<script src="/aspx_Pages/scriptCombiner.ashx?s=WebServices_JS" type="text/javascript"></script>


</head> 
 
<body> 

<div id="login">
	
	<div id="login_panel">		
	<a href="/TeamMentor"><img src='/Images/HeaderImage.jpg' id='SI_Logo'/>	</a>
		<form class="form">					
		    <div class="login_fields">
		        <%= message %>
		        
             </div>
		</form>
	</div> 
</div> 

</body> 
 
</html>