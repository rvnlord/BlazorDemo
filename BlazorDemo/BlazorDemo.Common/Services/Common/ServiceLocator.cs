using BlazorDemo.Common.Services.Common.Interfaces;

namespace BlazorDemo.Common.Services.Common
{
    public static class ServiceLocator
    {
        private static IServiceProviderProxy diProxy;

        public static IServiceProviderProxy ServiceProvider => diProxy;

        public static void Initialize(IServiceProviderProxy proxy)
        {
            diProxy = proxy;
        }
    }
}
