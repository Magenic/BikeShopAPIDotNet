# Workshop Prerequisites

## .NET Core

Usually we install the latest version of the .NET Core SDK. However, we should check which versions of .NET Core are supported by the build packs we'll be using. In a corporate install, you may need to contact your PCF administrator for this info.

### 1. Check for the .NET Core versions supported by the latest build-pack
Check for the .NET Core versions supported by the latest build-pack.

<https://buildpacks.cloudfoundry.org/#/buildpacks>

### 2. Download the latest supported version
Again, don't use a version that is too new. In this workshop we'll use 2.2.402 release.

<https://dotnet.microsoft.com/download>

## CF CLI

### 3. Download and install the Cloud Foundry CLI
Install the latest version of the CF CLI.

<https://docs.run.pivotal.io/cf-cli/install-go-cli.html>

NOTE: If you are having trouble installing the CLI on Linux via Bash, here is an alternative method:

### 3.1 Navigate to /etc/apt in the linux filesystem and open up the 'sources.list' file. We will be adding the package repository manually.
Add the follow line to the end of the 'sources.list' file:
```bash
   deb <https://packages.cloudfoundry.org/debian> stable main
```

### 3.2 Open <https://packages.cloudfoundry.org/debian/cli.cloudfoundry.org.key> and save as a .key file.

### 3.3 Navigate in bash to the location of the .key file added in the previous step. Run the following command:
```bash
sudo apt-key add cli.cloudfoundry.org.key
```

### 3.4 Update your packages in Windows Bash:
```bash
sudo apt-get update
```

### 3.5 Finally, run:
```bash
sudo apt-get install cf-cli
```

### 3.6 Verify that you have an up-to-date version e.g. 6.46.xxx
```shell
cf --version
```

## An Editor (Visual Studio Code)
We'll use VS Code in this workshop.

NOTE: You can use another editor, but will likely need to change a few things in each lab e.g. to get debugging to work.

### 4. Download and install Visual Studio Code
Install the latest version of Visual Studio Code.

NOTE: The per-user install is now recommended.

<https://code.visualstudio.com/>

The following VS Code plug-ins are used in this workshop. You may be prompted for them if they are not installed. Versions below as of 2019-09-14.

#### 4.1 Get the latest C# plug-in
Available via search in the extensions manager in VS Code. 

C# - Microsoft (v1.21.2) - Necessary for full (local) run & debug support

## Database

### 5. Download and install MySQL
NOTE: If you're running Docker, there is a Docker image provided by the SteelToe team that may do the trick.

<https://dev.mysql.com/downloads/>

You don't need to install this locally unless you want a fully-functional local dev experience.
The Docker image mentioned above has not (yet) been tested with these labs.

## Logging in to PCF

### 6. Test your account
You can pass your credentials as parameters if you wish.
```bash
cf login -a https://api.run.pivotal.io
```

You should be prompted for your email and password.

## Next Steps

Next, we will start in on the labs.