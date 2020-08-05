using System;

namespace BlazorDemo.Common.Services.Frontend
{
    public interface IRefreshService
    {
        event Action RefreshRequested;
        void RequestRefresh();
    }
}
