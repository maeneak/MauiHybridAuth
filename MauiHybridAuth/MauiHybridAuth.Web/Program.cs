using MauiHybridAuth.Shared.Services;
using MauiHybridAuth.Web.Components;
using MauiHybridAuth.Web.Services;
using MauiHybridAuth.Web.Data;
using MauiHybridAuth.Web.Components.Account;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the MauiHybridAuth.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add Auth services used by the Web app
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
});
 
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//Needed for external clients to log in
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register needed elements for authentication:
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

var app = builder.Build();

//Needed for external clients to log in
app.MapIdentityApi<ApplicationUser>();

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

app.MapAdditionalIdentityEndpoints();

app.Run();