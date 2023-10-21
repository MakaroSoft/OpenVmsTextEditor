# OpenVMS file structure for the project
This folder contains the required files for zipping up and transfer to the OpenVMS server.

The folders are:
+ BIN - The commands used to run the OpenVMS file explorer
+ CONF - The configuration files for setting up the Java version and logicals for the project
+ LIB - jar files for the project
+ LOG - log files

## OpenVMS Requirements
VSI OpenVMS I64 Version 8.4-2L1 and Java 8.0-372B

You can download the OpenJDK here: https://vmssoftware.com/products/openjdk/

Make sure you follow the system requirements.

## Setup
Enter the following commands into __sys$manager:systartup_vms.com__, but don't execute them before the configuration is finished. Replace __@DKB1:[makarosoft.vmsExplorer__ with the actual location of the files.
```
$! Start up the MakaroSoft VMS explorer
$ @DKB1:[makarosoft.vmsExplorer.CONF]setup_logicals.com
$ @ms$vmsexplorer_home:[bin]vmsexplorer_submit
```
