<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" doctype-system="about:legacy-compat" />


  <xsl:template match="/">    
    <!--<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html></xsl:text>-->
    <html>
      <head>
        <title>TeamMentor Article Editor</title>        
        <link rel="stylesheet" href="/javascript/bootstrap/bootstrap.min.css" type="text/css"></link>
        <script src="/Javascript/jQuery/jquery-1.7.1.min.js"                  type="text/javascript"></script>
        <script src="/Javascript/jQuery/jquery.textarea.js"                   type="text/javascript"></script>

        <script src="/Javascript/TM/GlobalVariables.js"></script>
        <script src="/Javascript/TM/Settings.js"></script>
        <script src="/Javascript/TM/WebServices.js"></script>
        <script src="/Javascript/TM/Events.js"></script>
        <script src="/Javascript/TM.Gui/TM.Gui.CurrentUser.js"></script>
        <style>
          .Content { width:95%; height:300px; }
          .Footer  { text-align:center}                    
        </style>
      </head>
      <body>
        <script>
           jQuery.ctrl      = function(key, callback, args)                                       //from http://www.gmarwaha.com/blog/2009/06/16/ctrl-key-combination-simple-jquery-plugin/
                                    {
                                        $(document).keydown(function(e) 
                                        {
                                            if(!args) args=[];                              // IE barks when args is null
                                            if(e.keyCode == key.charCodeAt(0) &amp;&amp; e.ctrlKey) 
                                            {
                                                callback.apply(this, args);
                                                return false;
                                            }
                                         });
                                    };
        </script>
        <script>
            var title =    '<xsl:value-of select='//Metadata/Title'/>';                   
                        
            var onCreate = function(result)
              {
                if (result != TM.Const.emptyGuid)
                  document.location = "/xsl/" + result;
                else
                  $("#SaveButton text").html(' Saved Failed!!!');
              }
              
            var saveContent = function()
              { 
                var libraryId = $("#library").val();                
                var title = $("#title").val();                
                var html = $(".Content").val();  
                var dataType = $("#dataType").val();
                $("#SaveButton text").html(' Creating Article');
                TM.WebServices.WS_Libraries.add_Article_Simple(libraryId, title, dataType, html,onCreate, function(error) { alert(error.responseText)});
                return false;
              }
          
             var setSaveButtonText = function()
              {
                $("#SaveButton text").html('  Save changes');
              }
                           
              
             var setupGui = function()
              {
                if (TM.Gui.CurrentUser.isEditor())
                {
                  $("#SaveButton").click(saveContent);
                  $.ctrl('S', saveContent);
                  $(".Content").keydown(setSaveButtonText);
                  setSaveButtonText();     
                  $("textarea").tabby();
                                    
                  $("body").show();                  
                  loadLibraries();
                  
                  $("#title").val(title);        
                  $("#dataType").val('html');
                }
                else
                  document.location = '/Login';
              }
              
            $(function() 
                {
                  $("body").hide();                  
                  TM.Events.onUserDataLoaded.add(setupGui);       
                  TM.Gui.CurrentUser.loadUserData();
                });
                
           //Extra methods for  Create
           
           var loadLibraries = function()
            {
            
              var populateSelectWithCurrentLibraries = function(libraries)
                {
                  $.each(libraries , function() 
                    { 
                        $("#library").append($("&lt;option>").append(this.Caption).attr('value', this.Id ) )
                    });
                } 
              TM.WebServices.WS_Libraries.get_Libraries(populateSelectWithCurrentLibraries );
            }
        </script>

        <div class='container-fluid'>          
          
          
          <h2>Create TeamMentor Article</h2>
    
          <form class="form-horizontal">
            <fieldset>
              <legend></legend>
                <div class="control-group">
                  <label class="control-label" for="select01">Target Library</label>
                  <div class="controls">
                    <select id="library"/>              
                  </div>
                </div>
        
              <div class="control-group">
                <label class="control-label" for="input01">Title</label>
                <div class="controls">
                  <input type="text" class="input-large" id="title"/>
                </div>
              </div>
              
                    <div class="control-group">
                <label class="control-label" for="input01">DataType</label>
                <div class="controls">
                  <input type="text" class="input-large span1" id="dataType"/>
                </div>
              </div>
              <div class="control-group">
                <label class="control-label" for="input01">Html Content</label>
                <div class="controls">
                  <textarea class="Content" />
                </div> 
              </div>
              
              <div class="control-group">
                <label class="control-label" for="input01"></label>
                <div class="controls">
                  <button type="submit" class="btn btn-primary" id="SaveButton">
                    <i class="icon-file icon-white"></i> <text></text>
                  </button>
                </div>
              </div>
              
        
            </fieldset>
          </form>
        </div>
        
        <footer class="Footer">
      Powered by <a href="http://www.securityinnovation.com/products/secure-development-knowledgebase.html">Team.Mentor </a> 
    </footer>
        <script src="/Javascript/Gauges/Gauges_Tracking_Code.js"   type="text/javascript"></script>     
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
