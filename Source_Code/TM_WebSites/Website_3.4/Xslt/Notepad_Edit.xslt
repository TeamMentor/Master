<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" doctype-system="about:legacy-compat" />


  <xsl:template match="/">    
    <!--<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html></xsl:text>-->
    <html>
      <head>
        <title>TeamMentor 'Notepad' Editor</title>        
		    <link rel="stylesheet" href="/Css/NotepadEditor.css" type="text/css"></link>
        <script src="/Javascript/jQuery/jquery-1.7.1.min.js"                  type="text/javascript"></script>
        <script src="/Javascript/jQuery/jquery.textarea.js"                   type="text/javascript"></script>
				
				
        <script src="/Javascript/TM/GlobalVariables.js"                       type="text/javascript"></script>
        <script src="/Javascript/TM/Settings.js"                              type="text/javascript"></script>
        <script src="/Javascript/TM/WebServices.js"                           type="text/javascript"></script>
        <script src="/Javascript/TM/Events.js"                                type="text/javascript"></script>
        <script src="/Javascript/TM.Gui/TM.Gui.CurrentUser.js"                type="text/javascript"></script>
        <script src="/Javascript/jQuery/jquery.validate.min.js"               type="text/javascript"></script>
        <script src="/Javascript/jO2/jO2_jQuery_ExtensionMethods.js"          type="text/javascript"></script>

				<script src="/Javascript/json/json2.js"																type="text/javascript"></script>
				<script src="/Javascript/IE_Fixes.js"																	type="text/javascript"></script>
      </head>
      <body>

        <span id="DataType_RawValue" class="NEHiddenValue"><xsl:value-of select="*/Content/@DataType"/></span>        
        <span id="Title_RawValue"    class="NEHiddenValue"><xsl:value-of select="*/Metadata/Title"/></span>        
        <span id="Id_RawValue"       class="NEHiddenValue"><xsl:value-of select="*/Metadata/Id"/></span>    
        
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
          
            var id       = $("#Id_RawValue").html(); 
            var title    = $("#Title_RawValue").html();
            var dataType = $("#DataType_RawValue").html().toString().toLowerCase();
            
            document.title = "Editing: " + title;
            
            var onSave = function(result)
              {
                if (result)
                  $("#SaveButton").html('Saved OK');
                else
                  $("#SaveButton").html('Save Failed');
              }
              
            var saveContent = function()
              {                
                var html =      $(".Content").val(); 
                var dataType =  $("#DataType").val(); 
                $("#SaveButton").html('Saving Content');
                TM.WebServices.WS_Libraries
                              .set_Article_Content(id, dataType, html, onSave , function(error) { alert(error.responseText)});            
                return false;
              }
          
             var setSaveButtonText = function()
              {
                $("#SaveButton").html('Save Changes');
              }
              
             var openArticle = function()
              {
                //window.open("/article/"+id);
                document.location = "/article/"+id;
                return false;
              }
              
             var setupGui = function()
              {
                if (TM.Gui.CurrentUser.isEditor())
                {
                  $("#SaveButton").click(saveContent);
                  $.ctrl('S', saveContent);
                  $(".Content").keydown(setSaveButtonText);
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
                     
                /*$("#OpenArticle").mouseleave(function() { $("#preview").hide().attr('src','about:blank') });
                $("#OpenArticle").mouseenter(function() { $("#preview").show().attr('src','/xsl/' + id) });*/
              }
              
            $(function() 
                {
                  $("body").hide();                 
                  addPreviewDiv();
                  TM.Events.onUserDataLoaded.add(setupGui);       
                  TM.Gui.CurrentUser.loadUserData();
                  $("#DataType").val(dataType);
                });
        </script>
        
        <xsl:apply-templates select="*"/>
      </body>
    </html>
  </xsl:template>


                
  <xsl:template match="Metadata">
    <div class="NEHeader">
      <h2>
        <xsl:value-of select='Title'/>        
      </h2>
      id: <xsl:value-of select="Id"/>
    </div>
    <span class="NEToolbar">
		<a href="" type="submit" class="NEButton" id="OpenArticle">View Article</a>	
		<a href="" type="submit" class="NEButton" id="SaveButton">Save Changes
		</a>
    </span>
  </xsl:template>

  <xsl:template match="Content">
	<span class="NEDataTypeLabel">Data Type:</span>
    <select id="DataType" class="NEDataType">
      <option value ="html">Html</option>
      <option value ="wikitext">WikiText</option>      
     </select>
    <span id="WikiTextHelp">WikiText uses <a href="http://docs.teammentor.net/article/Team Mentor Wiki Markup" target="_blank"> WikiCreole</a> </span>
	
	<div class="NEContent">
    <textarea class="Content" id="Content">
        <xsl:value-of select="Data" disable-output-escaping="yes"/>
    </textarea>
	</div>
  </xsl:template>
</xsl:stylesheet>
