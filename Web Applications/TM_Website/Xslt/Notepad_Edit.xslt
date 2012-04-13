<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" doctype-system="about:legacy-compat" />


  <xsl:template match="/">    
    <!--<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html></xsl:text>-->
    <html>
      <head>
        <title>TeamMentor 'Notepad' Editor</title>        
        <link rel="stylesheet" href="/javascript/bootstrap/bootstrap.min.css" type="text/css"></link>
        <script src="/Javascript/jQuery/jquery-1.7.1.min.js"                  type="text/javascript"></script>
        <script src="/Javascript/jQuery/jquery.textarea.js"                   type="text/javascript"></script>

        <script src="/Javascript/TM/GlobalVariables.js"                       type="text/javascript"></script>
        <script src="/Javascript/TM/Settings.js"                              type="text/javascript"></script>
        <script src="/Javascript/TM/WebServices.js"                           type="text/javascript"></script>
        <script src="/Javascript/TM/Events.js"                                type="text/javascript"></script>
        <script src="/Javascript/TM.Gui/TM.Gui.CurrentUser.js"                type="text/javascript"></script>
        <style>
          .Content  { position: fixed;    width:95%; top:210px; bottom:10px;  left:20px;  }
          .Footer   { position: fixed;    left: 0;  right: 0;   bottom: 0;  text-align:center}                                    
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
            var id = '<xsl:value-of select='//Metadata/Id'/>';            
            var title = '<xsl:value-of select='//Metadata/Title'/>';            
            
            document.title = "Editing: " + title;
            
            var onSave = function(result)
              {
                if (result)
                  $("#SaveButton text").html(' Saved Ok');
                else
                  $("#SaveButton text").html(' Saved Failed!!!');
              }
              
            var saveContent = function()
              {                
                var html =      $(".Content").val(); 
                var dataType =  $("#DataType").val(); 
                $("#SaveButton text").html(' Saving Content');
                TM.WebServices.WS_Libraries
                              .set_Article_Content(id, dataType, html, onSave , function(error) { alert(error.responseText)});            
                return false;
              }
          
             var setSaveButtonText = function()
              {
                $("#SaveButton text").html('  Save changes');
              }
              
             var openArticle = function()
              {
                //window.open("/article/"+id);
                document.location = "/xsl/"+id;
                return false;
              }
              
             var setupGui = function()
              {
                if (TM.Gui.CurrentUser.isEditor())
                {
                  $("#SaveButton").click(saveContent);
                  $.ctrl('S', saveContent);
                  $(".Content").keydown(setSaveButtonText);
                  $("#OpenArticleMessage").css({ left: 10 , top: 2  , position : 'relative'});
                  $("#OpenArticle").click(openArticle);
                  setSaveButtonText();     
                  $("textarea").tabby();
                  $("body").show();                  
                }
                else
                  document.location = '/Login';
              }
            var addPreviewDiv = function()
              {
                $("&lt;" + "iframe>").appendTo("body")
                     .css({ 
                              position : 'absolute' , 
                              top : 0 , height : '100%' , width: '70%' , left : '29.7%'  , 
                              border : 'solid 3px' ,
                              background : 'white'
                          } )
                     .attr('id', 'preview')                 
                     .hide();
                     
                $("#OpenArticle").mouseleave(function() { $("#preview").hide().attr('src','about:blank') });
                $("#OpenArticle").mouseenter(function() { $("#preview").show().attr('src','/xsl/' + id) });
              }
              
            $(function() 
                {
                  $("body").hide();                 
                  addPreviewDiv();
                  TM.Events.onUserDataLoaded.add(setupGui);       
                  TM.Gui.CurrentUser.loadUserData();
                });
        </script>
        
          <xsl:apply-templates select="*"/>
        
        <script src="/Javascript/Gauges/Gauges_Tracking_Code.js"   type="text/javascript"></script>     
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Metadata">
    <div class="form-actions">
      <h2>
        <xsl:value-of select='Title'/>        
      </h2>
      id: <xsl:value-of select="Id"/>
    </div>
    <div class="container-fluid">      
      <button type="submit" class="btn btn-primary" id="SaveButton">
        <i class="icon-file icon-white"></i> <text></text>
      </button>
      <span id="OpenArticleMessage" >
        <a href="" type="submit"  id="OpenArticle">Back to Article</a> (hover to see it)
      </span>
    </div>    
    
  </xsl:template>

  <xsl:template match="Content">
    <br/>

    <div class="container-fluid">
      <input type ="input" id="DataType" class="span1">
        <xsl:attribute name="value">
          <xsl:value-of select="@DataType"/>
        </xsl:attribute>
      </input> DataType
    </div>
    
    <textarea class="Content" id="Content">
        <xsl:value-of select="Data" disable-output-escaping="yes"/>
    </textarea>

    <footer class="Footer">
      Powered by <a href="http://www.securityinnovation.com/products/secure-development-knowledgebase.html">Team.Mentor </a> 
    </footer>

    
    
  </xsl:template>
</xsl:stylesheet>
