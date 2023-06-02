using DunnoAlternative.Shared;
using DunnoAlternative.World;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.Battle
{
    public struct SoldierAttack
    {
        public float moveSpeedMultPre;
        public float moveSpeedMultPost;

        public Vector2f preparationTime;
        public Vector2f postTime;
        public Vector2f damage;

        public float initRangeMax;
        public float continueRangeMax;

        public int Ammo; //negative for infinite ammo
        public float skirmishMaxRange;
        public float skirmishNoAttackRange;
        public float skirmishMinRange;

        public SoldierAttack(Attack baseAttack)
        {
            moveSpeedMultPost = baseAttack.moveSpeedMultPost;
            moveSpeedMultPre = baseAttack.moveSpeedMultPre;
            preparationTime = baseAttack.preparationTime;
            postTime = baseAttack.postTime;
            damage = baseAttack.damage;

            initRangeMax = baseAttack.initRangeMax.RandomFromRange();
            continueRangeMax = baseAttack.continueRangeMax.RandomFromRange();

            Ammo = baseAttack.Ammo;
            skirmishMaxRange = baseAttack.skirmishMaxRange;
            skirmishNoAttackRange = baseAttack.skirmishNoAttackRange;
            skirmishMinRange = baseAttack.skirmishMinRange;
        }

        //min range
    }
}
