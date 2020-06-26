using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ui_stat_panel
    {
        TurnScheduler turnScheduler;
        GameObject turnSchedulerGO;
        GameObject unitGO;
        Tile tile;
        PlayerUnit unit;
        GameAssets gameAssets;
        public GameObject TilePrefab = Resources.Load<GameObject>("Tests/Tile");
        public GameObject PlayerPrefab = Resources.Load<GameObject>("Tests/PlayerTest");

        [SetUp]
        public void SetUp()
        {
            turnSchedulerGO = new GameObject("turnScheduler", typeof(TurnScheduler));
            turnScheduler = turnSchedulerGO.GetComponent<TurnScheduler>();
            tile = MonoBehaviour.Instantiate(TilePrefab, Vector3.zero, Quaternion.identity).GetComponent<Tile>();
            unitGO = MonoBehaviour.Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            unit = unitGO.GetComponent<PlayerUnit>();
        }


        [UnityTest]
        public IEnumerator major_stat_panel_appears_on_right_click_and_small_stat_panel_on_hover()
        {
            tile.occupied = true;         
            unit.statPanel = unit.statPanelGO.GetComponent<StatPanel>();

            unit.currentTile = tile;
            Assert.IsTrue(unit.majorStatPanel != null);

            List<PlayerUnit> players = new List<PlayerUnit>() { unit };
            List<EnemyUnit> enemies = new List<EnemyUnit>();
            turnScheduler.players = players;
            turnScheduler.enemies = enemies;

            gameAssets = new GameObject("gameAssets", typeof(GameAssets)).GetComponent<GameAssets>();
            GameAssets.MyInstance.turnScheduler = turnScheduler;

            yield return null;

            tile.CheckAndActivateMajorStatPanel();

            Assert.IsTrue(unit.majorStatPanel.gameObject.activeInHierarchy);

            tile.OnMouseEnter();
            
            Assert.IsTrue(unit.statPanelGO.activeInHierarchy);

            yield return null;
        }


        [UnityTest]
        public IEnumerator stat_panels_get_updated_after_skills_used_and_are_rounded_to_1dp()
        {
            unit.statPanel = unit.statPanelGO.GetComponent<StatPanel>();

            Dictionary<StatString, UnitStat> defaultStats = new Dictionary<StatString, UnitStat>();

            defaultStats.Add(StatString.PHYSICAL_DAMAGE, new UnitStat(20));
            defaultStats.Add(StatString.MAGIC_DAMAGE, new UnitStat(20));
            defaultStats.Add(StatString.MANA, new UnitStat(20));
            defaultStats.Add(StatString.HP, new UnitStat(10));
            defaultStats.Add(StatString.ARMOR, new UnitStat(10));
            defaultStats.Add(StatString.MAGIC_RES, new UnitStat(5));
            defaultStats.Add(StatString.MOVEMENT_RANGE, new UnitStat(4));
            defaultStats.Add(StatString.ATTACK_RANGE, new UnitStat(2));

            unit.stats = defaultStats;
            unit.UpdateUI();

            Assert.AreEqual("20", unit.statPanel.unitPhysicalDamage.text);
            Assert.AreEqual("20", unit.majorStatPanel.unitPhysicalDamage.text);

            // simulating astral flare used on it.
            int attackDamage = 5;
            float armorDebuff = 0.2f;
            unit.stats[StatString.HP].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
            unit.stats[StatString.ARMOR].AddModifier(new StatModifier(-armorDebuff, StatModType.PercentAdd));
            unit.stats[StatString.MAGIC_RES].AddModifier(new StatModifier(0.1444f, StatModType.PercentAdd));

            unit.UpdateUI();

            Debug.Log($"Magic Res = {unit.statPanel.unitMagicRes.text}");
            Assert.AreEqual("5/10", unit.statPanel.unitHP.text);
            Assert.AreEqual("5/10", unit.majorStatPanel.unitHP.text);
            Assert.AreEqual("8(-2)", unit.statPanel.unitArmor.text);
            Assert.AreEqual("5.7(+0.7)", unit.statPanel.unitMagicRes.text);
            Assert.AreEqual("8(-2)", unit.majorStatPanel.unitArmor.text);
            Assert.AreEqual("5.7(+0.7)", unit.majorStatPanel.unitMagicRes.text);


            yield return null;
        }

        /*[TearDown]
        public void OneTimeTearDown()
        {
            Object.Destroy(turnSchedulerGO);
            Object.Destroy(unitGO);
            Object.Destroy(tile.gameObject);
            Object.Destroy(gameAssets.gameObject);
        }*/
    }
}
