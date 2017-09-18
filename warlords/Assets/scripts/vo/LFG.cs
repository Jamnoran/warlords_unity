using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.vo {
   public class LFG {

        // General stats
        public int id = 0;
        public int heroId1 = 0;
        public string heroClass1 = null;
        public string herolobby1 = null;
        public int heroId2 = 0;
        public string heroClass2 = null;
        public string herolobby2 = null;
        public int heroId3 = 0;
        public string heroClass3 = null;
        public string herolobby3 = null;
        public int heroId4 = 0;
        public string heroClass4 = null;
        public string herolobby4 = null;
        public string gameType = null;
        public int highestLevel = 0;
        public int heroesJoined = 0;
        public int maxPlayers = 4;

        public int getId() {
            return id;
        }

        public void setId(int id) {
            this.id = id;
        }

        public int getHeroId1() {
            return heroId1;
        }

        public void setHeroId1(int heroId1) {
            this.heroId1 = heroId1;
        }

        public int getHeroId2() {
            return heroId2;
        }

        public void setHeroId2(int heroId2) {
            this.heroId2 = heroId2;
        }

        public int getHeroId3() {
            return heroId3;
        }

        public void setHeroId3(int heroId3) {
            this.heroId3 = heroId3;
        }

        public int getHeroId4() {
            return heroId4;
        }

        public void setHeroId4(int heroId4) {
            this.heroId4 = heroId4;
        }

        public string getGameType() {
            return gameType;
        }

        public void setGameType(string gameType) {
            this.gameType = gameType;
        }

        public string getHeroClass1() {
            return heroClass1;
        }

        public void setHeroClass1(string heroClass1) {
            this.heroClass1 = heroClass1;
        }

        public string getHeroClass2() {
            return heroClass2;
        }

        public void setHeroClass2(string heroClass2) {
            this.heroClass2 = heroClass2;
        }

        public string getHeroClass3() {
            return heroClass3;
        }

        public void setHeroClass3(string heroClass3) {
            this.heroClass3 = heroClass3;
        }

        public string getHeroClass4() {
            return heroClass4;
        }

        public void setHeroClass4(string heroClass4) {
            this.heroClass4 = heroClass4;
        }

        public int getHighestLevel() {
            return highestLevel;
        }

        public void setHighestLevel(int highestLevel) {
            this.highestLevel = highestLevel;
        }

        public string getHerolobby1() {
            return herolobby1;
        }

        public void setHerolobby1(string herolobby1) {
            this.herolobby1 = herolobby1;
        }

        public string getHerolobby2() {
            return herolobby2;
        }

        public void setHerolobby2(string herolobby2) {
            this.herolobby2 = herolobby2;
        }

        public string getHerolobby3() {
            return herolobby3;
        }

        public void setHerolobby3(string herolobby3) {
            this.herolobby3 = herolobby3;
        }

        public string getHerolobby4() {
            return herolobby4;
        }

        public void setHerolobby4(string herolobby4) {
            this.herolobby4 = herolobby4;
        }

        public int getHeroesJoined() {
            return heroesJoined;
        }

        public void setHeroesJoined(int heroesJoined) {
            this.heroesJoined = heroesJoined;
        }

        public int getMaxPlayers() {
            return maxPlayers;
        }

        public void setMaxPlayers(int maxPlayers) {
            this.maxPlayers = maxPlayers;
        }
    }
}
