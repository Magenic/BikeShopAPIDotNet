# Lab #1 - Setup
In this first lab we will create a new Web API project. We will run it locally,
configure VS Code for easy debugging, and the deploy it ("cf push") onto PCF.

## Create a Web API project

### 1. Open the top-level repo directory in VS Code

### 2. Open a terminal in the same directory

### 3. Create your working directory for this session and navigate to it

We're going to use the name 'BikeShop' for this session. If you choose something else you'll 
need to replace 'BikeShop' with whatever you choose going forward.

In CMD (Windows)

```bash
mkdir BikeShop
cd BikeShop
```

In Bash (Linux)

```bash
mkdir -p ./BikeShop
cd ./BikeShop
```

### 4. Create a solution file

```bash
mkdir -p ./BikeShop
dotnet new sln --name BikeShop
```

### 5. Generate a Web API application called "BikeShop.API"

```bash
dotnet new webapi --output src/BikeShop.API --name BikeShop.API
#or even shorter...
dotnet new webapi -o src/BikeShop.API
```

### 6. Add the BikeShop.API project to the BikeShop solution

```bash
dotnet sln BikeShop.sln add src/BikeShop.API/BikeShop.API.csproj
```

## Build, run, and debug it locally

### 7. Buld and run it from the terminal

```bash
dotnet build
dotnet run -p src/BikeShop.API/BikeShop.API.csproj
```

### 8. Build and debug it in VS Code

#### 8.1 Remove the previous build artifacts

```bash
dotnet clean
```

#### 8.2 Add a 'tasks.json' file in the ~/.vscode directory
NOTE: You can skip this step if you cloned the repo.

Right-click on the ~/.vscode directory, choose **New File**, enter **tasks.json**, and paste in the following.

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/BikeShop/src/BikeShop.API/BikeShop.API.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "publish",
            "command": "dotnet",
            "type": "process",
            "args": [
                "publish",
                "${workspaceFolder}/BikeShop/src/BikeShop.API/BikeShop.API.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "watch",
            "command": "dotnet",
            "type": "process",
            "args": [
                "watch",
                "run",
                "${workspaceFolder}/BikeShop/src/BikeShop.API/BikeShop.API.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        }
    ]
}
```

#### 8.3 Add a 'launch.json' file in the ~/.vscode directory
NOTE: You can skip this step if you cloned the repo.

Right-click on the ~/.vscode directory, choose **New File**, enter **launch.json**, and paste in the following.

```json
{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/BikeShop/src/BikeShop.API/bin/Debug/netcoreapp2.2/BikeShop.API.dll",
            "args": [],
            "cwd": "${workspaceFolder}/BikeShop/src/BikeShop.API",
            "stopAtEntry": false,
            // Enable launching a web browser when ASP.NET Core starts. For more information: https://aka.ms/VSCode-CS-LaunchJson-WebBrowser
            // NOTE: The following entry SHOULD work but DOES NOT for a .NET Core project in C# as of 2019-09-15.
            // NOTE: This is a "Preview Feature" from the Feb-2019 release.
            // "serverReadyAction": {
            //     "action": "openExternally",
            //     "pattern": "^\\s*Now listening on:\\s+(https?://\\S+)",
            //     "uriFormat": "%s/api/values"
            // },
            // NOTE: The following DOES work as of 2019-09-15.
            "launchBrowser": {
                "enabled": true,
                "args": "${auto-detect-url}",
                "windows": {
                    "command": "cmd.exe",
                    "args": "/C start ${auto-detect-url}/api/values"
                },
                "osx": {
                    "command": "open"
                },
                "linux": {
                    "command": "xdg-open"
                }
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/Views"
            }
        },
        {
            "name": ".NET Core Attach",
            "type": "coreclr",
            "request": "attach",
            "processId": "${command:pickProcess}"
        }
    ]
}
```

### 8.4 Hit F5 to verify that the projects launches (in Debug mode)

Life is better when debug mode just works.

## Deploy and run on PCF

### 9. Create a manifest file

Right-click on the **~/BikeShop/src/BikeShop.API** directory, choose **New File**, enter **manifest.yml**, and paste in the following.

NOTE: If you are using a shared space, add your initials (or something) to the end of the name so that it is unique.

```yml
---
applications:
-   name: BikeShop-API-[YOUR INITIALS]
    buildpacks:
    - dotnet-core-buildpack
    memory: 128m
    disk_quota: 256m
    random-route: true
    stack: cflinuxfs3
    timeout: 180
```

### 10. Deploy onto PCF (aka 'cf push')

Switch over to the terminal, or open a new one. The working directory should be the **~/BikeShop** (where we left off).

#### 10.1 Check your aim
```bash
cf target
```
You should see somehing like this come back:
```bash
api endpoint:   https://api.run.pivotal.io
api version:    2.141.0
user:           jasonw@magenic.com
org:            jasonw
space:          development
```

#### 10.2 Publish the app (locally)
We will publish into the ~/BikeShop/publish directory.

In CMD (Windows)
```bash
dotnet publish -o publish src/BikeShop.API/BikeShop.API.csproj
```

In Bash (Linux)
```bash
TODO: Confirm syntax (see above)
```

#### 10.3 Deploy the published bits onto PCF
Here we go:
```bash
cf push -f src/BikeShop.API/manifest.yml -p src/BikeShop.API/publish
```

#### 10.4 Wait for your app to deploy and start up

#### 10.5 Test it

Get the route for the app.
```bash
cf app BikeShop-API-[YOUR INITIALS]
```

Note: To see all of the apps in the Space:
```bash
cf apps
```

The route will be listed next to the app.
```bash
name:              BikeShop-API-[YOUR INITIALS]
requested state:   started
routes:            bikeshop-api-[YOUR INITIALS]-sleepy-cassowary.cfapps.io
last uploaded:     Sun 06 Oct 23:21:02 EDT 2019
stack:             cflinuxfs3
buildpacks:        dotnet-core

type:            web
instances:       1/1
memory usage:    128M
start command:   cd ${DEPS_DIR}/0/dotnet_publish && exec ./BikeShop.API --server.urls http://0.0.0.0:${PORT}
     state     since                  cpu    memory      disk        details
#0   running   2019-10-07T03:21:19Z   0.0%   0 of 128M   0 of 256M
```

Open a browser and navigat to the values controller (`https://bikeshop-api-[YOUR INITIALS].cf.magenic.net/api/values`).

You should see the same results as when run locally
```json
["value1", "value2"]
```

## Recap

So far we have:
 - Configured our development environment
 - Created an app and tested it locally
 - Pushed our app to PCF to tested it

## Next Steps

Next, we will use SteelToe Connectors to access a MySQL database.
