using System;

namespace BlazorDemo.Common.Services.Frontend
{
    public class RefreshService : IRefreshService
    {
        public event Action RefreshRequested;
        public void RequestRefresh() => RefreshRequested?.Invoke();
    }
}
