@echo off

set REL_PATH=\SiteStarter.Index.html
set ABS_SUITEPATH=

rem // save current directory
pushd .

rem // change to relative directory and save value of CD (current directory) variable
cd %REL_PATH%
set ABS_SUITEPATH=%CD%

rem // restore current directory
popd

echo Relative path : %REL_PATH%
echo Maps to path  : %ABS_SUITEPATH%

mkdir "Results"

set ABS_SUITEPATH=%ABS_SUITEPATH%%REL_PATH%

java -jar ../../../lib/selenium-server.jar -htmlSuite "*iexplore" "http://localhost/SiteStarter/" "%ABS_SUITEPATH%" "Results/Results.html"

pause