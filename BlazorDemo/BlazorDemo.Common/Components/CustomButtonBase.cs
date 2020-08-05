using System.Threading.Tasks;
using BlazorDemo.Common.Converters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorDemo.Common.Components
{
    public class CustomButtonBase : ComponentBase
    {
        protected string _btnClass;

        [Parameter] 
        public ButtonStyling Styling { get; set; }

        [Parameter] 
        public ButtonState ButtonState { get; set; } = ButtonState.Enabled;

        [Parameter] 
        public ButtonType ButtonType { get; set; } = ButtonType.Submit;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected override void OnParametersSet()
        {
            _btnClass = $"btn-{Styling.EnumToString().ToLowerInvariant()}";
            if (Styling == ButtonStyling.Brand)
                _btnClass = $"btn-success {_btnClass}";
        }

        protected async Task Button_ClickAsync(MouseEventArgs e) => await OnClick.InvokeAsync(e).ConfigureAwait(false);
    }

    public enum ButtonStyling
    {
        Primary,
        Light,
        Info,
        Danger, 
        Success,
        Warning,
        Input,
        Brand
    }

    public enum ButtonState
    {
        Enabled,
        Disabled,
        Loading
    }

    public enum ButtonType
    {
        Submit,
        Button
    }
}
