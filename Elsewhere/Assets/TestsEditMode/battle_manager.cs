using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class battle_manager
    {

        Dictionary<StatString, UnitStat> defaultStats = new Dictionary<StatString, UnitStat>();

        [SetUp]
        public void SetUp()
        {
            if (defaultStats.Count == 0 )
            {
                defaultStats.Add(StatString.PHYSICAL_DAMAGE, new UnitStat(20));
                defaultStats.Add(StatString.MAGIC_DAMAGE, new UnitStat(20));
                defaultStats.Add(StatString.MANA, new UnitStat(20));
                defaultStats.Add(StatString.HP, new UnitStat(10));
                defaultStats.Add(StatString.ARMOR, new UnitStat(10));
                defaultStats.Add(StatString.MAGIC_RES, new UnitStat(10));
                defaultStats.Add(StatString.MOVEMENT_RANGE, new UnitStat(4));
                defaultStats.Add(StatString.ATTACK_RANGE, new UnitStat(2));
            }
        }


        // A Test behaves as an ordinary method
        [Test]
        public void base_damage_is_armor_minus_attack_equals_10()
        {
            // ARRANGE
            IUnit attacker = Substitute.For<IUnit>();
            IUnit recipient = Substitute.For<IUnit>();
            attacker.stats.Returns(defaultStats);
            attacker.characterClass.Returns("Swordsman");
            recipient.stats.Returns(defaultStats);

            // ACT
            float result = BattleManager.CalculateBaseDamage(attacker, recipient);

            // ASSERT 
            Assert.AreEqual(10, result);

        }

        [Test]
        public void physical_damage_is_armor_minus_attack()
        {
            // ARRANGE
            float attackDamage = 20;
            IUnit recipient = Substitute.For<IUnit>();
            recipient.stats.Returns(defaultStats);

            // ACT
            float result = BattleManager.CalculatePhysicalDamage(attackDamage, recipient);

            // ASSERT 
            Assert.AreEqual(10, result);
        }

        [Test]
        public void magic_damage_is_mres_minus_mattack()
        {
            // ARRANGE
            float attackDamage = 20;
            IUnit recipient = Substitute.For<IUnit>();
            recipient.stats.Returns(defaultStats);

            // ACT
            float result = BattleManager.CalculateMagicDamage(attackDamage, recipient);

            // ASSERT 
            Assert.AreEqual(10, result);
        }

    }
}
