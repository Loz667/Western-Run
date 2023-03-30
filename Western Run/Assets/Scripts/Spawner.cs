using System.Collections.Generic;
using UnityEngine;

namespace WesternRun
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private int tileStartCount = 10;
        [SerializeField] private int minStraightTiles = 3;
        [SerializeField] private int maxStraightTiles = 15;

        [SerializeField] private GameObject startTile;

        [SerializeField] private List<GameObject> turnTiles;
        [SerializeField] private List<GameObject> obstacles;

        private Vector3 currentTileLocation = Vector3.zero;
        private Vector3 currentTileDirection = Vector3.forward;

        private GameObject prevTile;
        private List<GameObject> currentTiles;
        private List<GameObject> currentObstacles;

        private void Start()
        {
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = 0; i < tileStartCount; i++)
            {
                SpawnTile(startTile.GetComponent<Tile>());
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
        }

        private void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            if (spawnObstacle) SpawnObstacle();

            if (tile.type == TileType.Straight)
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
        }

        public void AddNewDirection(Vector3 direction)
        {
            currentTileDirection = direction;
            DeletePrevious();

            Vector3 tilePlacementScale;
            if (prevTile.GetComponent<Tile>().type == TileType.Sideways)
            {
                tilePlacementScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size / 2 + 
                                    (Vector3.one * startTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }
            else
            {
                tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) +
                                    (Vector3.one * startTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }

            currentTileLocation += tilePlacementScale;

            int currentPathLength = Random.Range(minStraightTiles, maxStraightTiles);
            for (int i = 0; i < currentPathLength; i++)
            {
                SpawnTile(startTile.GetComponent<Tile>(), (i == 0) ? false : true);
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
        }

        void DeletePrevious()
        {
            while (currentTiles.Count != 1)
            {
                GameObject tile = currentTiles[0];
                currentTiles.RemoveAt(0);
                Destroy(tile);
            }

            while (currentObstacles.Count != 0)
            {
                GameObject obstacle = currentObstacles[0];
                currentObstacles.RemoveAt(0);
                Destroy(obstacle);
            }
        }

        private void SpawnObstacle()
        {
            if (Random.value > 0.2f) return;

            GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
            Quaternion newObstacleRotation = obstaclePrefab.gameObject.transform.rotation * 
                                                Quaternion.LookRotation(currentTileDirection, Vector3.up);

            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObstacleRotation);
            currentObstacles.Add(obstacle);
        }

        private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;

            return list[Random.Range(0, list.Count)];
        }
    }
}

