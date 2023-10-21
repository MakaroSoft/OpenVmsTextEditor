# OpenVmsExplorer.Api
A java8 Restful API that runs on the OpenVMS server.

> [!WARNING]
> This version is __beta__ and does not have authentication, so if you run it, make sure it runs behind a firewall.

> [!IMPORTANT]
> The VMS folder has text files in it that are in OpenVMS format. They have the same EOL as unix text files. If you decide to edit
> any of these files before sending them to the OpenVMS server, use an editor that preserves the EOL, such as [notepad++](https://notepad-plus-plus.org/)

## build
Make sure you are in the root folder of the OpenVmsExplorer.Api project. If you have [Maven](https://maven.apache.org/install.html) installed, then enter the following command:
```
mvn clean compile package
```
Prepare the release for transfer to the OpenVMS server. This will copy the required jars into the VMS folder and zip up the release.
```
powershell -file buildRelease.ps1
```

## OpenVMS Requirements
+ OpenVMS I64 Version 8.4-2L1 with latest updates
+ Java8 https://vmssoftware.com/products/openjdk/

## Install
The previous step created an __OpenVmsExplorerApi-1_0_0.zip__ file in the package folder. Transfer the zip file to your OpenVMS server.

> [!NOTE]
> The following steps are all done on your OpenVMS server.

Once the zip file has been transferred, unzip it to the location of your choice. I like to unzip it to disk:[000000]. This will create disk:[MakaroSoft.OpenVmsExplorerApi]

## Configure ms$vmsexplorer_home: logical

> [!NOTE]
> I will be referencing __disk:[makarosoft...]__ in this documentation. You need to replace that with where you actually installed the Explorer code.

Edit `disk:[makarosoft.OpenVmsExplorerApi.CONF]setup_logicals.com` and replace __disk:[makarosoft.OpenVmsExplorerApi__ with the actual location.

Now, you can execute the following command:
```
@disk:[makarosoft.OpenVmsExplorerApi.CONF]setup_logicals.com
```
You can now reference all the files with the __@ms$vmsexplorer_home:__ prefix. Example: @ms$vmsexplorer_home:[bin]vmsexplorer_submit

## Configuration continued
Add the following lines to `sys$manager:systartup_vms.com`. This will make sure the explorer service will start back up on reboot.

```
$!
$! Start up the MakaroSoft VMS explorer
$!
$ @disk:[makarosoft.OpenVmsExplorerApi.CONF]setup_logicals.com
$ @ms$vmsexplorer_home:[bin]vmsexplorer_submit
```

## Start detached process

To run the OpenVMS explorer, execute the following command. This will start up the explorer service as a detached process.
```
$ @ms$vmsexplorer_home:[bin]vmsexplorer_submit
```

# Frequently Asked Questions

+ [How do I change the port?](#how-do-i-change-the-port)
+ [How do I change the user that the detached process is running under?](#how-do-i-change-the-user-that-the-detached-process-is-running-under)

## How do I change the port?
The default port is __8001__ and is defined in __ms$vmsexplorer_home:[bin]vmsExplorer.com__. You need to edit that file and change 8001 to the port number you
want to use.

## How do I change the user that the detached process is running under?
The default user is __system__ and is defined in __ms$vmsexplorer_home:[CONF]vmsExplorer.com__. You need to edit that file and change /USER=SYSTEM to /USER= and the username you want.
