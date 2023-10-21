# OpenVmsTextEditor.Editor
A Node.js project using [Codemirror6](https://codemirror.net/) to create an editor with a rudimentary DecBASIC language parser. This project is used by the 
OpenVmsTextEditor.Web project.

## Build
Make sure you are in the root folder of the OpenVmsTextEditor.Editor project.
Running the following commands will generate an __OpenVms.Editor.Bundle.js__ file in ..\OpenVmsTextEditor.Web\OpenVmsTextEditor.Web\wwwroot\js
```
npm install
npm run build
npm run test
```
The build generates an __OpenVmsEditor.bundle.js__ file and places it into the [OpenVmsTextEditor.Web wwwroot/js](../OpenVmsTextEditor.Web/OpenVmsTextEditor.Web/wwwroot/js) folder.
