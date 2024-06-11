using MauiHybridAuth.Shared.Services;
using MauiHybridAuth.Web.Components;
using MauiHybridAuth.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the MauiHybridAuth.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Identity services
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//TODO: Add real DB later
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("AppDb"));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register needed elements for authentication:
// This is our custom provider
builder.Services.AddScoped<ICustomAuthenticationStateProvider, BlazorAuthenticationStateProvider>();
// Use our custom provider when the app needs an AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider>(s
    => (BlazorAuthenticationStateProvider)s.GetRequiredService<ICustomAuthenticationStateProvider>());

var app = builder.Build();
app.MapIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MauiHybridAuth.Shared._Imports).Assembly);

app.Run();
