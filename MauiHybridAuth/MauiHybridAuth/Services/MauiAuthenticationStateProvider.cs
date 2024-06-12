using MauiHybridAuth.Shared.Services;

namespace MauiHybridAuth.Services
{
    public class MauiAuthenticationStateProvider : CustomAuthenticationStateProvider, ICustomAuthenticationStateProvider
    {
        public MauiAuthenticationStateProvider()
        {
            //See: https://learn.microsoft.com/dotnet/maui/data-cloud/local-web-services
            //Android Emulator uses 10.0.2.2 to refer to localhost            
            LoginUri =
                DeviceInfo.Platform == DevicePlatform.Android ? LoginUri.Replace("localhost", "10.0.2.2") : LoginUri;
        }

        //Override the HttpClient creation to allow for custom handling of localhost for Android and iOS
        protected override HttpClient GetHttpClient()
        {
#if WINDOWS || MACCATALYST
            return new HttpClient();
#else
            return new HttpClient(new HttpsClientHandlerService().GetPlatformMessageHandler()); 
#endif
        }
    }
    public class HttpsClientHandlerService
    {
        public HttpMessageHandler GetPlatformMessageHandler()
        {
#if ANDROID
            var handler = new Xamarin.Android.Net.AndroidMessageHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
#elif IOS
        var handler = new NSUrlSessionHandler
        {
            TrustOverrideForUrl = IsHttpsLocalhost
        };
        return handler;
#else
     throw new PlatformNotSupportedException("Only Android and iOS supported.");
#endif
        }

#if IOS
    public bool IsHttpsLocalhost(NSUrlSessionHandler sender, string url, Security.SecTrust trust)
    {
        return url.StartsWith("https://localhost");
    }
#endif
    }
}


