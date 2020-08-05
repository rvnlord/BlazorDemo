using System;
using System.Collections.Generic;

namespace BlazorDemo.Common.Models.Admin.ViewModels
{
    public class FindRoleVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> UserNames { get; set; } = new List<string>();

        public override bool Equals(object o)
        {
            if (!(o is FindRoleVM))
                return false;

            var that = (FindRoleVM) o;

            return Name == that.Name;
        }

        public override int GetHashCode() => Name.GetHashCode() ^ 3 * Id.GetHashCode() ^ 5 * UserNames.GetHashCode() ^ 7;
    }
}
