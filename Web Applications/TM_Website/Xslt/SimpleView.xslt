<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <html>
      <body >
        <h1>TeamMentor Article (via XSLT transformation)</h1>
        <xsl:apply-templates select="*"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Metadata">
    <hr/>
    <h2>1) Metadata:</h2>
    <hr/>
    <ul>
      <b>Title:</b>
      <xsl:value-of select="Title"/>
      <b>Technology:</b>
      <xsl:value-of select="Technology"/>
      <br/>
      <b>Phase:</b>
      <xsl:value-of select="Phase"/>
      <br/>
      <b>Type:</b>
      <xsl:value-of select="Type"/>
      <br/>
      <b>Category:</b>
      <xsl:value-of select="Catrgory"/>
      <br/>
      <b>Id:</b>
      <xsl:value-of select="Id"/>
      <br/>
    </ul>
  </xsl:template>

  <xsl:template match="Content">
    <hr/>
    <h2>2) Content (raw):</h2>
    <hr/>
    <textarea style="width:100%;height:400px">    
      <xsl:value-of select="Data" disable-output-escaping="yes"/>    
    </textarea>
    <br/>
    <hr/>    
    <h2>3) Content (Html formated):</h2>
    <hr/>
    <xsl:value-of select="Data" disable-output-escaping="yes"/>
    <hr/>
    
  </xsl:template>
</xsl:stylesheet>
