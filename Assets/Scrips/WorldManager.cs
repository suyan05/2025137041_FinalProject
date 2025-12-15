using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 월드 전체를 관리: 청크 생성, 데이터 채우기, 메쉬 빌드
/// </summary>
public class WorldManager : MonoBehaviour
{
    public VoxelSettings settings;          // 월드 설정
    public Material chunkMaterial;          // 청크 머티리얼
    public int radiusChunks = 4;            // 플레이어 주변 청크 반경

    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    private WorldSampler sampler;

    void Start()
    {
        sampler = new WorldSampler(settings);
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int cx = -radiusChunks; cx <= radiusChunks; cx++)
        {
            for (int cz = -radiusChunks; cz <= radiusChunks; cz++)
            {
                Vector3Int coord = new Vector3Int(cx, 0, cz);

                ChunkData data = new ChunkData(coord, settings.chunkSizeX, settings.chunkSizeZ, settings.chunkHeight);
                FillChunkData(data);

                GameObject go = new GameObject($"Chunk_{cx}_{cz}");
                go.transform.parent = transform;
                go.transform.position = new Vector3(cx * settings.chunkSizeX, 0, cz * settings.chunkSizeZ);

                Chunk chunk = go.AddComponent<Chunk>();
                chunk.Init(data, settings, sampler);

                chunks[coord] = chunk;
            }
        }

        foreach (var kv in chunks)
        {
            kv.Value.BuildMesh(chunks);
        }
    }

    void FillChunkData(ChunkData data)
    {
        for (int x = 0; x < data.sizeX; x++)
        {
            for (int z = 0; z < data.sizeZ; z++)
            {
                int wx = data.coord.x * data.sizeX + x;
                int wz = data.coord.z * data.sizeZ + z;

                int height = sampler.SampleHeight(wx, wz);
                int soilDepth = sampler.SampleSoilDepth(wx, wz);

                for (int y = 0; y < data.height; y++)
                {
                    int wy = y;

                    if (sampler.IsCave(wx, wy, wz))
                    {
                        data.blocks[x, y, z] = BlockId.Air;
                        continue;
                    }

                    if (wy <= settings.seaLevel && wy > height)
                    {
                        data.blocks[x, y, z] = BlockId.Water;
                        continue;
                    }

                    if (wy == height)
                    {
                        if (sampler.IsSandy(wx, wz, height))
                        {
                            data.blocks[x, y, z] = BlockId.Sand;
                        }
                        else
                        {
                            data.blocks[x, y, z] = BlockId.Grass;
                        }
                    }
                    else if (wy < height && wy >= height - soilDepth)
                    {
                        data.blocks[x, y, z] = BlockId.Dirt;
                    }
                    else if (wy < height - soilDepth)
                    {
                        BlockId ore = sampler.SampleOre(wx, wy, wz);
                        data.blocks[x, y, z] = ore;
                    }
                    else
                    {
                        data.blocks[x, y, z] = BlockId.Air;
                    }
                }
            }
        }
    }
}