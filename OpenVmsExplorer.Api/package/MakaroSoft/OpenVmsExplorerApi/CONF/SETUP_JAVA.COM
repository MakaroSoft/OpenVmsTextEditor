$ 	@SYS$STARTUP:OPENJDK$SETUP.COM
$!       used to be 8 which worked for java$80 but not openjdk
$	 DEFINE/JOB/NOLOG JAVA$FILENAME_CONTROLS 0
