mkdir "Results"
java -jar ../../../lib/selenium-server.jar -htmlSuite "*firefox" "http://localhost:80" "SiteStarter.Index.html" "Results/Results.html"
pause