# Circuit-Breaking

## Continuing from Configuration

### Open BikeShop

1. Open the solution folder `~/Workspace/BikeShop` in Visual Studio Code.

2. Open a terminal and navigate to the project folder inside your working directory for this session.

    ```bash
    cd ~/Workspace/BikeShop/working/src/BikeShop.API
    ```

    NOTE: You can find the finished version of this lab at ~/Workspace/BikeShop/05-circuit-breaker.

---

### Add packages from Nuget

1. Steeltoe CircuitBreaker Hystrix

    ```bash
    dotnet add package Steeltoe.CircuitBreaker.HystrixCore
    ```

    Or add the following lines to your .csproj file

    ```xml
    <ItemGroup>
      <!-- All the other package references -->
      <PackageReference Include="Steeltoe.CircuitBreaker.HystrixCore" Version="2.3.0" />
      <!-- Added by Initlzr: <PackageReference Include="Newtonsoft.Json" Version="12.0.2" /> -->
    </ItemGroup>
    ```

---

### Create Get All Command

1. Create a directory for the commands called `Commands`.

    In CMD:

    ```cmd
    mkdir src\BikeShop.API\Commands
    ```

    In bash:

    ```bash
    mkdir -p ./src/BikeShop.API/Commands
    ```

2. Create a `GetAllBicyclesCommand.cs` file in the `/src/BikeShop.API/Commands` directory.

3. In the `GetAllBicyclesCommand.cs` file extend the class with the Hystrix command and implement and implement the missing members.

    ```c#
    HystrixCommand<IEnumerable<Bicycle>>
    ```
4. Add in the following private members.

    ```c#
    private readonly BicycleRepository _bicycleRepository;
    private readonly ILogger<GetAllBicyclesCommand> _logger;
    ```
5. Update the constructor to initilize the private members.

    ```c#
    public GetAllBicyclesCommand(IHystrixCommandOptions options, ILogger<GetAllBicyclesCommand> logger, BicycleRepository bicycleRepository) : base(options)
    {
        _bicycleRepository = bicycleRepository;
        _logger = logger;
    }
    ```

6. Create a public method called `GetAllBicycles()` and add `ExecuteAsync()` to it.

    ```c#
    public async Task<IEnumerable<Bicycle>> GetAllBicycles()
    {
        return await ExecuteAsync();
    }
    ```
7. Create a protect override method for `RunAsync()`.

    ```c#
    protected override async Task<IEnumerable<Bicycle>> RunAsync()
    {
    }
    ```

8. Add the following to the `RunAsync()` method.

    ```c#
    _logger.LogInformation("GetAllBicyclesCommand.RunAsync()");
    return await _bicycleRepository.GetAllBicycles();
    ```

    This will log the information that this method has been executed and then return the results.

9. Create a protect override method for `RunFallbackAsync()`.

    ```c#
    protected override async Task<IEnumerable<Bicycle>> RunFallbackAsync()
    {
    }
    ```

10. Add the following to the `RunFallbackAsync()` method.

    ```c#
    _logger.LogInformation("GetAllBicyclesCommand.RunFallbackAsync()");
    return await Task.FromResult<IEnumerable<Bicycle>>(new List<Bicycle>());
    ```

    This will log the information that this method has been executed in fallback and then return empty results.

<details>
<summary>GetAllBicyclesCommand.cs</summary>

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;

namespace BikeShop.API.Commands
{
    public class GetAllBicyclesCommand : HystrixCommand<IEnumerable<Bicycle>>
    {

        private readonly BicycleRepository _bicycleRepository;
        private readonly ILogger<GetAllBicyclesCommand> _logger;

        public GetAllBicyclesCommand(IHystrixCommandOptions options, ILogger<GetAllBicyclesCommand> logger, BicycleRepository bicycleRepository) : base(options)
        {
            _bicycleRepository = bicycleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            return await ExecuteAsync();
        }

        protected override async Task<IEnumerable<Bicycle>> RunAsync()
        {
            _logger.LogInformation("GetAllBicyclesCommand.RunAsync()");
            // await Task.Delay(30000);
            return await _bicycleRepository.GetAllBicycles();
        }

