$ SET NOVERIFY
$ @MS$VMSEXPLORER_HOME:[CONF]SETUP_JAVA
$ define/job JAVA$ENABLE_ENVIRONMENT_EXPANSION TRUE
$
$ set noverify
$java "-Djava.util.logging.config.file=[]logging.properties" -mx64m -cp "../lib/OpenVmsExplorerApi-1_0_0-1_0_0.jar:../lib/log4j-core-2_20_0.jar:../lib/log4j-api-2_20_0.jar:../lib/gson-2_8_0.jar" "makarosoft.vmsExplorer.Engine" -port 8001
