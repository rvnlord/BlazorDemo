using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CommonLibrary.Extensions
{
    public static class TExtensions
    {
        public static string GetDisplayName(this object model, string propName)
        {
            var type = model.GetType();
            return ((DisplayNameAttribute)type.GetProperty(propName)?.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName
                ?? ((type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault() as MetadataTypeAttribute)?
                    .MetadataClassType.GetProperty(propName)?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                    .SingleOrDefault() as DisplayNameAttribute)?.DisplayName 
                ?? propName;
        }
    }
}
