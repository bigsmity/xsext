# xsext
A command line tool for performing maintenance tasks for xsa via xs cli. This enhances the capability of the xs cli tool by applying it to common development environment scenarios.


## Setup:
* Install the xs cli application as per instructions
* Install .NET Core Runtime 3.1.5 https://dotnet.microsoft.com/download/dotnet-core/3.1
* Add xsext.exe to your system path

## Usage:
* Open a terminal window
* login to xs cli as usual: xs login -a https://hxehost:39030 -u XSA_DEV -p password -o HANAExpress -s development --skip-ssl-validation
* Use the xsext command in the same terminal window to share the login and perform commands: xsext delete-workspace-services

### Eg:

xs login -a https://hxehost:39030 -u XSA_DEV -p password -o HANAExpress -s development --skip-ssl-validation
xsext delete-workspace-services

### Output:

delete-workspace-services
https://hxehost:39030 (API version: 1)
XSA_DEV
HANAExpress
development

1 services found for user XSA_DEV.

XSA_DEV-mh6o91onz1rdnfgf-TestApp1-TestApp1_hdi_db

Deleting service instance "XSA_DEV-mh6o91onz1rdnfgf-TestApp1-TestApp1_hdi_db"...
  delete succeeded
OK



delete-workspace-services complete.



## Commands:

### xsext delete-workspace-services
Will delete all services prepended with the current logged in username.
Params:
whitelist=[ServiceNameSearchTerm1,ServiceNameSearchTerm2]

### xsext delete-all-workspace-services
Will delete all services prepended with any existing username.
Params:
whitelist=[ServiceNameSearchTerm1,ServiceNameSearchTerm2]



