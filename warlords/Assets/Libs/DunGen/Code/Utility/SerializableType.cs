using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen
{
	// Unity won't serialize a Type on it's own so we'll need to use a wrapper which references the Type by its Assembly Qualified Name

	[Serializable]
	public sealed class SerializableType
	{
		public Type Type
		{
			get { return (string.IsNullOrEmpty(typeName)) ? null : Type.GetType(typeName); }
			set { typeName = (value == null) ? "" : value.AssemblyQualifiedName; }
		}

		[SerializeField]
		private string typeName;


		public SerializableType() { }

		public SerializableType(Type type)
		{
			Type = type;
		}

		public SerializableType(string assemblyQualifiedName)
		{
			typeName = assemblyQualifiedName;
		}


		#region Operator Overrides

		public static implicit operator Type(SerializableType serializableType)
		{
			return serializableType.Type;
		}

		public static implicit operator SerializableType(Type type)
		{
			return new SerializableType(type);
		}

		#endregion
	}
}