        protected override async Task<IEnumerable<Bicycle>> RunFallbackAsync()
        {
            _logger.LogInformation("GetAllBicyclesCommand.RunFallbackAsync()");
            return await Task.FromResult<IEnumerable<Bicycle>>(new List<Bicycle>());
        }
    }
}
```

</details>

---

### Update the Bicycle Service with a Get All Command

1. In the `BicycleService.cs` file update the using statements with the following:

    ```c#
    using BikeShop.API.Commands;
    ```

2. Add the following private members:

    ```c#
    private readonly GetAllBicyclesCommand _getAllBicyclesCommand;
    ```

3. Inilitize this from the constructor:

    ```c#
    public BicycleService(BicycleRepository bicycleRepository, GetAllBicyclesCommand getAllBicyclesCommand)
    {
        _bicycleRepository = bicycleRepository;
        _getAllBicyclesCommand = getAllBicyclesCommand;
    }
    ```

4. Update the `GetAllBicycles()` method and use the following return statement:

    ```c#
    return await _getAllBicyclesCommand.GetAllBicycles();
    ```
<details>
<summary>BicycleService.cs</summary>

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
using BikeShop.API.Commands;

namespace BikeShop.API.Services
{
    public class BicycleService
    {
        private readonly BicycleRepository _bicycleRepository;
        private readonly GetAllBicyclesCommand _getAllBicyclesCommand;

        public BicycleService(BicycleRepository bicycleRepository, GetAllBicyclesCommand getAllBicyclesCommand)
        {
            _bicycleRepository = bicycleRepository;
            _getAllBicyclesCommand = getAllBicyclesCommand;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            return await _getAllBicyclesCommand.GetAllBicycles();
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

### Update Startup with Get All Command

1. in the `Startup.cs` update the using statments and add the following:

    ```c#
    using Steeltoe.CircuitBreaker.Hystrix;
    using BikeShop.API.Commands;
    ```

2. In the `Startup.cs` file add the following to the `ConfigureServices()` method:

    ```c#
    services.AddHystrixCommand<GetAllBicyclesCommand>("BicycleService", Configuration);
    ```
3. In the `Startup.cs` file add the following to the `Configure()` method:

    ```c#
    app.UseHystrixRequestContext();
    ```
---

### Create Update Command

1. Create an `UpdateBicycleCommand.cs` file in the `/src/BikeShop.API/Commands` directory.

2. In the `UpdateBicycleCommand.cs` file extend the class with the Hystrix command and implement the missing members.

    ```c#
    HystrixCommand<int>
    ```
3. Add in the following private members.

    ```c#
    private readonly BicycleRepository _bicycleRepository;
    private readonly ILogger<UpdateBicycleCommand> _logger;
    ```
4. Update the constructor to initilize the private members.

    ```c#
    public GetAllBicyclesCommand(IHystrixCommandOptions options, ILogger<UpdateBicycleCommand> logger, BicycleRepository bicycleRepository) : base(options)
    {
        _bicycleRepository = bicycleRepository;
        _logger = logger;
    }
    ```

6. Create a public method called `UpdateBicycle()` passing the `bicycle` object. Be sure to add `ExecuteAsync()` to the method.

    ```c#
   public async Task<int> UpdateBicycle(Bicycle bicycle)
        {
            _bicycle = bicycle;
            return await ExecuteAsync();
        }
    ```
7. Create a protected override method for `RunAsync()`.

    ```c#
    protected override async Task<int> RunAsync()
    {
    }
    ```

8. Add the following to the `RunAsync()` method.

    ```c#
    _logger.LogInformation("UpdateBicycleCommand.RunAsync()");
    return await _bicycleRepository.UpdateBicycle(_bicycle));
    ```

    This will log the information that this method has been executed and then return the results.

9. Create a protect override method for `RunFallbackAsync()`.

    ```c#
    protected override async Task<IEnumerable<Bicycle>> RunFallbackAsync()
    {
    }
    ```

10. Add the following to the `RunFallbackAsync()` method.

    ```c#
    _logger.LogInformation("UpdateBicycleCommand.RunFallbackAsync()");
    return await Task.FromResult<int>(0));
    ```

    This will log the information that this method has been executed in fallback and then return empty results.

<details>
<summary>UpdateBicycleCommand.cs</summary>

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;

namespace BikeShop.API.Commands
{
    public class UpdateBicycleCommand : HystrixCommand<int>
    {

        private readonly BicycleRepository _bicycleRepository;
        private readonly ILogger<UpdateBicycleCommand> _logger;
        private Bicycle _bicycle;

        public UpdateBicycleCommand(IHystrixCommandOptions options, ILogger<UpdateBicycleCommand> logger, BicycleRepository bicycleRepository) : base(options)
        {
            _bicycleRepository = bicycleRepository;
            _logger = logger;
        }

        public async Task<int> UpdateBicycle(Bicycle bicycle)
        {
          _bicycle = bicycle;
            return await ExecuteAsync();
        }

        protected override async Task<int> RunAsync()
        {
            _logger.LogInformation("UpdateBicycleCommand.RunAsync()");
            return await _bicycleRepository.UpdateBicycle(_bicycle);
        }

        protected override async Task<int> RunFallbackAsync()
        {
            _logger.LogInformation("UpdateBicycleCommand.RunFallbackAsync()");
            return await Task.FromResult<int>(0);
        }
    }
}
```

