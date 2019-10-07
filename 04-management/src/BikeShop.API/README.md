# Management

## Continuing from Configuration

## Adding Heath Endpoint

### Open BikeShop

1. Open the solution folder `~/Workspace/BikeShop` in Visual Studio Code.

2. Open a terminal and change directories.

```bash
cd ~/Workspace/BikeShop/
```

---

### Update Nuget Packages

From a terminal set your directory to the `BikeShop.API` folder:

```bash
cd ~/Workspace/BikeShop/src/BikeShop.API/
```

Then add the following reference(s):

```bash
dotnet add package Steeltoe.Management.CloudFoundryCore
```

---

### Update the Startup file

Update the using declartion in `Startup.cs`:

```c#
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Management.Endpoint.CloudFoundry;
```

Add the following to the `ConfigureServices` method in `Startup.cs`:

```c#
services.AddCloudFoundryActuators(Configuration);
```

Add the following to the `Configure` method in `Startup.cs`:

```c#
app.UseCloudFoundryActuators();
```

---

### Update the Program file

Update the using declartion in `Program.cs`:

```c#
using Steeltoe.Extensions.Logging;
```

In the `ConfigureLogging` method add the following:

```c#
builder.AddDynamicConsole();
builder.AddDebug();
```

<details>
<summary>Program.cs</summary>

```c#
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pivotal.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Extensions.Logging;

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
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder.AddDynamicConsole();
                    builder.AddDebug();
                });
    }
}

```

</details>

---

### Update the App Settings file

Add in the following sections of the `appsettings.json` file:

```json
"management": {
     "endpoints": {
       "path": "/cloudfoundryapplication",
       "cloudfoundry": {
          "validateCertificates": false
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
  "management": {
    "endpoints": {
      "path": "/cloudfoundryapplication",
      "cloudfoundry": {
        "validateCertificates": false
      }
    }
  },
  "AllowedHosts": "*",
  "multipleMySqlDatabases": false,
  "mysql": {
    "client": {
      "sslmode": "none"
    }
  }
}
```

</details>

---

### Update the Bicycle Repository

Add the following methods to the `BicycleRepository.cs` file:

```c#
public async Task<int> UpdateBicycle(Bicycle bicycle)
{
    _bicycleDbContext.Update(bicycle);
    return await _bicycleDbContext.SaveChangesAsync();
}

public async Task<int> DeleteBicycle(long id)
{
    var bicycle = await _bicycleDbContext.FindAsync<Bicycle>(id);
    _bicycleDbContext.Remove(bicycle);
    return await _bicycleDbContext.SaveChangesAsync();
}
```

<details>
<summary>BicycleRepository.cs</summary>

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Data;
using BikeShop.API.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<int> UpdateBicycle(Bicycle bicycle)
        {
            _bicycleDbContext.Update(bicycle);
            return await _bicycleDbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteBicycle(long id)
        {
            var bicycle = await _bicycleDbContext.FindAsync<Bicycle>(id);
            _bicycleDbContext.Remove(bicycle);
            return await _bicycleDbContext.SaveChangesAsync();
        }
    }

}
```

</details>

---

### Update the Bicycle Service

Add the following methods to the `BicycleService.cs` file:

```c#
public async Task<int> UpdateBicycle(Bicycle bicycle)
{
    return await _bicycleRepository.UpdateBicycle(bicycle);
}

public async Task<int> DeleteBicycle(long id)
{
    return await _bicycleRepository.DeleteBicycle(id);
}
```

<details>
<summary>BicycleService.cs</summary>

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

        public async Task<int> UpdateBicycle(Bicycle bicycle)
        {
            return await _bicycleRepository.UpdateBicycle(bicycle);
        }

        public async Task<int> DeleteBicycle(long id)
        {
            return await _bicycleRepository.DeleteBicycle(id);
        }
    }
}
```

</details>

---

### Update the Bicycle Controller

Add the following methods to the `BicycleController.cs` file:

