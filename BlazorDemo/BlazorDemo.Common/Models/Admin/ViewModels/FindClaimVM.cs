﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorDemo.Common.Models.Admin.ViewModels
{
    public class FindClaimVM
    {
        public string Name { get; set; }
        public List<FindClaimValueVM> Values { get; set; } = new List<FindClaimValueVM>();
        public string OriginalName { get; set; } // for validation attr purposes

        public List<string> GetUserNames() => Values.SelectMany(cv => cv.UserNames).OrderBy(n => n).ToList();

        public override bool Equals(object o)
        {
            if (!(o is FindClaimVM))
                return false;

            var that = (FindClaimVM) o;

            return Name == that.Name;
        }

        public override int GetHashCode() => Name.GetHashCode() ^ 3 * Values.GetHashCode() ^ 7;
    }
}
