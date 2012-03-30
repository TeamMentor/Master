@echo off
start "WebServer" "Tools/CassiniDev_4.0.exe" /port:12115 /portMode:Specific /path:"Web Applications\TM_Website"
ping 123.45.67.89 -n 1 -w 3000 > nul
start http://127.0.0.1:12115
