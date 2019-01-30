# dotnet-impersonate
Based on the HWC project https://github.com/mxplusb/dotnet-hwc

There several ways to launch a process using impersonation on the Windows platform such as RunAs and PSExec (from Microsoft). These commands/tools can be run on Cloud Foundry but there are some limitations such as capturing logging and ensuring that all child processes are terminated when the container is restarted or shut down. This project provides a way to implement simple impersonation using AD accounts on Cloud Foundry. Note: This solution requires modifications to the Windows stem cell to allow communication and authorization with the AD controllers.

## Instructions for Use

To use the launcher:

1. Compile the application and build the impersonate.exe 
2. Add the executable to the push directory of the application that will be pushed using the binary buildpack
3. Modify the manifest for the application to set a custom start command to: impersonate.exe {app.exe} "{optional args}"
	- Example: `impersonate.exe ConsoleApp2.exe`
4. Add environment variables in the manifest or using the cf cli for the account that will be used for impersonation: SERVICE_USERNAME and SERVICE_PASSWORD - if these are not set, the application will execute using the current container user account (vcap)
	- Credentials can be hidden using credhub integration
5. Set the health check type to "none" - process

## Stem Cell Configuration

Please see the HWC Buildpack Project referenced above for setting up the stem cells. Future updates will be provided in this section on configuring Windows stem cells for leveraging AD based authentication. 


