# .NET Framework 4.0 WCF Service Application

A production-ready Windows Communication Foundation (WCF) service application built with .NET Framework 4.0.

## Project Structure

```
dotnet-monolith-v3/
├── WcfService.sln                 # Main solution file
├── WcfServiceLibrary/            # WCF service library project
│   ├── ISampleService.cs         # Service contract interface
│   ├── SampleService.cs          # Service implementation
│   ├── Properties/
│   │   └── AssemblyInfo.cs      # Assembly metadata
│   ├── App.config                # Service configuration
│   └── WcfServiceLibrary.csproj # Project file
├── WcfServiceHost/               # Service host console application
│   ├── Program.cs                # Main host program
│   ├── Properties/
│   │   └── AssemblyInfo.cs      # Assembly metadata
│   ├── App.config                # Host configuration
│   └── WcfServiceHost.csproj    # Project file
├── WcfServiceClient/             # Service client test application
│   ├── Program.cs                # Main client program
│   ├── Properties/
│   │   └── AssemblyInfo.cs      # Assembly metadata
│   ├── App.config                # Client configuration
│   └── WcfServiceClient.csproj  # Project file
└── README.md                     # This file
```

## Prerequisites

- **Visual Studio 2010 or later** (for .NET Framework 4.0 support)
- **.NET Framework 4.0** installed on the target machine
- **Windows OS** (WCF is Windows-specific)

## Features

### Service Operations
- **GetGreeting**: Returns a personalized greeting message
- **Calculate**: Performs basic arithmetic operations (add, subtract, multiply, divide)
- **GetServerTime**: Returns the current server time
- **Echo**: Echoes back the input message

### Production-Ready Features
- **Error Handling**: Proper fault contracts and exception handling
- **Configuration Management**: Externalized service configuration
- **Logging Support**: Ready for integration with logging frameworks
- **Security**: Basic HTTP binding with extensibility for security
- **Metadata Exchange**: WSDL generation for service discovery
- **Instance Management**: Per-call instance context mode for scalability

## Building the Solution

### Using Visual Studio
1. Open `WcfService.sln` in Visual Studio
2. Set `WcfServiceHost` as the startup project
3. Build the solution (Ctrl+Shift+B)
4. Ensure all projects build successfully

### Using MSBuild Command Line
```bash
# Build the entire solution
msbuild WcfService.sln /p:Configuration=Release /p:Platform="Any CPU"

# Build individual projects
msbuild WcfServiceLibrary\WcfServiceLibrary.csproj /p:Configuration=Release
msbuild WcfServiceHost\WcfServiceHost.csproj /p:Configuration=Release
msbuild WcfServiceClient\WcfServiceClient.csproj /p:Configuration=Release
```

## Running the Application

### Step 1: Start the Service Host
1. Navigate to `WcfServiceHost\bin\Release\` (or Debug)
2. Run `WcfServiceHost.exe`
3. The service will start and display the endpoint addresses
4. Keep this console window open

### Step 2: Test with the Client
1. Open a new console window
2. Navigate to `WcfServiceClient\bin\Release\` (or Debug)
3. Run `WcfServiceClient.exe`
4. The client will test all service methods

## Service Endpoints

- **Service Endpoint**: `http://localhost:8733/Design_Time_Addresses/WcfServiceLibrary/SampleService/`
- **Metadata Endpoint**: `http://localhost:8733/Design_Time_Addresses/WcfServiceLibrary/SampleService/mex`

## Configuration

### Service Configuration (App.config)
The service configuration includes:
- Basic HTTP binding
- Service metadata enabled
- Service debug information (disabled in production)
- Customizable base addresses

### Client Configuration (App.config)
The client configuration includes:
- Service endpoint address
- Binding configuration
- Contract reference

## Production Deployment

### Windows Service Deployment
1. **Create Windows Service**:
   ```csharp
   // Extend the service host to run as a Windows Service
   public class WcfWindowsService : ServiceBase
   {
       private ServiceHost serviceHost;
       
       protected override void OnStart(string[] args)
       {
           serviceHost = new ServiceHost(typeof(SampleService));
           serviceHost.Open();
       }
       
       protected override void OnStop()
       {
           if (serviceHost != null)
           {
               serviceHost.Close();
           }
       }
   }
   ```

2. **Install as Windows Service**:
   ```bash
   sc create "WcfSampleService" binPath="C:\Path\To\WcfServiceHost.exe"
   sc start "WcfSampleService"
   ```

### IIS Deployment
1. **Create Web Application**:
   - Create a new ASP.NET Web Application
   - Add WCF service references
   - Configure web.config with service endpoints

2. **Configure Application Pool**:
   - Set .NET Framework version to 4.0
   - Configure appropriate identity and permissions

