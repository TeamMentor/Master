@echo off
start "WebServer" "WebServer/CassiniDev.exe" /port:12345 /portMode:Specific /path:"Web Applications\TM_Website"
ping 123.45.67.89 -n 1 -w 3000 > nul
start http://127.0.0.1:12345
