using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class map
    {
        [SerializeField] public GameObject TilePrefab = Resources.Load<GameObject>("Tests/Tile");
        [SerializeField] public GameObject testGrid = Resources.Load<GameObject>("Tests/TutorialTestGrid");
        private Map _map;
        private GameObject go;
        private GameObject grid;


        [OneTimeSetUp]
        public void SetUp()
        {
            //Debug.Log("Step 1");
            grid = GameObject.Instantiate(testGrid,Vector3.zero, Quaternion.identity);
            go = new GameObject();
            _map = go.AddComponent<Map>();
            
            //Debug.Log("Step 2");
            _map.tileCostReference = grid.GetComponentInChildren<LevelTileCostData>();
            _map.TilePrefab = TilePrefab;
            _map.mapSize = new Vector2Int(8, 8);
            _map.bottomLeft = new Vector2Int(-4, -3);

            _map.GenerateMap();
        }
        
        [UnityTest]
        public IEnumerator tile_cost_correctly_generated()
        {
            // ARRANGE
            ITile grass = Substitute.For<ITile>();
            grass.movementCost.Returns(1);
            grass.walkable.Returns(true);

            ITile bush = Substitute.For<ITile>();
            bush.movementCost.Returns(2);
            bush.walkable.Returns(true);

            ITile obstacle = Substitute.For<ITile>();
            obstacle.movementCost.Returns(int.MaxValue);
            obstacle.walkable.Returns(false);

            // ACT 
            // done above

            // ASSERT
            Assert.AreEqual(grass.movementCost, _map.tileList[0][0].movementCost);
            Assert.AreEqual(grass.walkable, _map.tileList[0][0].walkable);
            Assert.AreEqual(bush.movementCost, _map.tileList[4][0].movementCost);
            Assert.AreEqual(bush.walkable, _map.tileList[4][0].walkable);
            Assert.AreEqual(obstacle.movementCost, _map.tileList[1][3].movementCost);
            Assert.AreEqual(obstacle.walkable, _map.tileList[1][3].walkable);

            yield return null;
        }

        [UnityTest]
        public IEnumerator astar_finds_shortest_distance_from_5x1y_to_7x0y_equals_4()
        {
            // ACT
            AStarSearch.GeneratePath(_map, _map.tileList[5][1], _map.tileList[7][0]);

            // ASSERT
            Assert.AreEqual(4, _map.tileList[7][0].distance);

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator selectable_tiles_are_generated_and_removed_correctly_movement_range_3_from_4x3y()
        {
            _map.FindSelectableTiles(_map.tileList[4][3], 3);

            bool allHighlighted = true;
            bool correctTilesHighlighted = true;

            List<Vector2Int> selectableCoords = new List<Vector2Int>() { new Vector2Int(2, 2), new Vector2Int(2, 4) , new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(3, 4),
                                    new Vector2Int(4,1), new Vector2Int(4,2), new Vector2Int(4,3), new Vector2Int(4,4), new Vector2Int(4,5), new Vector2Int(4,6), new Vector2Int(5,2), new Vector2Int(5,4), 
                                    new Vector2Int(5,5), new Vector2Int(6,4), new Vector2Int(6,2)};

            foreach (Vector2Int pos in selectableCoords)
            {
                if (_map.tileList[pos.x][pos.y].selectable != true)
                {
                    allHighlighted = false;
                }
            }

            foreach (Tile tile in _map.GetSelectableTiles())
            {
                Vector2Int curPos = new Vector2Int(tile.gridPosition.x, tile.gridPosition.y);
                if (!selectableCoords.Contains(curPos))
                {
                    correctTilesHighlighted = false;
                }
            }

            Assert.IsTrue(allHighlighted);
            Assert.IsTrue(correctTilesHighlighted);

            HashSet<Tile> selectableTilesCache = _map.GetSelectableTiles();
            _map.RemoveSelectableTiles(_map.tileList[4][3]);

            bool tilesRemoved = true;

            foreach (Tile tile in selectableTilesCache)
            {
                tilesRemoved = !tile.selectable;
            }

            Assert.IsTrue(tilesRemoved);

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            foreach(List<Tile> row in _map.tileList)
            {
                foreach(Tile tile in row)
                {
                    tile.Reset();
                }
            }
        }

        [OneTimeTearDown]
        public void OneTearDown()
        {
            GameObject.Destroy(go);
            GameObject.Destroy(grid);
        }
    }
}
