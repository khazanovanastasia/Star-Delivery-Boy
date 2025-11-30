using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Level Generation/TileSet Data")]
public class TileSetData : ScriptableObject
{
    [System.Serializable]
    public class TileInfo
    {
        public TileBase tile;
        [Range(0f, 1f)]
        public float probability = 1f;
    }

    public List<TileInfo> tiles = new List<TileInfo>();

    public TileBase GetRandomTile()
    {
        if (tiles == null || tiles.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (var info in tiles)
        {
            totalWeight += info.probability;
        }

        float random = Random.value * totalWeight;
        float current = 0f;

        foreach (var info in tiles)
        {
            current += info.probability;
            if (random <= current)
                return info.tile;
        }

        return tiles[0].tile;
    }
}
