<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
    	<head><title><xsl:value-of select="Document/Head/Title/."/></title>
    	
      </head>
      <style>
        Body, TD, P
        {
          font-family: verdana;
          font-size: 12px;
        }
        
        H1
        {
			font-size: 16px;
			font-weight: bold;
			color: #515151;
        }
        
        H2
        {
        	font-family: Verdana;
			font-size: 14px;
			font-weight: bold;
			padding: 3px;
			color: #515151;
			padding-left: 4px; /*background-color: #DEDEDE;*/
			background-color: #F5F5F5;
			border-bottom: solid 1px #DEDEDE;
			border-left: solid 4px #DEDEDE;
			border-top: solid 1px #DEDEDE;
			border-right: solid 4px #DEDEDE;
			vertical-align: middle;
			height: 22px;
			margin-top: 0px;
        }
      </style>
      <body>
          
          <xsl:for-each select="Document/Body/.">
              	<xsl:apply-templates/>
          </xsl:for-each>
      </body>
    </html>
  </xsl:template>
  

  				<xsl:template match="h1">
  					<h1><xsl:value-of select="." disable-output-escaping="yes"/></h1>
				</xsl:template>
				<xsl:template match="h2">
  					<h2><xsl:value-of select="." disable-output-escaping="yes"/></h2>
				</xsl:template>
				<xsl:template match="p">
  					<p><xsl:value-of select="." disable-output-escaping="yes"/></p>
				</xsl:template>
				<xsl:template match="a">
  					<a href='{@href}' target='{@target}'><xsl:value-of select="." disable-output-escaping="yes"/></a>
				</xsl:template>
				<xsl:template match="ul">
  					<ul>
  						<xsl:for-each select="li">	
  							<li><xsl:apply-templates/></li>
         				</xsl:for-each>
  					</ul>
				</xsl:template>
				<xsl:template match="ol">
  					<ol>
  						<xsl:for-each select="li">	
  							<li><xsl:apply-templates/></li>
         				</xsl:for-each>
  					</ol>
				</xsl:template>
				


</xsl:stylesheet>