```c#
[HttpPut]
public async Task<int> Update([FromBody] Bicycle bicycle)
{
    return await _bicycleService.UpdateBicycle(bicycle);
}

[HttpDelete]
public async Task<int> Delete(long id)
{
    return await _bicycleService.DeleteBicycle(id);
}
```

<details>
<summary>BicycleController.cs</summary>

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

        [HttpPut]
        public async Task<int> Update([FromBody] Bicycle bicycle)
        {
            return await _bicycleService.UpdateBicycle(bicycle);
        }

        [HttpDelete]
        public async Task<int> Delete(long id)
        {
            return await _bicycleService.DeleteBicycle(id);
        }
    }
}
```

</details>

---

### Create a Health Contributor

In your `BikeShop.API` directory create a folder called `Contributors`.

In the `Contributors` folder create a file called `BicycleContributor.cs`.

Update the using declarations:

```c#
using Steeltoe.Common.HealthChecks;
```

Extended the `BicycleContributor` class with `IHealthContributor`.

It will add the following from the interface:

```c#
public HealthCheckResult Health()
{

}

public string Id { get; }
```

Add the following field:

```c#
private readonly BicycleDbContext _bicycleDbContext;
```

Initialize this field from the constructor:

```c#
public BicycleContributor(BicycleDbContext bicycleDbContext)
{
    _bicycleDbContext = bicycleDbContext;
}
```

Add the following to the `HealthCheckResult` method in `BicycleContributor.cs`:

```c#
var health = new HealthCheckResult();

try
{
    var count = _bicycleDbContext.Bicycles.ToList().Count;

    health.Status = HealthStatus.UP;

    health.Details.Add("count", count);
}
catch (Exception e)
{
    Console.WriteLine(e);

    health.Status = HealthStatus.DOWN;
}

return health;
```

Set the `Id`:

```c#
public string Id { get; } = "BicycleDb";
```

<details>
<summary>BicycleContributor.cs</summary>

```c#
using System;
using System.Linq;
using BikeShop.API.Data;
using Steeltoe.Common.HealthChecks;

namespace BikeShop.API.Contributors
{
    public class BicycleContributor : IHealthContributor
    {
        private readonly BicycleDbContext _bicycleDbContext;

        public BicycleContributor(BicycleDbContext bicycleDbContext)
        {
            _bicycleDbContext = bicycleDbContext;
        }


        public HealthCheckResult Health()
        {
            var health = new HealthCheckResult();

            try
            {
                var count = _bicycleDbContext.Bicycles.ToList().Count;

                health.Status = HealthStatus.UP;
                health.Details.Add("count", count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                health.Status = HealthStatus.DOWN;
            }
            return health;
        }

        public string Id { get; } = "BicycleDb";
    }
}
```

</details>

---

### Add a demo Health Contributor

In the `Contributors` folder create another file called `DemoContributor.cs` and copy the
following code into it.

```c#
using System;
using Steeltoe.Common.HealthChecks;

namespace BikeShop.API.Contributors
{
    public class DemoContributor : IHealthContributor
    {
        public DemoContributor() { }

        public HealthCheckResult Health()
        {
            var health = new HealthCheckResult();
            try
            {
                //toggle status between UP and WARNING every minute
                var inGoodShape = (DateTime.UtcNow.Minute % 2) == 0;
                health.Status = inGoodShape ? HealthStatus.UP : HealthStatus.WARNING;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                health.Status = HealthStatus.DOWN;
            }
            health.Details.Add("status", health.Status.ToString());
            return health;
        }