</details>

---
### Update the Bicycle Service with an Update Command

1. Add the following private members:

    ```c#
    private readonly UpdateBicycleCommand _updateBicycleCommand;
    ```

2. Inilitize this from the constructor:

    ```c#
    public BicycleService(BicycleRepository bicycleRepository, GetAllBicyclesCommand getAllBicyclesCommand, UpdateBicycleCommand updateBicycleCommand)
    {
        _bicycleRepository = bicycleRepository;
        _getAllBicyclesCommand = getAllBicyclesCommand;
        _updateBicycleCommand = updateBicycleCommand;
    }
    ```

3. Update the `UpdateBicycle(Bicycle bicycle)` method and use the following return statement:

    ```c#
    return await _updateBicycleCommand.UpdateBicycle(bicycle);
    ```
<details>
<summary>BicycleService.cs</summary>

```c#
using System.Collections.Generic;
using System.Threading.Tasks;
using BikeShop.API.Models;
using BikeShop.API.Repositories;
//next line added in lab #5
using BikeShop.API.Commands;

namespace BikeShop.API.Services
{
    public class BicycleService
    {
        private readonly BicycleRepository _bicycleRepository;

        //next 2 lines added in lab #5
        private readonly GetAllBicyclesCommand _getAllBicyclesCommand;
        private readonly UpdateBicycleCommand _updateBicycleCommand;

        public BicycleService(BicycleRepository bicycleRepository, GetAllBicyclesCommand getAllBicyclesCommand, UpdateBicycleCommand updateBicycleCommand)
        {
            _bicycleRepository = bicycleRepository;
            //next 2 lines added in lab #5
            _getAllBicyclesCommand = getAllBicyclesCommand;
            _updateBicycleCommand = updateBicycleCommand;
        }

        public async Task<IEnumerable<Bicycle>> GetAllBicycles()
        {
            //return await _bicycleRepository.GetAllBicycles();
            //next line added in lab #5
            return await _getAllBicyclesCommand.GetAllBicycles();
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
            return await _updateBicycleCommand.UpdateBicycle(bicycle);
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

### Update Startup with Update Command

1. In the `Startup.cs` file add the following to the `ConfigureServices()` method:

    ```c#
    services.AddHystrixCommand<UpdateBicycleCommand>("BicycleService", Configuration);
    ```

---

## Your Turn

You've seen how to build a Hystrix Command to wrap your service requests, both with and without parameters. Now, take what you've learned and create Commands to request an individual bicycle by ID and to delete a bicycle.

## Summary
We modified our basic service requests and wrapped them in Hystrix Commands. This allows for the foundation of effective Circuit Breaking and graceful failure, centering around the `ExecuteAsync()`, `RunAsync()` and `FallbackAsync()` methods.