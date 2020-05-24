using System;

namespace BlazorDemo.Services
{
    public class RefreshService : IRefreshService
    {
        public event Action RefreshRequested;
        public void RequestRefresh() => RefreshRequested?.Invoke();
    }
}
