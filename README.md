# PPE_Tracker - PPE Tracking Tool for MEDITECH Data Repository or Blue Elm's OpenGate

This project aims to provide a starting point for customers to build a materials tracking dashboard out of their MEDITECH system. While the focus was clearly on the current need for PPE tracking, our hope is this project is useful template for developing dashboards and reporting tools against other domains of the MEDITECH system.

## What it does
This project collects Materials Management stock information, including Quantity On Hand (QOH), at a user defined interval. With both current and historic stock data, the dashboard aims to provide an easy overview of your organization's current PPE supplies as well as the historic use rate.
 
## Built with
* [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-3.1?view=aspnetcore-3.1)
* [Server-side Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-3.1)
* [Radzen Blazor Components](https://blazor.radzen.com/)
* [ChartJS](https://www.chartjs.org/)
* [Datatables.net](https://datatables.net/)
* [SQLite](https://www.sqlite.org/)

## Features
* Data Repository integration
* Optional real-time MEDITECH integration via [OpenGate](https://www.blueelm.com/opengate-overview)
* Generic data source implementation allowing for non-MEDITECH sources

## Configuration
PPE Tracker supports both MEDITECH's Data Repository and direct MEDITECH access via Blue Elm's OpenGate, an ADO.NET driver that provides real-time, direct access to MEDITECH MAGIC, C/S, and Expanse databases. OpenGate support was built on top of the OpenGate SQL CLR integration, but direct integration with the OpenGate data driver assemblies would be a trivial task.

The first step is to decide which option you will use. By default, the project is configured to use Data Repository.

### Data Repository Configuration
*ConnectionString*  Enter a valid SQL Server connection string for your MEDITECH DR's LIVE NPR database
*RefreshInterval*   Enter the frequency, in milliseconds, that you wish to retrieve the current stock QOH data

*appsettings.json*
```json
{
"Logging": {
"LogLevel": {
"Default": "Information",
"Microsoft": "Warning",
"Microsoft.Hosting.Lifetime": "Information"
}
},
"AllowedHosts": "*",
"ConnectionString": "Data Source=localhost;Initial Catalog=livedb;Integrated Security=True",
"OpenGateConnectionString": "Platform=CS;Data Source=BEC;HCIS=BEC.LIVEN;Database=MM.BEC",
"RefreshInterval": 600000
}
```

### OpenGate Configuration
*ConnectionString* Enter a valid SQL Server connection string for your MEDITECH DR's LIVE NPR database
*OpenGateConnectionString* Enter a valid OpenGate SQL CLR connection string
*RefreshInterval* Enter the frequency, in milliseconds, that you wish to retrieve the current stock QOH data

*appsettings.json*
```json
{
"Logging": {
"LogLevel": {
"Default": "Information",
"Microsoft": "Warning",
"Microsoft.Hosting.Lifetime": "Information"
}
},
"AllowedHosts": "*",
"ConnectionString": "Data Source=localhost;Initial Catalog=livedb;Integrated Security=True",
"OpenGateConnectionString": "Platform=CS;Data Source=BEC;HCIS=BEC.LIVEN;Database=MM.BEC",
"RefreshInterval": 600000
}
```
#### Disable DR as datasource, enable OpenGate

You'll need to adjust the application's configuration settings in file Startup.cs, installing an OpenGate IDataSource instance in place of the default DataRepository service.

*Startup.cs*
```csharp
public void ConfigureServices(IServiceCollection services)
{
services.AddRazorPages();
services.AddServerSideBlazor();

// Enable below to use DR as data source
/*services.AddScoped<IDataSource, DataRepository>
(sp => new DataRepository(Configuration["ConnectionString"]));
services.AddHostedService<Worker>
(sp => new Worker(sp.GetService<ILogger<Worker>>(), Configuration["ConnectionString"], int.Parse(Configuration["RefreshInterval"]))); */

// Enable below to use OpenGate SQL CLR integration instead of DR.
services.AddScoped<IDataSource, OpenGate>(sp => new OpenGate(Configuration["ConnectionString"],Configuration["OpenGateConnectionString"]));
services.AddHostedService<Worker>(sp => new OpenGateWorker(sp.GetService<ILogger<OpenGateWorker>>(), Configuration["ConnectionString"],  int.Parse(Configuration["RefreshInterval"]), Configuration["OpenGateConnectionString"]));
}
```

## Final Steps
At this point, you'll want to test locally via Visual Studio. You may consider updating the SQL query to include a condition on a single SourceID to improve performance (and avoid issues with multiple-source DR databases). Once satisified that the tool is working, you can follow [Microsoft's directions](https://docs.microsoft.com/en-us/aspnet/core/tutorials/publish-to-iis?view=aspnetcore-3.1&tabs=visual-studio) to publish the application to be hosted by IIS. **While the application can and will run self-hosted, Microsoft recommends all production ASP.NET Core applications be deployed with IIS as a reverse proxy.** Authentication, authorization, SSL certificate management, etc. can all be configured via IIS.

### License
MIT License

Copyright (c) 2020 Blue Elm Company LLC

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
