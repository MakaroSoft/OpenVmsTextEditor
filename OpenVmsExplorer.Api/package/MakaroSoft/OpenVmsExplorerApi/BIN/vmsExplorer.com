$ set noverify
$ @ms$vmsExplorer_home:[conf]setupJava
$ 
$ define/nolog CLASSPATH "../lib/OpenVmsExplorerApi-1_0_0.jar:../lib/log4j-core-2_20_0.jar:../lib/log4j-api-2_20_0.jar:../lib/gson-2_8_0.jar:../conf/:"
$
$java -mx64m "makarosoft.vmsExplorer.Engine" -port 8001
