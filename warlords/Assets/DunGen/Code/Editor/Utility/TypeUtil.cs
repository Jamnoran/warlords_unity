using DunGen.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DunGen.Editor
{
	public static class TypeUtil
	{
		public static bool GetAdapterTypesInfo(Type parentType, out Type[] types, out string[] names, bool includeEmptySlotAtBeginning = false)
		{
			List<Type> typesList = GetValidSubtypes(parentType).ToList();

			if (includeEmptySlotAtBeginning)
				typesList.Insert(0, null);

			types = typesList.ToArray();

			names = new string[types.Length];

			for (int i = 0; i < names.Length; i++)
			{
				Type type = types[i];

				if (type == null)
					names[i] = "None";
				else
				{
					AdapterDisplayName nameAtt = type.GetCustomAttributes(typeof(AdapterDisplayName), false).FirstOrDefault() as AdapterDisplayName;
					names[i] = (nameAtt != null) ? nameAtt.Name : StringUtil.SplitCamelCase(type.Name);
				}
			}

			return (includeEmptySlotAtBeginning) ? types.Length > 1 : types.Length > 0;
		}

		public static bool IsValidSubtypeOf(this Type childType, Type parentType)
		{
			if (childType == null || parentType == null)
				return false;

			return !childType.IsAbstract && parentType.IsAssignableFrom(childType);
		}

		public static IEnumerable<Type> GetValidSubtypes(this Type parentType, Assembly assembly = null)
		{
			if (assembly == null)
				assembly = typeof(DungeonGenerator).Assembly;

			foreach (var type in assembly.GetTypes())
				if (type.IsValidSubtypeOf(parentType))
					yield return type;
		}
	}
}
