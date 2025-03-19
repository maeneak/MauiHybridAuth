# MudBlazor .NET MAUI Blazor Hybrid and Web App Auth with MudBlazor and dotnet new Template.
Fork of the [Microsft MAUI Hybrid with Blazor Identity sample](https://github.com/BethMassi/MauiHybridAuth) converted to MudBlazor with added functionality.

Additions include:
1. MudBlazor theme control
2. MudBlazor extensions and components
3. Profile menu with profile picture/avatar and profile editor
4. Added user roles, profile picture and custom database properties
5. Authenticated Admin user and role management page

## Installing and using the template
1. Clone the repository
2. Make sure you have [.NET 9 installed and the MAUI workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-9.0&tabs=vswin)
3. Open a terminal in the repo root folder and type ```dotnet new install .```
4. Create a new project in the current directory from the terminal using ```dotnet new maui-auth -n {name of project}``` (will create a subdirectory with the name of your project)

Run the web project to test setup. Default admin login: admin@app.email p: Admin@123

The project has been setup to run in Visual Studio but should be easily adapted to vscode

## 'Web' project must be running to run Maui Projects
