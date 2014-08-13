@echo off
rem start "WebServer" "Tools/CassiniDev_4.0.exe" /port:12120 /portMode:Specific /path:"TeamMentor\TM_Website"

start "WebServer" "Tools/CassiniDev_4.0.exe" /port:12120 /portMode:Specific /path:"TeamMentor\Publish\Website.3.5"
ping 123.45.67.89 -n 1 -w 3000 > nul
start http://127.0.0.1:12120


rem start "WebServer" "Tools/CassiniDev_4.0.exe" /port:12121 /portMode:Specific /path:"Source_Code\TM_WebSites\Website_3.5"
rem ping 123.45.67.89 -n 1 -w 3000 > nul
rem start http://127.0.0.1:12121