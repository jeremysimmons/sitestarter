mkdir "Results"
java -jar ../../../lib/selenium-server.jar -htmlSuite "*firefox" "http://localhost/SiteStarter/" "SiteStarter.Index.html" "Results/Results.html"
pause