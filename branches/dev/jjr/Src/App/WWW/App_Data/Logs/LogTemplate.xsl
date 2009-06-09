<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <style>
        Body, TD
        {
          font-family: verdana;
         font-size: 12;
        }
      </style>
      <body>
        <h2>Log</h2>
        <table>
          <xsl:for-each select="Log/Entry">
            <tr>
              <td>
                <xsl:value-of select="Timestamp"/>
              </td>
              <td>
                <table>
                  <tr>
                    <xsl:call-template name="IndentLoop">
                      <xsl:with-param name="i">0</xsl:with-param>
                      <xsl:with-param name="count">
                        <xsl:value-of select="Indent"/>
                      </xsl:with-param>
                    </xsl:call-template>
                    <td>
                      <font color="gray">
                      <xsl:value-of select="Component"/>.<xsl:value-of select="Method"/>
                      </font><br/>

                      <xsl:value-of select="Data"/>
                    </td>
                  </tr>
                </table>

              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>

  <!--begin_: Define_The_Output_Loop -->
  <xsl:template name="IndentLoop">

    <xsl:param name="i"      />
    <xsl:param name="count"  />

    <!--begin_: Line_by_Line_Output -->
    <xsl:if test="$i &lt;= $count">
      <td>&#160;&#160;&#160;&#160;</td>
    </xsl:if>

    <!--begin_: RepeatTheLoopUntilFinished-->
    <xsl:if test="$i &lt;= $count">
      <xsl:call-template name="IndentLoop">
        <xsl:with-param name="i">
          <xsl:value-of select="$i + 1"/>
        </xsl:with-param>
        <xsl:with-param name="count">
          <xsl:value-of select="$count"/>
        </xsl:with-param>
      </xsl:call-template>
    </xsl:if>

  </xsl:template>


</xsl:stylesheet>