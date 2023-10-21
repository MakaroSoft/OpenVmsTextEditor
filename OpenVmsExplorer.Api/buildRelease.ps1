Copy-Item "target\gson-2.8.0.jar" -Destination "package\MakaroSoft\OpenVmsExplorerApi\LIB\gson-2_8_0.jar"
Copy-Item "target\log4j-api-2.20.0.jar" -Destination "package\MakaroSoft\OpenVmsExplorerApi\LIB\log4j-api-2_20_0.jar"
Copy-Item "target\log4j-core-2.20.0.jar" -Destination "package\MakaroSoft\OpenVmsExplorerApi\LIB\log4j-core-2_20_0.jar"
Copy-Item "target\OpenVmsExplorerApi-1_0_0.jar" -Destination "package\MakaroSoft\OpenVmsExplorerApi\LIB"
Compress-Archive -Path package\MakaroSoft\ -DestinationPath package\OpenVmsExplorerApi-1_0_0.zip -Force