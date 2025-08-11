# Production Deployment Guide

This guide provides step-by-step instructions for deploying the WCF service application to production environments.

## Prerequisites

- Windows Server 2008 R2 or later
- .NET Framework 4.0 installed
- IIS 7.0 or later (for IIS deployment)
- Administrative privileges
- Network access to target machines

## Deployment Options

### Option 1: Windows Service Deployment (Recommended for Production)

#### Step 1: Prepare the Service Host
1. **Build the Release Version**:
   ```bash
   msbuild WcfService.sln /p:Configuration=Release /p:Platform="Any CPU"
   ```

2. **Create Windows Service Wrapper**:
   ```csharp
   // Add to WcfServiceHost project
   using System.ServiceProcess;
   
   public class WcfWindowsService : ServiceBase
   {
       private ServiceHost serviceHost;
       
       public WcfWindowsService()
       {
           ServiceName = "WcfSampleService";
           CanStop = true;
           CanPauseAndContinue = false;
       }
       
       protected override void OnStart(string[] args)
       {
           try
           {
               serviceHost = new ServiceHost(typeof(SampleService));
               serviceHost.Open();
               EventLog.WriteEntry(ServiceName, "WCF Service started successfully.");
           }
           catch (Exception ex)
           {
               EventLog.WriteEntry(ServiceName, $"Failed to start WCF Service: {ex.Message}", EventLogEntryType.Error);
               throw;
           }
       }
       
       protected override void OnStop()
       {
           try
           {
               if (serviceHost != null)
               {
                   serviceHost.Close();
                   EventLog.WriteEntry(ServiceName, "WCF Service stopped successfully.");
               }
           }
           catch (Exception ex)
           {
               EventLog.WriteEntry(ServiceName, $"Error stopping WCF Service: {ex.Message}", EventLogEntryType.Error);
           }
       }
   }
   ```

#### Step 2: Install as Windows Service
1. **Copy Files to Target Machine**:
   ```
   C:\Services\WcfSampleService\
   ├── WcfServiceHost.exe
   ├── WcfServiceLibrary.dll
   ├── App.config
   └── Dependencies\
   ```

2. **Install Service Using SC Command**:
   ```bash
   sc create "WcfSampleService" binPath="C:\Services\WcfSampleService\WcfServiceHost.exe" start=auto
   sc description "WcfSampleService" "WCF Sample Service for .NET Framework 4.0"
   ```

3. **Start the Service**:
   ```bash
   sc start "WcfSampleService"
   ```

4. **Verify Service Status**:
   ```bash
   sc query "WcfSampleService"
   ```

#### Step 3: Configure Service Recovery
```bash
sc failure "WcfSampleService" reset=86400 actions=restart/60000/restart/60000/restart/60000
```

### Option 2: IIS Deployment

#### Step 1: Create Web Application
1. **Open IIS Manager**
2. **Create New Application Pool**:
   - Name: `WcfSampleServicePool`
   - .NET Framework Version: `v4.0`
   - Managed Pipeline Mode: `Integrated`

3. **Create New Web Application**:
   - Site: `Default Web Site`
   - Application Name: `WcfSampleService`
   - Physical Path: `C:\inetpub\wwwroot\WcfSampleService`
   - Application Pool: `WcfSampleServicePool`

#### Step 2: Deploy Service Files
1. **Copy Service Files**:
   ```
   C:\inetpub\wwwroot\WcfSampleService\
   ├── Web.config
   ├── WcfServiceLibrary.dll
   ├── Global.asax
   └── Dependencies\
   ```

