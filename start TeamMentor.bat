@echo off
rem start "WebServer" "Tools/CassiniDev_4.0.exe" /port:12120 /portMode:Specific /path:"TeamMentor\TM_Website"
start "WebServer" "Tools/CassiniDev_4.0.exe" /port:12120 /portMode:Specific /path:"publish"
ping 123.45.67.89 -n 1 -w 3000 > nul
start http://127.0.0.1:12120