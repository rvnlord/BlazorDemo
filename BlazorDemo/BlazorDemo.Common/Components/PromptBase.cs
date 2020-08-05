using System.Threading.Tasks;
using BlazorDemo.Common.Services;
using BlazorDemo.Common.Services.Frontend;
using Microsoft.AspNetCore.Components;

namespace BlazorDemo.Common.Components
{
    public class PromptBase : ComponentBase
    {
        protected string _promptStylingClass;

        [Inject]
        public IParametersService ParametersService { get; set; }

        [Parameter] 
        public PromptType PromptType { get; set; } = PromptType.Error;

        [Parameter] 
        public string Message { get; set; } = "No Message, it shouldn't be visible";

        protected override Task OnInitializedAsync()
        {
            ParametersService.Parameters.TryGetValue(nameof(Message), out var errorMessageObj);

            if (errorMessageObj is string errorMessage && !string.IsNullOrWhiteSpace(errorMessage))
                Message = errorMessage;

            return Task.CompletedTask;
        }

        protected override Task OnParametersSetAsync()
        {
            _promptStylingClass = PromptType switch
            {
                PromptType.Success => "alert-success",
                PromptType.Error => "alert-danger",
                _ => ""
            };

            return Task.CompletedTask;
        }
    }

    public enum PromptType
    {
        Success,
        Error
    }

}
