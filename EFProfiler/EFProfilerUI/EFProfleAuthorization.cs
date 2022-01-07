using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFProfiler.EFProfilerUI
{
    public class EFProfleAuthorization
    {
        private static readonly string[] EmptyArray = new string[0];

        private string _roles;
        private string[] _rolesSplit = EmptyArray;
        private string _users;
        private string[] _usersSplit = EmptyArray;

        public string Roles
        {
            get { return _roles ?? String.Empty; }
            set
            {
                _roles = value;
                _rolesSplit = SplitString(value);
            }
        }
        public string Users
        {
            get { return _users ?? String.Empty; }
            set
            {
                _users = value;
                _usersSplit = SplitString(value);
            }
        }

        internal bool Authorize(HttpContext httpContext)
        {
            if (httpContext.User == null || !httpContext.User.Identities.Any() || !httpContext.User.Identities.First().IsAuthenticated)
            {
                return false;
            }

            if (_usersSplit.Length > 0 && !_usersSplit.Contains(httpContext.User.Identities.First().Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            if (_rolesSplit.Length > 0)
            {
                for (int i = 0; i < _rolesSplit.Length; i++)
                {
                    var roleClaim = httpContext.User.Identities.FirstOrDefault().Claims.FirstOrDefault(a => a.Type == System.Security.Claims.ClaimTypes.Role);
                    if (roleClaim != null && roleClaim.Value.ToLower().Contains(_rolesSplit[i].ToLower()))
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }

        private static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return EmptyArray;
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}
