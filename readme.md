# .NET MAUI Blazor Hybrid and Web App Auth Sample
This sample demonstrates how to build .NET MAUI Blazor Hybrid and Web Apps that share the same UI *with authentication*. It uses ASP.NET Core Identity local accounts but you can use this pattern for any authentication provider.


## Running the sample
1. Clone the repository.
1. Make sure you have .NET 8 installed and the MAUI workload.
1. Open the solution in Visual Studio 2022.
1. Set the `MauiHybridAuth` MAUI project as the startup project.
1. Start the `MauiHybridAuth.Web` project without debugging.
1. Navigate to `https://localhost:7157/swagger` in your browser to pull up the identity endpoints.
1. Register a user using the `/Register` endpoint.
1. Start (F5) the `MauiHybridAuth` MAUI project. You can set the debug target to Windows, or an Android device or emulator.
1. Notice you can only see the Home and Login pages.
1. Log in with the user you registered.
1. Notice you can now see the Counter and Weather pages.
1. Log out and notice you can only see the Home and Login pages again.
1. Navigate the web app to `https://localhost:7157/` and the app will behave the same.

## Tour of the important parts
### Shared UI 
The shared UI is in the `MauiHybridAuth.Shared` project. This project contains the Razor components that are shared between the MAUI and Web projects. The `Routes.razor` uses the `AuthorizeRouteView` to route users appropriately based on their authentication status. If a user is not authenticated, they are redirected to the `Login` page. 

```code
<AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(Layout.MainLayout)">
    <Authorizing>
        Authorizing...
    </Authorizing>
    <NotAuthorized>
        <Login />
    </NotAuthorized>
</AuthorizeRouteView>  
```

The `NavManu.razor` component contains the navigation menu that uses `AuthorizationView` to show/hide links based on the user's authentication status.

```code
<AuthorizeView>
    <NotAuthorized>
       <!-- Navlinks that display when not logged in -->    
    </NotAuthorized>
    <Authorized>
        <!-- Navlinks that display when logged in -->    
    </Authorized>               
</AuthorizeView>
```				

The `Counter.razor` and `Weather.razor` pages are protected by the `[Authorize]` attribute so you cannot navigate to them unless you are logged in. 

The `Login.razor` page is where the user logs in. The `Login` page injects the `ICustomAuthenticationStateProvider` and uses the `AuthenticationStateProvider` to authenticate the user and redirect them to the home page if successful. When the state changes, the `AuthorizeView` reacts and will show the appropriate pages/links based on the user's authentication status.

```code
//Called on valid submit
private async Task LoginUser()
{
    await AuthStateProvider.LogInAsync(LoginModel);

    if (AuthStateProvider.LoginStatus != LoginStatus.Success)
    {
        //Show error message
        loginFailureHidden = false;
        return;
    }        
        
    Navigation.NavigateTo(""); //Root URL
}
```

### ICustomAuthenticationStateProvider
The `ICustomAuthenticationStateProvider` interface is implemented by the `CustomAuthenticationStateProvider` class in the `MauiHybridAuth.Shared` project. This class is responsible for managing the user's authentication state and providing the `AuthenticationState` to the app. The `CustomAuthenticationStateProvider` class uses the `HttpClient` to make requests to the server to authenticate the user. See the official documentation for more information on [ASP.NET Core Blazor Hybrid authentication and authorization](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/security/?view=aspnetcore-8.0&pivots=maui).

```code
public interface ICustomAuthenticationStateProvider 
{
    public LoginStatus LoginStatus { get; set; }
    Task<AuthenticationState> GetAuthenticationStateAsync();
    Task LogInAsync(LoginModel loginModel);
    void Logout();
}
```

The derived class `MauiAuthenticationStateProvider` is used in the MAUI project and handles calling localhost via the emulators and simulators for easy testing. See the [official documentation](https://learn.microsoft.com/dotnet/maui/data-cloud/local-web-services) for information on what you need to do to be able to call local services from emulators and simulators.
This class could also use the `SecureStorage` API to store the user's token securely on the device, or handle any other platform specific functionality if needed.

### MAUI MauiProgram.cs
The MAUI project's `MauiProgram.cs` file is where the `MauiAuthenticationStateProvider` is registered with the DI container. It also needs to register the Authorization core components where things like `AuthorizeView` are defined.

```code
 // This is the core functionality
builder.Services.AddAuthorizationCore();
// This is our custom provider
builder.Services.AddScoped<ICustomAuthenticationStateProvider, MauiAuthenticationStateProvider>();
// Use our custom provider when the app needs an AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider>(s 
    => (MauiAuthenticationStateProvider)s.GetRequiredService<ICustomAuthenticationStateProvider>());
```
### Web Program.cs 
The Web project's `Program.cs` file is where the Identity endpoints are set up and the `BlazorAuthenticationStateProvider` (derived from `CustomAuthenticationStateProvider`) is registered with the DI container. This sample uses an in-memory database for simplicity but you can easily swap this out for a real database. See the [official documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0) for more information on setting up ASP.NET Core Identity endpoints.

```code
// This is our custom provider
builder.Services.AddScoped<ICustomAuthenticationStateProvider, BlazorAuthenticationStateProvider>();
// Use our custom provider when the app needs an AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider>(s
    => (BlazorAuthenticationStateProvider)s.GetRequiredService<ICustomAuthenticationStateProvider>());
```

## A note on setting up a blank solution 
You can use .NET 9 Preview 5 or later to quickly create the structure of the solution demonstrated here. The new .NET MAUI Blazor Hybrid and Web App solution template is available in .NET 9 Preview 5 or later. This template creates a solution with three projects: a .NET MAUI Blazor Hybrid App, a Blazor Web App, and a shared Razor Class Library that contains all of the UI.

See the [official documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/tutorials/maui-blazor-web-app?view=aspnetcore-9.0) for more information on this template.

