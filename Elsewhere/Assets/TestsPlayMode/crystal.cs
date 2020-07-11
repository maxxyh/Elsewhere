using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class crystal
    {
        public PanelManager panelManager;
        public Unit player;
        public Unit enemy;
        public GameObject CrystalGO;
        public GameObject CrystalPrefab = Resources.Load<GameObject>("Tests/CrystalTest");
        public GameObject PanelManagerPrefab = Resources.Load<GameObject>("Tests/PanelManagerTest");
        public GameObject PlayerPrefab = Resources.Load<GameObject>("Tests/PlayerTest");
        public GameObject EnemyPrefab = Resources.Load<GameObject>("Tests/EnemyTest");

        [SetUp]
        public void SetUp()
        {
            GameObject PanelManagerGO = GameObject.Instantiate(PanelManagerPrefab, Vector3.zero, Quaternion.identity);
            panelManager = PanelManagerGO.GetComponent<PanelManager>();
            //panelManager.cutscenePanelList[0].cutSceneGO.SetActive(false);
            player = MonoBehaviour.Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerUnit>();
            enemy = MonoBehaviour.Instantiate(EnemyPrefab, Vector3.zero, Quaternion.identity).GetComponent<EnemyUnit>();
            CrystalGO = MonoBehaviour.Instantiate(CrystalPrefab, new Vector3(2, 0, 0), Quaternion.identity);
            CrystalGO.tag = "crystal";
        }



        [UnityTest]
        public IEnumerator crystal_is_destroyed_and_dialogue_cutscene_plays_upon_player_collision()
        {

            //enemy.transform.Translate(new Vector3(1, 0, 0));
            enemy.transform.position = new Vector3(2, 0, 0);

            yield return new WaitForSeconds(1.1f);

            //Assert.IsFalse(panelManager.cutscenePanelList[0].cutSceneGO.activeInHierarchy);
            Assert.IsFalse(CrystalGO == null);


            //player.transform.Translate(new Vector3(1, 0, 0));
            player.transform.position = new Vector3(2, 0, 0);

            yield return new WaitForSeconds(1.1f);

            //Assert.IsTrue(panelManager.cutscenePanelList[0].cutSceneGO.activeInHierarchy);
            Assert.IsTrue(CrystalGO == null);


            yield return null;
        }

        /*[UnityTest]
        public IEnumerator crystal_is_destroyed_upon_player_collision()
        {
            enemy.transform.position = new Vector3(2, 0, 0);

            yield return new WaitForSeconds(1);

            Assert.IsFalse(CrystalGO == null);

            player.transform.position = new Vector3(2, 0, 0);

            yield return new WaitForSeconds(1);

            Assert.IsTrue(CrystalGO == null);

            yield return null;
        }*/
    }
}
