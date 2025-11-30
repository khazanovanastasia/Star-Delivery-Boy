using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private TilemapGenerator generator;
    [SerializeField] private List<GameObject> objectsToPlace;
    [SerializeField] private int objectsCount = 5;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        PlaceObjects();
    }

    public void PlaceObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }
        spawnedObjects.Clear();

        if (generator == null || generator.floorTilesSet == null) return;

        List<Vector2Int> validTiles = new List<Vector2Int>();

        foreach (var tile in generator.floorTilesSet)
        {
            if (!generator.IsCorridor(tile, generator.floorTilesSet))
                validTiles.Add(tile);
        }


        for (int i = 0; i < objectsCount; i++)
        {
            if (validTiles.Count == 0) break;

            int idx = Random.Range(0, validTiles.Count);
            Vector2Int pos = validTiles[idx];
            validTiles.RemoveAt(idx);

            GameObject prefab = objectsToPlace[Random.Range(0, objectsToPlace.Count)];
            Vector3 worldPos = generator.floorTilemap.CellToWorld(new Vector3Int(pos.x, pos.y, 0));
            worldPos.y += generator.floorTilemap.cellSize .y / 2f;
            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            spawnedObjects.Add(obj);
        }
    }
}
