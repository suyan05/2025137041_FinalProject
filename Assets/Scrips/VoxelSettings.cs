using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class OreEntry
{
    public BlockId blockId;
    public int maxHeight;
    [Range(0f, 1f)] public float chance;
}

[System.Serializable]
public class VoxelSettings
{
    [Header("청크 Settings")]
    public int chunkSizeX = 16;
    public int chunkSizeZ = 16;
    public int chunkHeight = 128;

    [Header("월드 시드")]
    public int worldSeed = 12345;
    public bool useRandomOffset = false;

    [Header("지형 Settings")]
    public float baseHeight = 50f;
    public float heightNoiseScale = 0.0075f;
    public float heightNoiseAmp = 18f;

    [Header("바이옴 Settings")]
    public float biomeScale = 0.0025f;
    public int soilDepthMin = 3;
    public int soilDepthMax = 6;
    public float sandThreshold = 0.62f;

    [Header("산 Settings")]
    public float mountainNoiseScale = 0.0015f;
    public float mountainNoiseAmp = 100f;
    public float mountainThreshold = 0.65f;

    [Header("강 Settings")]
    public float riverNoiseScale = 0.002f;
    public float riverWidth = 0.08f;
    public float riverDepth = 6f;

    [Header("동굴 Settings")]
    public float caveNoiseScale = 0.035f;
    public float caveThreshold = 0.58f;
    public float caveRidgeScale = 0.012f;

    [Header("광석 Settings")]
    public float oreNoiseScale = 0.05f;
    public List<OreEntry> oreEntries = new List<OreEntry>();

    [Header("수면 Settings")]
    public int seaLevel = 48;

    [Header("노이즈 오프셋")]
    public Vector3 noiseOffset = new Vector3(1000, 1000, 1000);

    public void InitializeOffsets()
    {
        if (useRandomOffset)
        {
            noiseOffset = new Vector3(
                Random.Range(0, 10000),
                Random.Range(0, 10000),
                Random.Range(0, 10000)
            );
        }
    }
}