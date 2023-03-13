using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.vo
{
    [System.Serializable]
    public class Talent {

        public int id;
        public int heroId;
        public int spellId;
        public int talentId;
        public string description;
        public float baseValue;
        public float scaling;
        public int pointAdded;
        public int position;
        public int maxPoints;
        private GameObject gameObject;

        public Talent() {
        }

        public Talent(int id, int heroId, int talentId) {
            this.id = id;
            this.heroId = heroId;
            this.talentId = talentId;
        }

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

        public int getSpellId() {
            return spellId;
        }

        public void setSpellId(int spellId) {
            this.spellId = spellId;
        }

        internal void setGameObject(GameObject talentHolder) {
            gameObject = talentHolder;
        }

        public string getDescription() {
            return description;
        }

        public int getTalentId() {
            return talentId;
        }

        public void setTalentId(int talentId) {
            this.talentId = talentId;
        }

        public void setDescription(string description) {
            this.description = description;
        }

	    public float getBaseValue() {
		    return baseValue;
	    }

	    public void setBaseValue(float baseValue) {
		    this.baseValue = baseValue;
	    }

        public GameObject getGameObject() {
            return gameObject;
        }

        public float getScaling() {
            return scaling;
        }

        public int getPosition() {
            return position;
        }

        public void setPosition(int position) {
            this.position = position;
        }

        public void setScaling(float scaling) {
            this.scaling = scaling;
        }

        public int getPointAdded() {
            return pointAdded;
        }

        public void setPointAdded(int pointAdded) {
            this.pointAdded = pointAdded;
        }

        public int getMaxPoints()
        {
            return maxPoints;
        }

        public void setMaxPoints(int points)
        {
            maxPoints = points;
        }
    }
}