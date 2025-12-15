using UnityEngine;

/// <summary>
/// 청크 데이터: 블록 배열과 청크 좌표를 관리
/// </summary>
public class ChunkData
{
    public Vector3Int coord;          // 청크 좌표 (월드 기준)
    public int sizeX;                 // 청크 가로 크기
    public int sizeZ;                 // 청크 세로 크기
    public int height;                // 청크 높이
    public BlockId[,,] blocks;        // 블록 데이터

    public ChunkData(Vector3Int coord, int sizeX, int sizeZ, int height)
    {
        this.coord = coord;
        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
        this.height = height;
        blocks = new BlockId[sizeX, height, sizeZ];
    }
}

/// <summary>
/// 블록 종류 정의
/// </summary>
public enum BlockId
{
    Air,
    Grass,
    Dirt,
    Stone,
    Sand,
    Water,

    // 광석 추가
    CoalOre,
    IronOre,
    GoldOre,
    DiamondOre
}