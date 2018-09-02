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
            item.Type = "Sword";
            item.ItemType = (int)UIEquipmentType.Weapon_MainHand;
            item.Subtype = "One handed";
            item.Icon = Resources.Load<Sprite>("Spells/" + updatedItem.image);
            item.EquipType = UIEquipmentType.Weapon_MainHand;
            return item;
        }
    }
}
