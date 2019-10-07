# Lab #2 - Connectors
In this second lab we will hook our app up to a MySQL instance and serve some
data-driven content. We will get this running locally, and then we will do the
same in PCF.

## Continuing from Setup
If you haven't completed the steps from the first lab you can catch up by
copying the src directory (found at ~/01-setup/src) on top of your working
src directory (found at ~/BikeShop/src).

NOTE: Do NOT copy the ~/01-setup/.vscode directory in the same fashion.
It is configured for opening the 01-Setup directory from VS Code via open 
directory and the paths will be incorrect if copied into your working
directory.

### 1. Open the top-level repo directory in VS Code
If you're continuing from lab #1, it's already open.

### 2. Open a terminal in the same directory
NOTE: If you're continuing from lab #1, you're likely already here.

In CMD (Windows)
```bash
cd BikeShop
```

In Bash (Linux)
```bash
cd ./BikeShop
```

## Add project references

### 3. Add NuGet Packages
Add NuGet package references for:
  - Entity Framework
  - Pomelo (MySql for EF Core)
  - Steeltoe CloudFoundry Connector
  - Steeltoe CloudFoundry Connector for MySql

Via the terminal:
```bash
dotnet add src/BikeShop.API package Microsoft.EntityFrameworkCore
dotnet add src/BikeShop.API package Pomelo.EntityFrameworkCore.MySql
dotnet add src/BikeShop.API package Steeltoe.CloudFoundry.Connector
dotnet add src/BikeShop.API package Steeltoe.CloudFoundry.Connector.MySql
dotnet restore
```

Or add these references "manually." Copy and paste the 4 refeneces below
directly into the project file (found at ~/BikeShop/src/BikeShop.API/BikeShop.API.csproj).
```xml
<ItemGroup>
    <!-- Existing PackageReference elements here -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="2.2.6" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="2.2.0" />
    <PackageReference Include="Steeltoe.CloudFoundry.Connector" Version="1.1.0" />
    <PackageReference Include="Steeltoe.CloudFoundry.Connector.MySql" Version="1.1.0" />
</ItemGroup>
```

## Add a model

### 4. Create a `Models` directory
In CMD (Windows)
```cmd
mkdir src\BikeShop.API\Models
```

In Bash (Linux)
```bash
mkdir -p ./src/BikeShop.API/Models
```

### 5. Add a model class
Create a `Bicycle.cs` file in the new `Models` directory. Add the
following contents to the `Bicycle.cs` file.

```c#
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeShop.API.Models
{
    [Table("bicycle")]
    public class Bicycle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        
        public string ProductName { get; set; }
        
        public double Price { get; set; }
        
        public string Description{ get; set; }
        
        public string Image { get; set; }
    }
}
```

## Add the EF Core database context

### 6. Create a `Data` directory
In CMD

```cmd
mkdir src\BikeShop.API\Data
```

In Bash

```bash
mkdir -p ./src/BikeShop.API/Data
```

### 7. Add the EF Core database context class
Create a `BicycleDbContext.cs` file in the new `Data` directory. Add the
following contents to the `BicycleDbContext.cs` file.

```c#
using Microsoft.EntityFrameworkCore;
using BikeShop.API.Models;

namespace BikeShop.API.Data
{
    public class BicycleDbContext : DbContext
    {
        public DbSet<Bicycle> Bicycles { get; set; }

        public BicycleDbContext(DbContextOptions<BicycleDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);
    
          modelBuilder.Entity<Bicycle>(entity =>
          {
              entity.HasKey(e => e.Id);
              entity.Property(e => e.ProductName);
              entity.Property(e => e.Description);
              entity.Property(e => e.Price);
              entity.Property(e => e.Image);
          });
        }
    }
}
```

## Add a repository

### 8. Create a `Repositories` directory
In CMD
```cmd
mkdir src\BikeShop.API\Repositories
```

In Bash
```bash
mkdir -p ./src/BikeShop.API/Repositories
```

### 9. Add a repository class
Create a `BicycleRepository.cs` file in the new `Repositories` directory. Add the
following contents to the `BicycleRepository.cs` file.

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BikeShop.API.Data;
using BikeShop.API.Models;

namespace BikeShop.API.Repositories
{
    public class BicycleRepository 
    { 
        private readonly BicycleDbContext _bicycleDbContext;

        public BicycleRepository(BicycleDbContext bicycleDbContext)
        {
            _bicycleDbContext = bicycleDbContext;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            return await _bicycleDbContext.Bicycles.ToListAsync();
        }

        public async Task<Bicycle> GetBicycle(long id)
        {
            return await _bicycleDbContext.FindAsync<Bicycle>(id);
        }

