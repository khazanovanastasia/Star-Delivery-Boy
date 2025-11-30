using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField] private ObjectPlacer objectPlacer;

    [Header("Tilemap")]
    public Tilemap floorTilemap;
    [SerializeField] private TileSetData floorTileset;
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private TileSetData backgroundTileset;

    [Header("Floor Generation")]
    [SerializeField] private int iterations = 10;
    [SerializeField] private int walkLength = 10;

    [Header("Background Generation")]
    [SerializeField] private int backgroundWalks = 3;
    [SerializeField] private int backgroundWalkLength = 12;
    [SerializeField] private int backgroundPadding = 4;
    [SerializeField] private int backgroundBoxSize = 12;

    public HashSet<Vector2Int> floorTilesSet;
    public HashSet<Vector2Int> backgroundTilesSet;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        floorTilemap.ClearAllTiles();
        backgroundTilemap.ClearAllTiles();

        GenerateFloor();
        GenerateBackground();

        PaintTiles();

        if (objectPlacer != null)
            objectPlacer.PlaceObjects();
    }

    private void GenerateFloor()
    {
        floorTilesSet = new HashSet<Vector2Int>();
        Vector2Int pos = Vector2Int.zero;

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < walkLength; j++)
            {
                floorTilesSet.Add(pos);
                pos += GetRandomDirection();
            }
        }
    }

    private void GenerateBackground()
    {
        backgroundTilesSet = new HashSet<Vector2Int>();

        if (floorTilesSet.Count == 0) return;

        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (var pos in floorTilesSet)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        List<Vector2Int> seeds = CreateBackgroundSeeds(minX, maxX, minY, maxY);

        foreach(var seed in seeds)
        {
            Vector2Int p = seed;

            for (int i = 0; i < backgroundWalkLength; i++)
            {
                if (IsInsideBackgroundBox(seed, p))
                    backgroundTilesSet.Add(p);

                p += GetRandomDirection();
            }
        }

        // Remove tiles overlapping floor
        foreach (var f in floorTilesSet)
            backgroundTilesSet.Remove(f);
    }

    private List<Vector2Int> CreateBackgroundSeeds(int minX, int maxX, int minY, int maxY)
    {
        List<Vector2Int> seeds = new List<Vector2Int>();

        Vector2Int center = new Vector2Int(
            (minX + maxX) / 2,
            (minY + maxY) / 2
        );

        for (int i = 0; i < backgroundWalks; i++)
        {
            float t = (i + 1f) / (backgroundWalks + 1f);
            Vector2Int seed = new Vector2Int(
                Mathf.RoundToInt(Mathf.Lerp(minX, maxX, t)),
                center.y + backgroundPadding
            );

            seed += new Vector2Int(0, backgroundPadding);

            seeds.Add(seed);
        }

        return seeds;
    }

    private bool IsInsideBackgroundBox(Vector2Int center, Vector2Int p)
    {
        return Mathf.Abs(p.x - center.x) <= backgroundBoxSize &&
               Mathf.Abs(p.y - center.y) <= backgroundBoxSize;
    }

    private void PaintTiles()
    {
        foreach (var pos in floorTilesSet)
        {
            floorTilemap.SetTile(
                new Vector3Int(pos.x, pos.y, 0),
                floorTileset.GetRandomTile()
            );
        }

        foreach (var pos in backgroundTilesSet)
        {
            backgroundTilemap.SetTile(
                new Vector3Int(pos.x, pos.y, 0),
                backgroundTileset.GetRandomTile()
            );
        }
    }

    private static readonly Vector2Int[] Directions4 =
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    private Vector2Int GetRandomDirection()
    {
        return Directions4[Random.Range(0, 4)];
    }

    public bool IsFloorTile(Vector2Int p)
    {
        return floorTilesSet != null && floorTilesSet.Contains(p);
    }

    public bool IsCorridor(Vector2Int tile, HashSet<Vector2Int> floor)
    {
        int count = 0;
        foreach (var d in Directions4)
        {
            if (floor.Contains(tile + d))
                count++;
        }

        return count <= 2; 
    }

    public HashSet<Vector2Int> GetFloorTilesPositions()
    {
        if (floorTilesSet == null)
        {
            floorTilesSet = new HashSet<Vector2Int>();
        }
        return floorTilesSet;
    }

}
