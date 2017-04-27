using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen.Adapters
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class AdapterDisplayName : Attribute
	{
		public string Name { get; private set; }


		public AdapterDisplayName(string name)
		{
			Name = name;
		}
    }

	[Serializable]
	public abstract class PortalCullingAdapter
	{
		public virtual void Clear() { }
		public abstract PortalCullingAdapter Clone();
		public abstract void PrepareForCulling(DungeonGenerator generator, Dungeon dungeon);
		public abstract void ChangeDoorState(Door door, bool isOpen);

		[Conditional("UNITY_EDITOR")]
		public virtual void OnInspectorGUI(DungeonGenerator generator, bool isRuntimeDungeon) { }
	}
}
