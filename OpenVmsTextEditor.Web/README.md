# OpenVmsTextEditor.Web
A browser-based explorer and editor for editing text files on an OpenVMS server. This project uses the OpenVmsTextEditor.Editor for doing the text editing and the OpenVmsTextEditor.Api for loading and saving the files.

> [!WARNING]
> This version is __beta__ and does not have authentication, so if you run it, make sure it runs behind a firewall. 

This project uses two plugins which exist in the Infrastructure project.
+ WindowsIo.cs - This plugin is used for testing and is defined in appsettings.Development.json
+ VmsIo.cs - This plugin uses the Restful API defined by the [OpenVmsExplorer.Api](../OpenVmsExplorer.Api) project and is defined in appsettings.json

## Requirements
+ .net 6
+ Visual Studio Community 2022

# Setting up libman cli
If you don't have the libman cli installed, enter the following command:
```
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

## Build

You can either use VS2022 or the dotnet cli. Ensure you are in the folder containing the *.sln file and enter the following.
```
dotnet build
cd OpenVmsTextEditor.Web
libman restore
cd ../
```

## Running
Open the *.sln file up in VS2022 and run the OpenVmsTextEditor.Web project. This will run in debug mode, and there will not be any configuration changes you
will have to make.

## Configure
When running in release mode, you will have to make the following changes in appsettings.json. Change the IP address and port number to the location and port
number of your OpenVMS explorer Restful API.
```
"VmsExplorerApiUrl": "http://127.0.0.1:8001",

```
