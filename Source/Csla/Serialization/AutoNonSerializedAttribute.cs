//-----------------------------------------------------------------------
// <copyright file="AutoNonSerializedAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicate that a field or property should be excluded from auto serialization</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization
{

	/// <summary>
	/// Indicate that a public field or property should be excluded from auto serialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class AutoNonSerializedAttribute : Attribute
	{
	}

}