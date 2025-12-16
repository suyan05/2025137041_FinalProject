using UnityEngine;

public enum BlockId
{
    Air, Grass, Dirt, Stone, Sand, Water,
    CoalOre, IronOre, GoldOre, DiamondOre
}

public class ChunkData
{
    public Vector3Int coord;
    public int sizeX, sizeZ, height;
    public BlockId[,,] blocks;

    public ChunkData(Vector3Int coord, int sizeX, int sizeZ, int height)
    {
        this.coord = coord;
        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
        this.height = height;
        blocks = new BlockId[sizeX, height, sizeZ];
    }
}