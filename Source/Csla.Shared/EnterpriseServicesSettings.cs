#if !NETFX_CORE && !MONO && !(ANDROID || IOS)
//-----------------------------------------------------------------------
// <copyright file="EnterpriseServicesSettings.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.EnterpriseServices;

// EnterpriseServices settings
[assembly: ApplicationActivation(ActivationOption.Library)]
[assembly: ApplicationName("CSLA .NET DataPortal")]
[assembly: Description("CSLA .NET Serviced DataPortal")]
[assembly: ApplicationAccessControl(false)]

#endif