### Configuration Management
1. **Environment-Specific Configs**:
   ```xml
   <!-- Production App.config -->
   <system.serviceModel>
     <services>
       <service name="WcfServiceLibrary.SampleService" 
                behaviorConfiguration="ProductionBehavior">
         <endpoint address="" 
                   binding="basicHttpBinding" 
                   contract="WcfServiceLibrary.ISampleService" />
       </service>
     </services>
     <behaviors>
       <serviceBehaviors>
         <behavior name="ProductionBehavior">
           <serviceMetadata httpGetEnabled="False" />
           <serviceDebug includeExceptionDetailInFaults="False" />
         </behavior>
       </serviceBehaviors>
     </behaviors>
   </system.serviceModel>
   ```

2. **External Configuration**:
   - Use `configSource` attribute for external config files
   - Implement configuration transformation for different environments

## Security Considerations

### Current Implementation
- Basic HTTP binding (unencrypted)
- No authentication or authorization
- Suitable for development and internal networks

### Production Security
1. **Transport Security**:
   ```xml
   <binding name="SecureBinding">
     <security mode="Transport">
       <transport clientCredentialType="Windows" />
     </security>
   </binding>
   ```

2. **Message Security**:
   ```xml
   <binding name="MessageSecurityBinding">
     <security mode="Message">
       <message clientCredentialType="UserName" />
     </security>
   </binding>
   ```

3. **Authentication**:
   - Windows Authentication
   - Custom UserName/Password validation
   - Certificate-based authentication

## Monitoring and Logging

### Performance Counters
```csharp
// Add performance monitoring
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
[PerformanceCounter("WcfSampleService", "Operations", "Total")]
public class SampleService : ISampleService
{
    // Implementation
}
```

### Logging Integration
```csharp
// Example with log4net
private static readonly ILog Log = LogManager.GetLogger(typeof(SampleService));

public string GetGreeting(string name)
{
    Log.Info($"GetGreeting called with name: {name}");
    // Implementation
}
```

## Troubleshooting

### Common Issues
1. **Service Won't Start**:
   - Check if port 8733 is available
   - Verify .NET Framework 4.0 is installed
   - Check Windows Firewall settings

2. **Client Connection Failed**:
   - Ensure service host is running
   - Verify endpoint addresses match
   - Check network connectivity

3. **Build Errors**:
   - Ensure all project references are correct
   - Verify .NET Framework 4.0 targeting
   - Clean and rebuild solution

### Debugging
1. **Enable Service Debug**:
   ```xml
   <serviceDebug includeExceptionDetailInFaults="True" />
   ```

2. **Use WCF Test Client**:
   - Launch `wcftestclient.exe`
   - Add service endpoint
   - Test operations interactively

## Extending the Service

### Adding New Operations
1. **Update Interface**:
   ```csharp
   [OperationContract]
   string NewOperation(string parameter);
   ```

2. **Implement Method**:
   ```csharp
   public string NewOperation(string parameter)
   {
       // Implementation
       return $"Processed: {parameter}";
   }
   ```

3. **Update Client**:
   - Rebuild service library
   - Update client configuration if needed

### Adding New Bindings
1. **Configure New Endpoint**:
   ```xml
   <endpoint address="net.tcp://localhost:8734/SampleService"
             binding="netTcpBinding"
             contract="WcfServiceLibrary.ISampleService" />
   ```

2. **Update Client Configuration**:
   ```xml
   <endpoint address="net.tcp://localhost:8734/SampleService"
             binding="netTcpBinding"
             contract="WcfServiceLibrary.ISampleService"
             name="NetTcpBinding_ISampleService" />
   ```

## Performance Optimization

### Instance Management
- **PerCall**: Default, good for stateless services
- **PerSession**: For stateful operations
- **Single**: Singleton pattern, use with caution

### Concurrency
```csharp
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                 ConcurrencyMode = ConcurrencyMode.Multiple)]
public class SampleService : ISampleService
{
    // Implementation
}
```

### Throttling
```xml
<serviceThrottling maxConcurrentCalls="16"
                   maxConcurrentInstances="10"
                   maxConcurrentSessions="10" />
```

## Testing

### Unit Testing
```csharp
[TestClass]
public class SampleServiceTests
{
    [TestMethod]
    public void GetGreeting_ValidName_ReturnsGreeting()
    {
        // Arrange
        var service = new SampleService();
        
        // Act
        var result = service.GetGreeting("Test");
        
        // Assert
        Assert.AreEqual("Hello, Test! Welcome to the WCF Service.", result);
    }
}
```

### Integration Testing
- Use WCF Test Client
- Create automated test clients
- Test with different bindings and configurations

## License

This project is provided as-is for educational and development purposes.

## Support

For issues and questions:
1. Check the troubleshooting section
2. Review WCF documentation
3. Check .NET Framework 4.0 compatibility
