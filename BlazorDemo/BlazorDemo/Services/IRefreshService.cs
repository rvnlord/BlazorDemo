using System;

namespace BlazorDemo.Services
{
    public interface IRefreshService
    {
        event Action RefreshRequested;
        void RequestRefresh();
    }
}
