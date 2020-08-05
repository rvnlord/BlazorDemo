using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BlazorDemo.Common.Extensions.Collections;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorDemo.Common.Components
{
    public class CustomValidationMessage<TValue> : ComponentBase, IDisposable
    {
        private EditContext? _previousEditContext;
        private Expression<Func<TValue>>? _previousFieldAccessor;
        private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;
        private FieldIdentifier _fieldIdentifier;

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }
        [CascadingParameter] private EditContext CurrentEditContext { get; set; } = default!;
        [Parameter] public Expression<Func<TValue>> For { get; set; }

        public CustomValidationMessage()
        {
            _validationStateChangedHandler = (sender, eventArgs) => StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a cascading parameter " +
                    $"of type {nameof(EditContext)}. For example, you can use {GetType()} inside " +
                    $"an {nameof(EditForm)}.");
            }

            if (For == null) // Not possible except if you manually specify T
            {
                throw new InvalidOperationException($"{GetType()} requires a value for the " +
                    $"{nameof(For)} parameter.");
            }
            if (For != _previousFieldAccessor)
            {
                _fieldIdentifier = FieldIdentifier.Create(For);
                _previousFieldAccessor = For;
            }

            if (CurrentEditContext != _previousEditContext)
            {
                DetachValidationStateChangedListener();
                CurrentEditContext.OnValidationStateChanged += _validationStateChangedHandler;
                _previousEditContext = CurrentEditContext;
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var classes = ((Dictionary<string, object>)AdditionalAttributes).VorN("class")?.ToString().Split(" ").ToList() ?? new List<string>();
            classes.Insert(0, "validation-message");
            var strClasses = string.Join(" ", classes);

            foreach (var message in CurrentEditContext.GetValidationMessages(_fieldIdentifier))
            {
                builder.OpenElement(0, "div");
                builder.AddMultipleAttributes(1, AdditionalAttributes);
                builder.AddAttribute(2, "class", strClasses);
                builder.AddContent(3, message);
                builder.CloseElement();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            DetachValidationStateChangedListener();
            Dispose(disposing: true);
        }

        private void DetachValidationStateChangedListener()
        {
            if (_previousEditContext != null)
                _previousEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
        }
    }
}
