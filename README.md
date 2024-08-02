# OpenVmsTextEditor
A browser-based OpenVMS text editor with a DecBASIC language parser written in NET8 and Node.JS. Also, a Restful API Service that runs on an OpenVMS server using Java8.

> [!WARNING]
> This version is __beta__ and does not have authentication, so if you run it, make sure it runs behind a firewall. 

## Build
The OpenVMS Text Editor has three projects in it (each one has its own build instructions):
  - [OpenVmsTextEditor.Editor](OpenVmsTextEditor.Editor/README.md) - build this one first. 
  - [OpenVmsExplorer.Api](OpenVmsExplorer.Api/README.md) - optional. This project is for the restful API that will run on the VMS server. It is not required for testing.
  - [OpenVmsTextEditor.Web](OpenVmsTextEditor.Web/README.md) - Build and run using Visual Studio Community 2022
