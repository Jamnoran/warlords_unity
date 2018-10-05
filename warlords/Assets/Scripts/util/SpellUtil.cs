using Assets.scripts.vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.util {
    class SpellUtil {

        public static Ability getSpell(int abilityId) {
            Ability abi = new Ability();
            switch (abilityId)
            {
                case 7:
                    abi.name = "CLEAVE";
                    abi.initialCast = true;
                    break;
                case 8:
                    abi.name = "TAUNT";
                    abi.initialCast = true;
                    break;

            }
            return abi;
        }
    }
}
