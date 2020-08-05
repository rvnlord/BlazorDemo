using System.Linq;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorDemo.Common.Extensions
{
    public static class EditContextExtensions
    {
        public static EditContext AddValidationMessages(this EditContext ec, ValidationMessageStore messageStore, ILookup<string, string> messages)
        {
            if (messages == null) 
                return ec;

            foreach (var (propertyName, message) in messages.SelectMany(g => g.Select(m => (g.Key, m))))
                messageStore.Add(new FieldIdentifier(ec.Model, propertyName), message);

            return ec;
        }
    }
}
