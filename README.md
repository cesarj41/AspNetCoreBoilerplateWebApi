# ASPNET Core WebApi Boilerplate

This is a project boilerplate web api for asp.net core, designed using clean arquitecture, it comes with logging, authentication (signed cookie), swagger, exception and error handling configured and implemented:

## Getting Started
Use these instructions to get the project up and running.

### Prerequisites
You will need the following tools:

* [.NET Core SDK 2.2](https://www.microsoft.com/net/download/dotnet-core/2.2)

### Setup
Follow these steps to get your development environment set up:

  1. Clone the repository
  2. Set DbConnection string either on the appsettings.json file or on the os enviroment variables (recommended way).

  2. At the root directory, restore projects:
     ```
     dotnet restore
     ```
  3. Within the `src.Infrastructure` directory, run database migrations:
     ```
     dotnet ef database update
     ```
  4. Within the `src.Web` directory, run:
     ```
	 dotnet run
	 ```
  5. Launch [https://localhost:5001/swagger](https://localhost:5001/swagger) in your browser to view the endpoints.
  

## Technologies
* .NET Core 2.2
* ASP.NET Core 2.2
* Entity Framework Core 2.2

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/cesarj41/AspNetCoreBoilerplateWebApi/blob/master/LICENSE.md) file for details.