        public string Id { get; } = "Demo";
    }
}
```


Update the using declaration in `Startup.cs`:

```c#
using Steeltoe.Common.HealthChecks;
using BikeShop.API.Contributors;
```

Add the following to the `ConfigureServices` method in `Startup.cs`:

```c#
services.AddScoped<IHealthContributor, BicycleContributor>();
services.AddSingleton<IHealthContributor, DemoContributor>();
```

---

### Testing so far

Push your app to PCF:

```bash
cf push
```

In [AppsMan](https://apps.cf.magenic.net) select the BikeShop app and go to the "Overview" tab. Under the "Processes and Instances" section you can expand your app and see the results of the health check there.

---

## Adding Info Endpoint

### Create the Tracked Operations enum

In the `Models` folder add a new file called `TrackedOperation.cs`.

Add the following fields to the enum:

```c#
Add,
Get,
All,
Update,
Delete
```

<details>
<summary>TrackedOperation.cs</summary>

```c#
namespace BikeShop.API.Models
{
    public enum TrackedOperation
    {
        Add,
        Get,
        All,
        Update,
        Delete
    }
}
```

</details>

---

### Create the Operation Counter interface

Create a new file in the `Contributors` directory called `IOperationCounter.cs`.

Update the using declartion in `IOperationCounter.cs`:

```c#
using System.Collections.Generic;
using BikeShop.API.Models;
```

Add the follow to this interface:

```c#
void Increment(TrackedOperation operation);

IDictionary<TrackedOperation, int> GetCounts { get; }

string Name { get; }
```

<details>
<summary>IOperationCounter.cs</summary>

```c#
using System.Collections.Generic;
using BikeShop.API.Models;

namespace BikeShop.API.Contributors
{
    public interface IOperationCounter<T>
    {
        void Increment(TrackedOperation operation);

        IDictionary<TrackedOperation, int> GetCounts { get; }

        string Name { get; }
    }
}
```

</details>

---

### Create the Operation Counter Class

Create a new file in the `Contributors` directory called `OperationCounter.cs`.

Update the using declarations:

```c#
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BikeShop.API.Models;
```

Extend `OperationCounter` with the `IOperationCounter` interface. This will look like so:

```c#
public void Increment(TrackedOperation operation)
{
    throw new System.NotImplementedException();
}

public IDictionary<TrackedOperation, int> GetCounts { get; }
public string Name { get; }
```

Set the `Name` property to:

```c#
public string Name => $"{typeof(T).Name}Operations";
```

Add the following field:

```c#
private readonly IDictionary<TrackedOperation, int> _count;
```

Update the `GetCounts` property to:

```c#
public IDictionary<TrackedOperation, int> GetCounts => _count.ToImmutableDictionary();
```

Update the `Increment` method with the following:

```c#
_count[operation] = ++_count[operation];
```

Add a public constructor for `OperationCounter` and add the following to it:

```c#
_count = new Dictionary<TrackedOperation, int>();

foreach (var action in Enum.GetValues(typeof(TrackedOperation)))
{
    _count.Add((TrackedOperation) action, 0);
}
```

<details>
<summary>OperationCounter.cs</summary>

```c#
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BikeShop.API.Models;

namespace BikeShop.API.Contributors
{
    public class OperationCounter<T> : IOperationCounter<T>
    {
        private readonly IDictionary<TrackedOperation, int> _count;

        public OperationCounter()
        {
            _count = new Dictionary<TrackedOperation, int>();

            foreach (var action in Enum.GetValues(typeof(TrackedOperation)))
            {
                _count.Add((TrackedOperation) action, 0);
            }
        }

        public void Increment(TrackedOperation operation)
        {
            _count[operation] = ++_count[operation];
        }

