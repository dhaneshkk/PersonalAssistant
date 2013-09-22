echo off
REM ZeroMQ Pub-Sub pattern example 3
REM Two Pub and one Sub
REM Author: Manar Ezzadeen
REM Blog  : http://idevhawk.phonezad.com
REM Email : idevhawk@gmail.com

cd /d %~dp0
start "Personal Server" cmd /T:8E /k PAServer.exe -c tcp://127.0.0.1:5000 -d 0