2. **Create Web.config**:
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <system.web>
       <compilation debug="false" targetFramework="4.0" />
     </system.web>
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
             <serviceThrottling maxConcurrentCalls="16"
                                maxConcurrentInstances="10"
                                maxConcurrentSessions="10" />
           </behavior>
         </serviceBehaviors>
       </behaviors>
     </system.serviceModel>
   </configuration>
   ```

3. **Create Global.asax**:
   ```csharp
   <%@ Application Codebehind="Global.asax.cs" Inherits="WcfSampleService.Global" Language="C#" %>
   ```

#### Step 3: Configure IIS Settings
1. **Set Application Pool Identity**:
   - Identity: `ApplicationPoolIdentity` (or custom service account)
   - Enable 32-bit Applications: `False`

2. **Configure Authentication**:
   - Anonymous Authentication: `Enabled`
   - Windows Authentication: `Disabled` (unless required)

3. **Set Permissions**:
   ```bash
   icacls "C:\inetpub\wwwroot\WcfSampleService" /grant "IIS_IUSRS:(OI)(CI)(RX)"
   icacls "C:\inetpub\wwwroot\WcfSampleService" /grant "NETWORK SERVICE:(OI)(CI)(RX)"
   ```

### Option 3: Self-Hosted Console Application

#### Step 1: Create Production Host
```csharp
class Program
{
    static void Main(string[] args)
    {
        try
        {
            using (ServiceHost host = new ServiceHost(typeof(SampleService)))
            {
                host.Open();
                Console.WriteLine("Service is running. Press Ctrl+C to stop.");
                
                // Wait for shutdown signal
                var waitHandle = new ManualResetEvent(false);
                Console.CancelKeyPress += (s, e) => {
                    e.Cancel = true;
                    waitHandle.Set();
                };
                
                waitHandle.WaitOne();
                host.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Service error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
```

#### Step 2: Deploy and Run
1. **Copy to Target Machine**
2. **Run as Background Process**:
   ```bash
   start /B WcfServiceHost.exe
   ```

## Production Configuration

### Security Configuration

#### 1. Transport Security (HTTPS)
```xml
<system.serviceModel>
  <services>
    <service name="WcfServiceLibrary.SampleService">
      <endpoint address="" 
                binding="basicHttpsBinding" 
                contract="WcfServiceLibrary.ISampleService" />
    </service>
  </services>
  <bindings>
    <basicHttpsBinding>
      <binding>
        <security mode="Transport">
          <transport clientCredentialType="None" />
        </security>
      </binding>
    </basicHttpsBinding>
  </bindings>
</system.serviceModel>
```

#### 2. Windows Authentication
```xml
<basicHttpBinding>
  <binding>
    <security mode="TransportCredentialOnly">
      <transport clientCredentialType="Windows" />
    </security>
  </binding>
</basicHttpBinding>
```

#### 3. Custom Authentication
```xml
<basicHttpBinding>
  <binding>
    <security mode="Message">
      <message clientCredentialType="UserName" />
    </security>
  </binding>
</basicHttpBinding>
```

### Performance Configuration

#### 1. Service Throttling
```xml
<serviceThrottling maxConcurrentCalls="32"
                   maxConcurrentInstances="20"
                   maxConcurrentSessions="20" />
```

#### 2. Instance Management
```csharp
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                 ConcurrencyMode = ConcurrencyMode.Multiple)]
public class SampleService : ISampleService
{
    // Implementation
}
```

#### 3. Binding Configuration
```xml
<basicHttpBinding>
  <binding maxReceivedMessageSize="65536"
           maxBufferSize="65536"
           maxBufferPoolSize="524288"
           receiveTimeout="00:10:00"
           sendTimeout="00:10:00"
           openTimeout="00:01:00"
           closeTimeout="00:01:00">
  </binding>
</basicHttpBinding>
```

## Monitoring and Logging

### 1. Event Log Integration
```csharp
public class SampleService : ISampleService
{
    private static readonly string ServiceName = "WcfSampleService";
    
    public string GetGreeting(string name)
    {
        try
        {
            EventLog.WriteEntry(ServiceName, $"GetGreeting called with name: {name}", EventLogEntryType.Information);
            // Implementation
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry(ServiceName, $"Error in GetGreeting: {ex.Message}", EventLogEntryType.Error);
            throw;
        }
    }
}
```

### 2. Performance Counters
```csharp
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
[PerformanceCounter("WcfSampleService", "Operations", "Total")]
public class SampleService : ISampleService
{
    // Implementation
}
```

### 3. Health Check Endpoint
```csharp
[OperationContract]
public ServiceHealth GetHealth()
{
    return new ServiceHealth
    {
        Status = "Healthy",
        Timestamp = DateTime.UtcNow,
        Version = "1.0.0.0"
    };
}
```

## Backup and Recovery

### 1. Configuration Backup
```bash
# Backup configuration files
robocopy "C:\Services\WcfSampleService" "C:\Backup\WcfSampleService\%date:~-4,4%%date:~-10,2%%date:~-7,2%" /MIR
```

### 2. Service Recovery Script
```batch
@echo off
echo Checking WCF Sample Service...
sc query "WcfSampleService" | find "RUNNING" >nul
if %errorlevel% neq 0 (
    echo Service not running. Attempting to start...
    sc start "WcfSampleService"
    timeout /t 10 /nobreak >nul
    sc query "WcfSampleService" | find "RUNNING" >nul
    if %errorlevel% neq 0 (
        echo Service failed to start. Sending alert...
        echo Service failure detected at %date% %time% >> C:\Logs\ServiceRecovery.log
    )
)
```

## Troubleshooting Production Issues

### 1. Service Won't Start
- Check Event Viewer for errors
- Verify .NET Framework 4.0 installation
- Check file permissions
- Verify port availability

### 2. Performance Issues
- Monitor CPU and memory usage
- Check service throttling settings
- Review binding configurations
- Analyze network latency

### 3. Security Issues
- Verify authentication settings
- Check firewall configurations
- Review SSL certificate validity
- Monitor access logs

## Maintenance Procedures

### 1. Regular Maintenance
- **Weekly**: Check service status and logs
- **Monthly**: Review performance metrics
- **Quarterly**: Update security patches
- **Annually**: Review and update configurations

### 2. Update Procedures
1. Stop the service
2. Backup current configuration
3. Deploy new version
4. Update configuration if needed
5. Start the service
6. Verify functionality
7. Monitor for issues

### 3. Rollback Procedures
1. Stop the service
2. Restore previous version
3. Restore previous configuration
4. Start the service
5. Verify functionality

## Support and Escalation

### 1. First Level Support
- Service restart procedures
- Basic configuration changes
- Log analysis

### 2. Second Level Support
- Performance tuning
- Security configuration
- Advanced troubleshooting

### 3. Third Level Support
- Code-level issues
- Architecture changes
- Vendor support escalation