        public IDictionary<TrackedOperation, int> GetCounts => _count.ToImmutableDictionary();
        public string Name => $"{typeof(T).Name}Operations";
    }
}
```

</details>

---

### Add Tracked Operations to the Bicycle Controller

Add the following field:

```c#
private readonly IOperationCounter<Bicycle> _operationCounter;
```

Initialize this field from the constructor:

```c#
public BicycleController(BicycleService bicycleService, IOperationCounter<Bicycle> operationCounter)
{
    _bicycleService = bicycleService;
    _operationCounter = operationCounter;
}
```

Update the `BicycleController` and add the following code to each CRUD operation being mindful of the type of operation.

Example:

```c#
_operationCounter.Increment(TrackedOperation.Get);
```

<details>
<summary>BicycleController.cs</summary>

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Services;
using Microsoft.AspNetCore.Mvc;
using BikeShop.API.Contributors;

namespace BikeShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BicycleController : ControllerBase
    {
        private readonly BicycleService _bicycleService;
        private readonly IOperationCounter<Bicycle> _operationCounter;

        public BicycleController(BicycleService bicycleService, IOperationCounter<Bicycle> operationCounter)
        {
            _bicycleService = bicycleService;
            _operationCounter = operationCounter;
        }

        [HttpGet]
        public async Task<IEnumerable<Bicycle>> Get()
        {
            _operationCounter.Increment(TrackedOperation.All);
            return await _bicycleService.GetAllBicycles();
        }

        [HttpGet("{id}")]
        public async Task<Bicycle> Get(long id)
        {
            _operationCounter.Increment(TrackedOperation.Get);
            return await _bicycleService.GetBicycle(id);
        }

        [HttpPost]
        public async Task<int> Add([FromBody] Bicycle bicycle)
        {
            _operationCounter.Increment(TrackedOperation.Add);
            return await _bicycleService.AddBicycle(bicycle);
        }

        [HttpPut]
        public async Task<int> Update([FromBody] Bicycle bicycle)
        {
            _operationCounter.Increment(TrackedOperation.Update);
            return await _bicycleService.UpdateBicycle(bicycle);
        }

        [HttpDelete]
        public async Task<int> Delete(long id)
        {
            _operationCounter.Increment(TrackedOperation.Delete);
            return await _bicycleService.DeleteBicycle(id);
        }
    }
}
```

</details>

---

### Create a Info Contributor

In the `Contributors` folder create a file called `BicycleInfoContributor.cs`.

Extended the `BicycleInfoContributor` class with `IInfoContributor`.

It will add the following from the interface:

```c#
public void Contribute(IInfoBuilder builder)
{

}
```

Update the using declarations:

```c#
using BikeShop.API.Models;
using Steeltoe.Management.Endpoint.Info;
```

Add the following field:

```c#
private readonly IOperationCounter<Bicycle> _operationCounter;
```

Initialize this field from the constructor:

```c#
public BicycleInfoContributor(IOperationCounter<Bicycle> operationCounter)
{
    _operationCounter = operationCounter;
}
```

In the `Contribute` method add the following code:

```c#
builder.WithInfo(
    _operationCounter.Name,
    _operationCounter.GetCounts
);
```

<details>
<summary>BicycleInfoContributor.cs</summary>

```c#
using BikeShop.API.Models;
using Steeltoe.Management.Endpoint.Info;

namespace BikeShop.API.Contributors
{
    public class BicycleInfoContributor : IInfoContributor
    {
        private readonly IOperationCounter<Bicycle> _operationCounter;

        public BicycleInfoContributor(IOperationCounter<Bicycle> operationCounter)
        {
            _operationCounter = operationCounter;
        }

        public void Contribute(IInfoBuilder builder)
        {
            builder.WithInfo(
                _operationCounter.Name,
                _operationCounter.GetCounts
            );
        }
    }
}
```

</details>

---

### Update the Startup with the Info Contributor

Update the using declartion in `Startup.cs`:

```c#
using Steeltoe.Management.Endpoint.Info;
```

Add the following to the `ConfigureServices` method in `Startup.cs`:

```c#
services.AddSingleton<IInfoContributor, BicycleInfoContributor>();
services.AddSingleton<IOperationCounter<Bicycle>, OperationCounter<Bicycle>>();
```

<details>
<summary>Startup.cs</summary>

```c#
using BikeShop.API.Contributors;
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
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Management.Endpoint.CloudFoundry;
using Steeltoe.Common.HealthChecks;
using Swashbuckle.AspNetCore.Swagger;
using Steeltoe.Management.Endpoint.Info;


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
            services.Configure<HeaderMessageConfiguration>(Configuration.GetSection("HeaderMessage"));
            services.AddCloudFoundryActuators(Configuration);
            services.AddSingleton<IHealthContributor, BicycleContributor>();
            services.AddSingleton<IInfoContributor, BicycleInfoContributor>();
            services.AddSingleton<IOperationCounter<Bicycle>, OperationCounter<Bicycle>>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
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
            app.UseCloudFoundryActuators();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            BicycleDbInitialize.init(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
        }
    }
}
```

</details>

---

### Add and configure Swashbuckle

NOTE: Consider checking for the latest guidance e.g. if you are reading this after the SpringOne session:
 * https://github.com/domaindrivendev/Swashbuckle.AspNetCore.
 * https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio

Add the following Nuget package via the CLI:

```bash
dotnet add package Swashbuckle.AspNetCore -v 5.0.0-rc4
#dotnet add package Swashbuckle.AspNetCore.Swagger
```

Or add the following line to your .csproj file

```xml
  <ItemGroup>
    <!-- All the other package references -->
    <PackageReference Include="Swashbuckle.AspNetCore" Version="5.0.0-rc4" />
  </ItemGroup>
```

Followed by a restore via the CLI from the project directory (04-management/src/BikeShop.API)

```bash
dotnet restore
```

Next, we'll update the `Startup.cs` file.

Add (via using directive) the 'Microsoft.OpenApi.Models' namespace.

```c#
using Microsoft.OpenApi.Models;
```

Then, update the `ConfigureServices` method.

```c#
// add the following after .AddMvc();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BikeShop API", Version = "v1" });
});
```

Then, update the `Configure` method.

```c#
//insert middleware to expose the generated Swagger as JSON endpoint(s)
app.UseSwagger();
//insert the swagger-ui middleware if you want to expose interactive documentation,
//specifying the Swagger JSON endpoint(s) to power it from
//NOTE: This is optional.
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeShop.API v1");
});
```

<details>
<summary>Startup.cs</summary>

```c#
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CloudFoundry.Connector.MySql;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using BikeShop.API.Data;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
using BikeShop.API.Services;

//next 4 lines added in lab #4
using Microsoft.OpenApi.Models;
using Steeltoe.Common.HealthChecks;
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Management.Endpoint.Info;
using BikeShop.API.Contributors;

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
            services.AddMySqlConnection(Configuration);
            services.AddTransient<BicycleService>();
            services.AddTransient<BicycleRepository>();
            services.AddDbContext<BicycleDbContext>(o => o.UseMySql(Configuration, "mysql"));
            services.AddOptions();
            // services.ConfigureConfigServerClientOptions(Configuration);

            // var test = Configuration.GetSection("headerMessage");
            // if(test.Value == null)
            // {
            //     Console.WriteLine("No 'headerMessage' section in configuration!");
            // }
            services.Configure<HeaderMessageConfiguration>(Configuration.GetSection("headerMessage"));

            //next 8 lines added in lab #4
            services.AddCloudFoundryActuators(Configuration);
            services.AddScoped<IHealthContributor, BicycleContributor>();
            services.AddSingleton<IHealthContributor, DemoContributor>();
            services.AddSingleton<IInfoContributor, BicycleInfoContributor>();
            services.AddSingleton<IOperationCounter<Bicycle>, OperationCounter<Bicycle>>();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BikeShop API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider sp)
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
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();

            //next 5 lines added in lab #4
            app.UseCloudFoundryActuators();
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeShop.API v1");
            });

            // BicycleDbInitialize.init(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
            using(var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                BicycleDbInitialize.init(scope.ServiceProvider);
            }
        }
    }
}
```

</details>

---

### Deeper Testing

Push your app to PCF:

```bash
cf push
```

Navigate to swagger in your app via the URL. From there you will be able to add and delete new bikes and then in [AppsMan](https://apps.cf.magenic.net) select the BikeShop app and go to the "Overview" tab. Under the "Processes and Instances" section you can expand your app and see the results of the health check there.

---