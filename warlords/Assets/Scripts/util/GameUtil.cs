using Assets.scripts.vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.util {
    class GameUtil {
        public static UIItemInfo convertItemToItemInfo(Item updatedItem) {
            UIItemInfo item = new UIItemInfo();
            item.Name = updatedItem.name;
            item.ItemId = updatedItem.id;
            item.Quality = UIItemQuality.Common;
            item.Description = "Cool item";
            item.AttackSpeed = 1.0f;
            item.Type = getItemTypeName(updatedItem);
            item.ItemType = (int)getItemType(updatedItem);
            item.Subtype = "One handed";
            item.Icon = Resources.Load<Sprite>("Collection/RPG_inventory_icons/" + updatedItem.image);
            item.EquipType = getItemType(updatedItem);
            return item;
        }

        private static UIEquipmentType getItemType(Item updatedItem)
        {
            if (updatedItem.position.Equals("MAIN_HAND"))
            {
                return UIEquipmentType.Weapon_MainHand;
            }
            else if (updatedItem.position.Equals("OFF_HAND"))
            {
                return UIEquipmentType.Weapon_OffHand;
            }
            else if (updatedItem.position.Equals("HEAD"))
            {
                return UIEquipmentType.Head;
            }
            else if (updatedItem.position.Equals("SHOULDERS"))
            {
                return UIEquipmentType.Shoulders;
            }
            else if (updatedItem.position.Equals("CHEST"))
            {
                return UIEquipmentType.Chest;
            }
            else if (updatedItem.position.Equals("LEGS"))
            {
                return UIEquipmentType.Pants;
            }
            else if (updatedItem.position.Equals("BOOTS"))
            {
                return UIEquipmentType.Boots;
            }
            return UIEquipmentType.Weapon_MainHand;
        }


        private static String getItemTypeName(Item updatedItem)
        {
            if (updatedItem.position.Equals("MAIN_HAND"))
            {
                return "Main hand";
            }
            else if (updatedItem.position.Equals("OFF_HAND"))
            {
                return "Off hand";
            }
            else if (updatedItem.position.Equals("HEAD"))
            {
                return "Head";
            }
            else if (updatedItem.position.Equals("SHOULDERS"))
            {
                return "Shoulders";
            }
            else if (updatedItem.position.Equals("CHEST"))
            {
                return "Chest";
            }
            else if (updatedItem.position.Equals("LEGS"))
            {
                return "Legs";
            }
            else if (updatedItem.position.Equals("BOOTS"))
            {
                return "Boots";
            }
            return "Main hand";
        }
    }
}
