# Configuration

## Continuing from Connectors

### Open BikeShop

1. Open the solution folder `~/Workspace/BikeShop` in Visual Studio Code.

2. Open a terminal and change directories.

```bash
cd ~/Workspace/BikeShop/
```

---

### Update the App Settings file

Add in the following sections of the `appsettings.json` file.

```json
  "spring": {
    "application": {
      "name": "BikeShop"
    },
    "cloud": {
      "config": {
        "uri": "http://localhost:8888",
        "validate_certificates": false
      }
    }
  },
```

<details>
<summary>appsettings.json</summary>

```json
{
  "spring": {
    "application": {
      "name": "BikeShop"
    },
    "cloud": {
      "config": {
        "uri": "http://localhost:8888",
        "validate_certificates": false
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "mysql": {
    "client": {
      "sslmode": "none"
    }
  },
  "multipleMySqlDatabases": false
}
```

</details>

---

### Create the configuration model

In the `Model` directory create a new model called `HeaderMessageConfiguration.cs`.

Add the following properties to that class:

```c#
public bool Visible { get; set; }
public string Title { get; set; }
public string Body { get; set; }
```

<details>
<summary>HeaderMessageConfiguration.cs</summary>

```c#
namespace BikeShop.API.Models
   {
       public class HeaderMessageConfiguration
       {
           public bool Visible { get; set; }
           public string Title { get; set; }
           public string Body { get; set; }
       }
   }
```

</details>

---

### Create the Configuration Server Configuration file

In the base directory create a new file called `configuration-server.json`

```json
{
  "git" : {
    "uri": "https://github.com/robertsirc/Cloud-Native-Configs.git"
  }
}
```

---

### Create the Configuration Server


First, check plan availability (current working directory does not matter):

```bash
cf marketplace -s p-config-server
```

Then, from a terminal set your directory to the `BikeShop.API` folder:

```bash
cd ~/Workspace/BikeShop/src/BikeShop.API/
```

Make sure you are logged in to the PCF instance and then run the following command:

```bash
cf create-service p-config-server standard BikeShopConfigServer -c ./configuration-server.json
```

---

### Update the Manifest file

Add in the following fields:

```yml
applications:
-   buildpack: https://github.com/cloudfoundry/dotnet-core-buildpack
    name: BikeShop
    memory: 256m
    disk_quota: 256m
    timeout: 180
    instances: 1
    random-route: true
    services:
      - BikeShopDB
      - BikeShopConfigServer
    env:
      spring:cloud:config:label: master
      spring:cloud:config:validate_certificates: false
```

Note: the deployment configuration hierarch goes as followed: command line > manifest > previous values > defaults

---

### Update the Nuget Packages

```bash
dotnet add package Steeltoe.Extensions.Configuration.ConfigServerCore
```

### Alternate approach

Copy the following into the .csproj file:

```xml
<PackageReference Include="Steeltoe.Extensions.Configuration.ConfigServerCore" Version="2.3.0" />
```


---

### Update the Startup file

Add the following to the `ConfigureServices` method in `Startup.cs`:

```c#
services.AddOptions();
services.Configure<HeaderMessageConfiguration>(Configuration.GetSection("HeaderMessage"));
```

Update the using declarations:

```c#
//using Steeltoe.Extensions.Configuration.ConfigServer;
using BikeShop.API.Models;
```

Add the following to the `Configure` method in `Startup.cs`:

```c#
app.UseStaticFiles();
```


<details>
<summary>Startup.cs</summary>

```c#
using BikeShop.API.Data;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
using BikeShop.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CloudFoundry.Connector.MySql;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;

namespace BikeShop.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMySqlConnection(Configuration);
            services.AddTransient<BicycleService>();
            services.AddTransient<BicycleRepository>();
            services.AddDbContext<BicycleDbContext>(o => o.UseMySql(Configuration));
            services.AddOptions();
            services.Configure<HeaderMessage>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
            BicycleDbInitialize.init(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
        }
    }
}
```

</details>

### Update the Program file

Add the following to the `CreateWebHostBuilder` method in `Program.cs`:

``` C#
.AddConfigServer();
```
and update the using statements to be:
``` C#
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.ConfigServer;
```

<details>
<summary>Program.cs</summary>

``` C#
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace BikeShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseCloudFoundryHosting()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, configBuilder) =>
                {
                    var env = builderContext.HostingEnvironment;
                    configBuilder.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables()
                        .AddCloudFoundry()
                        .AddConfigServer();
                })
                
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfigServer(context.Configuration.GetSection("Logging"));
                });
    }
}
```
</details>

### Create a Message Model

In the `Model` directory create a new model called `Message.cs`.

Add the following properties to that class:

```c#
public string Title { get; set; }
public string Body { get; set; }
```

<details>
<summary>Message.cs</summary>

```c#
namespace BikeShop.API.Models
   {
       public class Message
       {
           public string Title { get; set; }
           public string Body { get; set; }
       }
   }
```

</details>

---

### Create a Message Controller

In the `Controllers` directory create a new controller called `MessageController.cs`.

Add the following using declarations:

```c#
using BikeShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
```

Give the class the following attributes:

```c#
[ApiController]
[Route("api/[controller]")]
```

Extend the class with the 'ControllerBase' class:

```c#
public class MessageController : ControllerBase
```

Add the following varible and inilized it from the constructor:

```c#
private readonly HeaderMessageConfiguration _headerMessageConfiguration;

public MessageController(IOptions<HeaderMessageConfiguration> headerMessageConfiguration)
{
    _headerMessageConfiguration = headerMessageConfiguration.Value;
}
```

Add a new "GET" method returning the `Message` class:

```c#
[HttpGet]
public Message Get()
{

}
```

In this "GET" method add the following return statement:

```c#
if (_headerMessageConfiguration.Visible)
{
    return new Message
    {
        Title = _headerMessageConfiguration.Title,
        Body = _headerMessageConfiguration.Body
    };
}

return null;
```

<details>
<summary>MessageController.cs</summary>

```c#
using BikeShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BikeShop.API.Controllers
 {
     [ApiController]
     [Route("api/[controller]")]
     public class MessageController : ControllerBase
     {
         private readonly HeaderMessageConfiguration _headerMessageConfiguration;

         public MessageController(IOptions<HeaderMessageConfiguration> headerMessageConfiguration)
         {
             _headerMessageConfiguration = headerMessageConfiguration.Value;
         }

         [HttpGet]
         public Message Get()
         {
             if (_headerMessageConfiguration.Visible)
             {
                 return new Message
                 {
                     Title = _headerMessageConfiguration.Title,
                     Body = _headerMessageConfiguration.Body
                 };
             }

             return null;
         }
     }
 }
```

</details>

---
## Push BikeShop to PCF

1. Change directories:

```bash
cd src/BikeShop.API/
```

2. Publish BikeShop.API

```bash
dotnet publish
```

3. Push BikeShop.API

```bash
cf push
```

---

### Testing BikeShop API

Test the api in a web browser by navigating to the controller (`https://bikeshop-[RANDOM ROUTE].cf.magenic.net/api/Message`), note that now we are using a random route and this can change.

To find this route use the following command:

```bash
cf app BikeShop
```

The results you should get from this end point:

```json
{"title":"Bike Sale this week!","body":"Great bike sale this week on select bikes and accessories."}
```

---

## Recap

So far we have continued from the previous lab. We added the code to pull values from a configuration server and created a controller to serve up the values for this.

## Next Steps
Next, we will go through some of the available Management Endpoints that can be tapped to produce real time data about your apps operation. Change to the Management branch to continue.