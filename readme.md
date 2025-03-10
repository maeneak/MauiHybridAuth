# .NET MAUI Blazor Hybrid and Web App Auth Sample
This sample demonstrates how to build .NET MAUI Blazor Hybrid and Web Apps that shares common UI and also provides *authentication*. It uses ASP.NET Core Identity local accounts but you can use this pattern for any authentication provider you need to call from a MAUI Blazor Hybrid client. This sample demonstrates the following:
- Setting up the UI to show/hide pages if a user is/is not authenticated
- Setting up the ASP.NET Identity endpoints so they can be called by remote clients
- Logging in, Logging out, and refreshing tokens from the MAUI client
- Saving and retrieving tokens in secure device storage
- Calling a secure endpoint (/api/weather) from the client

## Running the sample
1. Clone the repository.
1. Make sure you have [.NET 9 installed and the MAUI workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-9.0&tabs=vswin).
1. Open the solution in Visual Studio 2022 or VS Code with the .NET MAUI extension installed.
1. Set the `MauiHybridAuth` MAUI project as the startup project.
1. Start the `MauiHybridAuth.Web` project without debugging (in Visual Studio right-click on the project and select "Debug -> Start without Debugging").
1. Register a user in the Blazor Web app UI or navigate to `https://localhost:7157/swagger` in your browser to pull up the identity endpoints and register a user using the `/identity/register` endpoint.
1. Start (F5) the `MauiHybridAuth` MAUI project. You can set the debug target to Windows or an Android emulator.
1. Notice you can only see the Home and Login pages.
1. Log in with the user you registered.
1. Notice you can now see the shared Counter and Weather pages.
1. Log out and notice you can only see the Home and Login pages again.
1. Navigate the web app to `https://localhost:7157/` and the app will behave the same.

## Tour of the important parts
### Shared UI
The shared UI is in the `MauiHybridAuth.Shared` project. This project contains the Razor components that are shared between the MAUI and Blazor Web projects (Home, Counter and Weather pages). The `Counter.razor` and `Weather.razor` pages are protected by the `[Authorize]` attribute so you cannot navigate to them unless you are logged in.

```code
@page "/counter"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize]
```
### Routing in the MAUI & Blazor apps
The `Routes.razor` uses the `AuthorizeRouteView` to route users appropriately based on their authentication status. If a user is not authenticated, they are redirected to the `Login` page.

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

The `NavManu.razor` components contain the navigation menu that uses `AuthorizationView` to show/hide links based on the user's authentication status.

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
### Setting up the server
The Blazor Web app contains all the pages and uses the `SignInManager` framework class to manage logins and users. All of this is generated automatically when you create a Blazor Web project and select to use Authentication with Individual accounts. In order for the MAUI client (or any external client) to authenticate, the ASP.NET Identity endpoints need to be exposed. In the `Program.cs` file this is set up with the call to `AddIdentityEnpoints` and `MapIdentityApi`. NOTE: You'll need to remove the generated call to `.AddIdentityCookies` on `.AddAuthentication`. It is not necessary when calling `.AddIdentityEndpoints` and will result in an error.

```code
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the MauiHybridAuth.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Add Auth services used by the Web app
builder.Services.AddAuthentication(options =>
{
    // Ensure that unauthenticated clients redirect to the login page rather than receive a 401 by default.
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Needed for external clients to log in
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Apply migrations & create database if needed at startup
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MauiHybridAuth.Shared._Imports).Assembly);

// Needed for external clients to log in
app.MapGroup("/identity").MapIdentityApi<ApplicationUser>();
// Needed for Identity Blazor components
app.MapAdditionalIdentityEndpoints();

//Add the weather API endpoint and require authorization
app.MapGet("/api/weather", async (IWeatherService weatherService) =>
{
    var forecasts = await weatherService.GetWeatherForecastsAsync();
    return Results.Ok(forecasts);
}).RequireAuthorization();

app.Run();
```
### Logging in from the MAUI client
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
NOTE: This sample only implements Login and Logout pages on the MAUI client but you could build Register and other management pages against the exposed endpoints for more functionality. For more information on identity endpoints see [How to use Identity to secure a Web API backend for SPAs](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization)

### MauiAuthenticationStateProvider
The `MauiAuthenticationStateProvider` class is in the `MauiHybridAuth` MAUI project. This class is responsible for managing the user's authentication state and providing the `AuthenticationState` to the app. The `MauiAuthenticationStateProvider` class uses an `HttpClient` to make requests to the server to authenticate the user. See the official documentation for more information on [ASP.NET Core Blazor Hybrid authentication and authorization](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/security/?view=aspnetcore-8.0&pivots=maui).

```code
 private async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginRequest loginModel)
 {
     var authenticatedUser = _defaultUser;
     LoginStatus = LoginStatus.None;

     try
     {
         //Call the Login endpoint and pass the email and password
         var httpClient = HttpClientHelper.GetHttpClient();
         var loginData = new { loginModel.Email, loginModel.Password };
         var response = await httpClient.PostAsJsonAsync(HttpClientHelper.LoginUrl, loginData);

         LoginStatus = response.IsSuccessStatusCode ? LoginStatus.Success : LoginStatus.Failed;

         if (LoginStatus == LoginStatus.Success)
         {
             // Save token to secure storage so the user doesn't have to login every time
             var token = await response.Content.ReadAsStringAsync();
             _accessToken = await TokenStorage.SaveTokenToSecureStorageAsync(token, loginModel.Email);

             authenticatedUser = CreateAuthenticatedUser(loginModel.Email);
             LoginStatus = LoginStatus.Success;
         }
         else
         {
             LoginFailureMessage = "Invalid Email or Password. Please try again.";
             LoginStatus = LoginStatus.Failed;
         }
     }
     catch (Exception ex)
     {
         Debug.WriteLine($"Error logging in: {ex}");
         LoginFailureMessage = "Server error.";
         LoginStatus = LoginStatus.Failed;
     }

     return authenticatedUser;
 }
```

The `MauiAuthenticationStateProvider` class uses the `HttpClientHelper` which handles calling localhost via the emulators and simulators for easy testing. See the [official documentation](https://learn.microsoft.com/dotnet/maui/data-cloud/local-web-services) for information on what you need to do to be able to call local services from emulators and simulators.

It also uses the `TokenStorage` class that uses [`SecureStorage` API](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage?view=net-maui-9.0) to store the user's token securely on the device. It refreshes the token if it's almost expired so the user doesn't have to login every time.

### MAUI MauiProgram.cs
The MAUI project's `MauiProgram.cs` file is where the `MauiAuthenticationStateProvider` is registered with the DI container. It also needs to register the Authorization core components where things like `AuthorizeView` are defined.

```code
 // This is the core functionality
builder.Services.AddAuthorizationCore();
// This is our custom provider
builder.Services.AddScoped<MauiAuthenticationStateProvider, MauiAuthenticationStateProvider>();
// Use our custom provider when the app needs an AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider>(s
    => (MauiAuthenticationStateProvider)s.GetRequiredService<MauiAuthenticationStateProvider>());
```
