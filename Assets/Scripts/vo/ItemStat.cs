using Assets.scripts.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.vo
{ 
    
    [System.Serializable]
    public class ItemStat {
        public int id;
        public String name;
        public String type;
        public int baseStat;
        public int top;

        public int getId()
        {
            return id;
        }

        public void setId(int id)
        {
            this.id = id;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public String getType()
        {
            return type;
        }

        public void setType(String type)
        {
            this.type = type;
        }

        public int getBaseStat()
        {
            return baseStat;
        }

        public void setBaseStat(int baseStat)
        {
            this.baseStat = baseStat;
        }

        public int getTop()
        {
            return top;
        }

        public void setTop(int top)
        {
            this.top = top;
        }


        public String toString()
        {
            return "ItemStat{" +
                    "id=" + id +
                    ", name='" + name + '\'' +
                    ", type='" + type + '\'' +
                    ", baseStat=" + baseStat +
                    ", top=" + top +
                    '}';
        }
    }
}