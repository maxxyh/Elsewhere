using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class unit_stats
    {
        // A Test behaves as an ordinary method
        [Test]
        public void modifier_with_default_duration_is_not_removed()
        {
            UnitStat stat = new UnitStat(10);
            stat.AddModifier(new StatModifier(0.4f, StatModType.PercentAdd));
            stat.DecrementDuration();
            stat.DecrementDuration();
            stat.DecrementDuration();
            Assert.AreEqual(10 * 1.4f, stat.Value);

        }

        [Test]
        public void modifier_with_duration_4_is_removed_after_4_endTurns()
        {
            UnitStat stat = new UnitStat(10);
            stat.AddModifier(new StatModifier(0.4f, 4, StatModType.PercentAdd));
            stat.DecrementDuration();
            stat.DecrementDuration();
            stat.DecrementDuration();
            Assert.AreEqual(10 * 1.4f, stat.Value);
            stat.DecrementDuration();
            Assert.AreEqual(10, stat.Value);
        }

        [Test]
        public void modifiers_applied_in_sorting_order()
        {
            UnitStat stat = new UnitStat(10);
            stat.AddModifier(new StatModifier(0.4f, StatModType.PercentAdd));
            stat.AddModifier(new StatModifier(-0.3f, StatModType.PercentMult));
            Assert.AreEqual(10 * 1.4f * 0.7f, stat.Value);
            stat.AddModifier(new StatModifier(-5, StatModType.Flat));
            Assert.AreEqual((10 - 5) * 1.4f * 0.7f, stat.Value);
            stat.AddModifier(new StatModifier(0.3f, StatModType.PercentMult));
            Assert.AreEqual((10 - 5) * 1.4f * 0.91f, stat.Value);
            stat.AddModifier(new StatModifier(0.3f, StatModType.PercentAdd));
            Assert.AreEqual((10 - 5) * 1.7f * 0.7f * 1.3f, stat.Value);
        }

        [Test]
        public void modifiers_applied_in_custom_sorting_order()
        {
            UnitStat stat = new UnitStat(10);
            stat.AddModifier(new StatModifier(0.4f, StatModType.PercentAdd,1));
            stat.AddModifier(new StatModifier(-0.3f, StatModType.PercentMult,2));
            Assert.AreEqual(10 * 1.4f * 0.7f, stat.Value);
            stat.AddModifier(new StatModifier(-5, StatModType.Flat,3));
            Assert.AreEqual(10 * 1.4f * 0.7f - 5, stat.Value);
            stat.AddModifier(new StatModifier(0.3f, StatModType.PercentMult,4));
            Assert.AreEqual(1.3f *(10 * 1.4f * 0.7f - 5), stat.Value);
        }

        [Test]
        public void unitstat_cannot_increase_beyond_limit()
        {
            UnitStat stat = new UnitStat(10, true);
            stat.AddModifier(new StatModifier(-3, StatModType.Flat));
            stat.AddModifier(new StatModifier(5, StatModType.Flat));
            Assert.AreEqual(10, stat.Value);
        }

        [Test]
        public void unitstat_cannot_decrease_beyond_zero()
        {
            UnitStat stat = new UnitStat(10);
            stat.AddModifier(new StatModifier(-15, StatModType.Flat));
            Assert.AreEqual(0, stat.Value);
        }
    }
}
