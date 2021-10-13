//-----------------------------------------------------------------------
// <copyright file="AutoSerializedAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicate that a non-public field or property should be included in auto serialization</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization
{

	/// <summary>
	/// Indicate that a non-public field or property should be included in auto serialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class AutoSerializedAttribute : Attribute
	{

		public AutoSerializedAttribute()
		{

		}

	}

}