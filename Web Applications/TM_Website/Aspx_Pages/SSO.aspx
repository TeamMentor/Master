<%@ Page Language="C#"%>
<%@ Import Namespace ="O2.DotNetWrappers.ExtensionMethods" %>
<%@ Import Namespace="TeamMentor.CoreLib" %>
<link href="../Javascript/bootstrap/bootstrap.v.1.2.0.css" rel="stylesheet" type="text/css" />

<%
    var xmlDatabase     = TM_Xml_Database.Current;
    var userData        = xmlDatabase.UserData;
    var authentication  = new TM_Authentication(null);
    var request         = HttpContextFactory.Request;
    var response        = HttpContextFactory.Response;
    var ssoKey          = string.Empty;
    
    var userName        = request["userName"].urlEncode();
    var requestToken    = request["requestToken"].urlEncode();
    
    //Safe check for the username
    if (userName.isNull())
    {
        "[TM SSO] Username is null or empty".error();
        throw new ArgumentException("Username is null or empty. Please verify");
    }
    //Safe check for Secret data.
    if (userData.notNull() && userData.SecretData.notNull())
    {
        ssoKey = userData.SecretData.Rijndael_Key;
        if (ssoKey.isNull())
        {
            "[TM SSO] SSO Key not found.: {0} {1}".error(userName, requestToken);
            throw new ArgumentException("SSO Key not found. Please verify Secret data");
        }
    }
    //Computing expected token.
    var expectedToken = (userName.Trim() + ssoKey.Trim()).md5Hash();
    try 
	{
        if (userName.valid() && requestToken.valid() && expectedToken == requestToken)
        {
            var tmUser = userName.tmUser();             // see if there is a user with the provided value
            if (tmUser.isNull())
            {
                "[TM SSO] Username does not exist in TeamMentor".error();
                throw new Exception(String.Format("Username {0} not found in TeamMentor.Please verify.", userName));
            }
            var loginGuid = tmUser.login();             // login user in TM   
            authentication.sessionID = loginGuid;       // triggers the update of user's cookies
            response.Redirect("/teammentor");           // redirects user to /teammentor
        }
        else
        {
            "[TM SSO] Failed to SSO with the values provided: {0} {1}".error(userName, requestToken);
            throw new ArgumentException("[TM SSO] Failed to SSO with the values provided: {0} {1}".error(userName, requestToken));
        }
    }
    catch (Exception ex)
    {
        ex.log();
    }
%> 
<!DOCTYPE html>
    <html>
      <head>
        <link   href="/Javascript/bootstrap/2.3.1/css/bootstrap-responsive.css" rel="stylesheet" type="text/css" />
        <link   href="/Javascript/bootstrap/2.3.1/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
        
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>TeamMentor SSO</title>
      </head>
    <body>        
          <div class="container">          
              <div class="hero-unit">
                <h3>
                  TeamMentor SSO Error
                </h3>
                <p>
                    It was not possible to login using the security token provided, please contact <a href="mailto:support@securityinnovation.com">support@securityinnovation.com</a> if this is affecting you.
			   </p>
            </div>
        </div>
    </body>
</html>
