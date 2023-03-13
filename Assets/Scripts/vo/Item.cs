using Assets.scripts.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.vo
{ 
	[System.Serializable]
    public class Item {
	    public int id;
        public int heroId;
        public int itemId;
        public int positionId;
        public String name;
        public String position;
        public String image;
        public String classType;
        public String rarity;
        public int baseStat;
        public int top;
        public int levelReq;
        public int statId_1;
        public int statId_2;
        public int statId_3;
        public int statId_4;
        public bool equipped = false;
        public List<ItemStat> stats;

	    public int getId() {
		    return id;
	    }

	    public void setId(int id) {
		    this.id = id;
	    }

	    public int getHeroId() {
		    return heroId;
	    }

	    public void setHeroId(int heroId) {
		    this.heroId = heroId;
	    }

	    public int getItemId() {
		    return itemId;
	    }

	    public void setItemId(int itemId) {
		    this.itemId = itemId;
	    }

	    public String getName() {
		    return name;
	    }

	    public void setName(String name) {
		    this.name = name;
	    }

	    public String getPosition() {
		    return position;
	    }

	    public void setPosition(String position) {
		    this.position = position;
	    }

        public int getPositionId()
        {
            return positionId;
        }

        public void setPositionId(int positionId)
        {
            this.positionId = positionId;
        }

        public String getImage() {
		    return image;
	    }

	    public void setImage(String image) {
		    this.image = image;
	    }

	    public String getClassType() {
		    return classType;
	    }

	    public void setClassType(String classType) {
		    this.classType = classType;
	    }

	    public String getRarity() {
		    return rarity;
	    }

	    public void setRarity(String rarity) {
		    this.rarity = rarity;
	    }

	    public int getBaseStat() {
		    return baseStat;
	    }

	    public void setBaseStat(int baseStat) {
		    this.baseStat = baseStat;
	    }

	    public int getTop() {
		    return top;
	    }

	    public void setTop(int top) {
		    this.top = top;
	    }

	    public int getLevelReq() {
		    return levelReq;
	    }

	    public void setLevelReq(int levelReq) {
		    this.levelReq = levelReq;
	    }

	    public int getStatId_1() {
		    return statId_1;
	    }

	    public void setStatId_1(int statId_1) {
		    this.statId_1 = statId_1;
	    }

	    public int getStatId_2() {
		    return statId_2;
	    }

	    public void setStatId_2(int statId_2) {
		    this.statId_2 = statId_2;
	    }

	    public int getStatId_3() {
		    return statId_3;
	    }

	    public void setStatId_3(int statId_3) {
		    this.statId_3 = statId_3;
	    }

	    public int getStatId_4() {
		    return statId_4;
	    }

	    public void setStatId_4(int statId_4) {
		    this.statId_4 = statId_4;
	    }

	    public bool isEquipped() {
		    return equipped;
	    }

	    public void setEquipped(bool equipped) {
		    this.equipped = equipped;
	    }
        
        public List<ItemStat> getItemStats()
        {
            return stats;
        }

        public void setItemStats(List<ItemStat> stats)
        {
            this.stats = stats;
        }

	    public String toString() {
		    return "Item{" +
				    "id=" + id +
				    ", heroId=" + heroId +
				    ", itemId=" + itemId +
				    ", name='" + name + '\'' +
				    ", position='" + position + '\'' +
				    ", image='" + image + '\'' +
				    ", classType='" + classType + '\'' +
				    ", rarity='" + rarity + '\'' +
                    ", baseStat=" + baseStat +
				    ", top=" + top +
				    ", levelReq=" + levelReq +
				    ", statId_1=" + statId_1 +
				    ", statId_2=" + statId_2 +
				    ", statId_3=" + statId_3 +
				    ", statId_4=" + statId_4 +
				    ", equipped=" + equipped +
				    '}';
	    }
    }
}