        public async Task<int> AddBicycle(Bicycle bicycle)
        {
            
            _bicycleDbContext.Add(bicycle);
            return await _bicycleDbContext.SaveChangesAsync();
        }
    }

}
```

## Add a service

### 10. Create a `Services` directory
In CMD
```cmd
mkdir src\BikeShop.API\Services
```

In Bash
```bash
mkdir -p ./src/BikeShop.API/Services
```

### 11. Add the Bicycle service class
Create a `BicycleService.cs` file in the new `Services` directory. Add the
following contents to the `BicycleService.cs` file.

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;

namespace BikeShop.API.Services
{
    public class BicycleService
    {
        private readonly BicycleRepository _bicycleRepository;

        public BicycleService(BicycleRepository bicycleRepository)
        {
            _bicycleRepository = bicycleRepository;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            return await _bicycleRepository.GetAllBicycles();
        }

        public async Task<Bicycle> GetBicycle(long id)
        {
            return await _bicycleRepository.GetBicycle(id);
        }

        public async Task<int> AddBicycle(Bicycle bicycle)
        {
            return await _bicycleRepository.AddBicycle(bicycle);
        }
    }
}
```

## Add a Controller

### 12. Add the BicycleController class

Create a `BicycleController.cs` file in the (existing) `/src/BikeShop.API/Controllers`
directory.  Add the following contents to the `BicycleController.cs` file.

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BicycleController : ControllerBase
    {
        private readonly BicycleService _bicycleService;

        public BicycleController(BicycleService bicycleService)
        {
            _bicycleService = bicycleService;
        }

        [HttpGet]
        public async Task<IEnumerable<Bicycle>> Get()
        {
            return await _bicycleService.GetAllBicycles();
        }

        [HttpGet("{id}")]
        public async Task<Bicycle> Get(long id)
        {
            return await _bicycleService.GetBicycle(id);
        }

        [HttpPost]
        public async Task<int> Add([FromBody] Bicycle bicycle)
        {
            return await _bicycleService.AddBicycle(bicycle);
        }
    }
}
```

## Add a database initializer

### 13. Add the BicycleDbInitialize class
Create a `BicycleDbInitialize.cs` file in the (existing) `/src/BikeShop.API/Data`
directory.  Add the following contents to the `BicycleDbInitialize.cs` file.

```c#
using System;
using System.Linq;
using BikeShop.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BikeShop.API.Data
{
    public static class BicycleDbInitialize
    {
        public static void init(IServiceProvider serviceProvider)
        {
            using (var context = new BicycleDbContext(serviceProvider.GetRequiredService<DbContextOptions<BicycleDbContext>>()))
            {
                context.Database.EnsureCreated();
                
                if (context.Bicycles.Any()) return;
                context.Bicycles.Add(new Bicycle
                {
                    Description = "Schwin",
                    Image = "img",
                    Price = 899.99,
                    ProductName = "Schwin Mountian Bike"
                });
                context.Bicycles.Add(new Bicycle
                {
                    Description = "Nishiki",
                    Image = "img",
                    Price = 399.99,
                    ProductName = "Nishiki Dirt Bike"
                });
                context.SaveChanges();
            }
        }
    }
}
```

## Update the `Startup` class
Make the following changes in the `~/BikeShop/src/BikeShop-API/Startup.cs` file.

### 14. Add using directives
```c#
using BikeShop.API.Data;
using BikeShop.API.Repositories;
using BikeShop.API.Services;
using Steeltoe.CloudFoundry.Connector.MySql;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
```


### 15. Update `ConfigureServices`
In the `ConfigureServices` method add the following:

```c#
services.AddMySqlConnection(Configuration);
services.AddTransient<BicycleService>();
services.AddTransient<BicycleRepository>();
services.AddDbContext<BicycleDbContext>(o => o.UseMySql(Configuration));          //local
//services.AddDbContext<BicycleDbContext>(o => o.UseMySql(Configuration, "mysql")); //deployed
```

### 16. Update `Configure`
In the `Configure` method add the following:

```c#
BicycleDbInitialize.init(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
```

### Expand below for the full `Startup.cs` file
<details>

<summary>Startup.cs</summary>

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// next 5 lines added in lab #2
using Steeltoe.CloudFoundry.Connector.MySql;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using BikeShop.API.Services;
using BikeShop.API.Repositories;
using BikeShop.API.Data;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // next 4 lines added in lab #2
            services.AddMySqlConnection(Configuration);
            services.AddTransient<BicycleService>();
            services.AddTransient<BicycleRepository>();
            services.AddDbContext<BicycleDbContext>(o => o.UseMySql(Configuration));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            // next line added in lab #2
            BicycleDbInitialize.init(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
        }
    }
}
```

</details>

---

## Update the `Program` class

### 17. Add using directives
```c#
using Steeltoe.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.CloudFoundry;
```

### 18. Update the `CreateWebHostBuilder` method to add the following:
```c#
.UseKestrel()
.UseCloudFoundryHosting()
.UseContentRoot(Directory.GetCurrentDirectory())
.UseIISIntegration() // unnecessary is not using IIS
.UseStartup<Startup>()
.ConfigureAppConfiguration((builderContext, configBuilder) =>
{
    var env = builderContext.HostingEnvironment;
    configBuilder.SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables()
        .AddCloudFoundry();
})
.ConfigureLogging((context, builder) =>
{
    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
});
```

### Expand below for the full `Program.cs` file

<details>

<summary>Program.cs</summary>

```c#
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.CloudFoundry;

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
                        .AddCloudFoundry();
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
    }
}

```

</details>

---

## Update the app settings files

### 19. Add the following to the `appsettings.json` file
```json
"mysql": {
    "client": {
      "sslmode": "none"
    }
  },
  "multipleMySqlDatabases": false
```

### Expand below for the full `appsettings.json` file

<details>

<summary>appsettings.json</summary>

```json
{
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

### 20. Add the following to the `appsettings.Development.json` file
Connects to the local instance of MySQL e.g. when debugging.
```json
"mysql": {
    "client": {
        "sslmode": "none",
        "ConnectionString": "Server=localhost;Database=BikeShop;Uid=root;Pwd=;sslmode=none;"
    }
}
```

### Expand below for the full `appsettings.Development.json` file

<details>

<summary>appsettings.Development.json</summary>

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "mysql": {
    "client": {
      "sslmode": "none",
      "ConnectionString": "Server=localhost;Database=newdb;Uid=root;Pwd=Abc123!;sslmode=none;"
    }
  }
}
```

</details>

---

## Run it locally

### 20. Update the `launch.json` file to run new controller
Edit the following section of the `~/BikeShop/.vscode/launch.json` file.
```json
"windows": {
    "command": "cmd.exe",
    "args": "/C start ${auto-detect-url}/api/values"
},
```

Change only the `args` parameter as follows
```json
"windows": {
    "command": "cmd.exe",
    "args": "/C start ${auto-detect-url}/api/bicycle"
},
```

### 21. Hit F5 to debug it locally
You should see the entries created by the database initializer.


## Provision a database on PCF

### 22. Verify you are logged in
```bash
cf target
```

This should return in something like:
```bash
api endpoint:   https://api.run.pivotal.io
api version:    2.141.0
user:           jasonw@magenic.com
org:            jasonw
space:          development
```

### 23. Review available services
```bash
cf marketplace
```

This should return in something like:
 ```bash
service                       plans                       description
app-autoscaler                standard                    Scales bound applications in response to load
p-circuit-breaker-dashboard   standard                    Circuit Breaker Dashboard for Spring Cloud Applications
p-config-server               standard                    Config Server for Spring Cloud Applications
p-rabbitmq                    standard                    RabbitMQ service to provide shared instances of this high-performance multi-protocol messaging broker.
p-redis                       dedicated-vm, shared-vm     Redis service to provide pre-provisioned instances configured as a datastore, running on a shared or dedicated VM.
p-service-registry            standard                    Service Registry for Spring Cloud Applications
p.mysql                       db-micro                    Dedicated instances of MySQL
p.rabbitmq                    single-node-3.7             RabbitMQ service to provide dedicated instances of this high-performance multi-protocol messaging broker
p.redis                       cache-small, cache-medium   Redis service to provide on-demand dedicated instances configured as a cache.
 ```

### 24. Create a MySQL service instance
NOTE: This may take a few minutes to provision.
 ```bash
cf create-service p.mysql db-micro BikeShopDB-[YOUR INITIALS]
 ```

 Confirm that the service has been created:
 ```bash
 cf services
 ```

Eventually you will see the `create succeeded` message under `last operatiom`
```bash
name                         service   plan       bound apps   last operation
BikeShopDB-[YOUR INITIALS]   p.mysql   db-micro                create succeeded
```

## Bind the new MySQL service instance to the app

### 25. Add the following lines to the bottom of the `manifest.yml` file:
```yml
  services:
   - mysql-[YOUR INITIALS]
```

### Expand below for the full `manifest.yml` file

<details>

<summary>manifest.yml</summary>

```yml
---
applications:
-   name: BikeShop-API-[YOUR INITIALS]
    buildpacks:
    - dotnet_core_buildpack
    memory: 128m
    disk_quota: 256m
    random-route: true
    stack: cflinuxfs3
    timeout: 180
    services:
    - mysql-[YOUR INITIALS]
```

</details>

### You can also create service instance bindings from the command line
This is not needed if you made the change above to the manifest.
NOTE: Wait until the service instance is successfully created or the following command will fail.

```bash
cf bind-service BikeShop-API-[YOUR USER NAME] BikeShopDB
```

## Publish and test the app

### 26. Deploy the app onto PCF
```bash
cf push -f src/BikeShop.API/manifest.yml
```

NOTE: Wait for your app to deploy and start up

### 27. Test it

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

Open a browser and navigat to the values controller (`https://bikeshop-api-[YOUR INITIALS].cf.magenic.net/api/bicycle`).

You should see the same results as when run locally
```json
[{"id":2,"productName":"Schwin Mountian Bike","price":899.99,"description":"Schwin","image":"img"},{"id":11,"productName":"Wonder Bike 2019","price":0.0,"description":"It's amazing!","image":""}]
```

## Recap

In this lab we:
 - Added the data-driven Bicycle controller and supporting classes
 - Provisioned a MySQL service instance in PCF
 - Bound the service instance to our app
 - Updated our app and tested it locally
 - Pushed our app to PCF to tested it

## Next Steps

Next, we will use SteelToe to easily externalize configuration.
