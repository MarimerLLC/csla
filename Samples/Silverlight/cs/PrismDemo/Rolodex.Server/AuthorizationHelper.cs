using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace Rolodex
{
    public static class AuthorizationHelper
    {
        public const string ReadWriteRole = "ReadWrite";
        public const string ReadOnlyRole = "ReadOnly";

        public static string[] WriteRoles = { ReadWriteRole };

        public static string[] ReadRoles = { ReadWriteRole, ReadOnlyRole };

        public static void AddObjectAuthorizationRules(Type objectType)
        {
            BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.CreateObject, AuthorizationHelper.WriteRoles));
            BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.DeleteObject, AuthorizationHelper.WriteRoles));
            BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.EditObject, AuthorizationHelper.WriteRoles));
            BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.GetObject, AuthorizationHelper.ReadRoles));
        }

       
    }
}
