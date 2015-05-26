//-----------------------------------------------------------------------
// <copyright file="EnterpriseServicesSettings.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

#if !MONO && !NETFX_PHONE
using System.EnterpriseServices;

// EnterpriseServices settings
[assembly: ApplicationActivation(ActivationOption.Library)]
[assembly: ApplicationName("CSLA .NET DataPortal")]
[assembly: Description("CSLA .NET Serviced DataPortal")]
[assembly: ApplicationAccessControl(false)]

#endif