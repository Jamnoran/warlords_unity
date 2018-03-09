using UnityEngine;
using System;

namespace UnityEngine.UI
{
	[Serializable]
	public class UIItemInfo
	{
		public int ID;
        public int ItemId;
		public string Name;
		public Sprite Icon;
		public string Description;
        public UIItemQuality Quality;
        public UIEquipmentType EquipType;
		public int ItemType;
		public string Type;
		public string Subtype;
		public int Damage;
		public float AttackSpeed;
		public int Block;
		public int Armor;
		public int Stamina;
		public int Strength;
	}
}
