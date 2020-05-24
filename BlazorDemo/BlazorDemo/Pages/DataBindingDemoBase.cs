using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Pages
{
    public class DataBindingDemoBase : ComponentBase
    {
        protected string Name { get; set; } = "Tom";
        protected string Gender { get; set; } = "Male";
        protected string BgStyle { get; set; } = "background: transparent";
        protected string Description { get; set; }
    }
}
