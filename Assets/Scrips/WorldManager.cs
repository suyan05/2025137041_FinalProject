using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [Header("월드 설정")]
    public VoxelSettings settings;
    public int radiusChunks = 2; // 테스트용 작은 반경

    private WorldSampler sampler;
    private Dictionary<Vector3Int, ChunkData> chunkDatas = new Dictionary<Vector3Int, ChunkData>();

    void Start()
    {
        BlockTextureManager.LoadTextures();
        sampler = new WorldSampler(settings);

        // 코루틴으로 청크 단위 박스 생성 시작
        StartCoroutine(BuildChunksStepByStep());
    }

    IEnumerator BuildChunksStepByStep()
    {
        for (int cx = -radiusChunks; cx <= radiusChunks; cx++)
        {
            for (int cz = -radiusChunks; cz <= radiusChunks; cz++)
            {
                Vector3Int coord = new Vector3Int(cx, 0, cz);

                // 청크 데이터 준비
                ChunkData data = new ChunkData(coord, settings.chunkSizeX, settings.chunkSizeZ, settings.chunkHeight);
                FillChunkData(data);
                chunkDatas[coord] = data;

                // 청크 오브젝트 생성
                GameObject chunkObj = new GameObject($"Chunk_{cx}_{cz}");
                chunkObj.transform.parent = transform;
                chunkObj.transform.position = new Vector3(cx * settings.chunkSizeX, 0, cz * settings.chunkSizeZ);

                // 박스 채워 넣기
                BuildChunkAsBoxes(chunkObj, data);

                // 한 청크 생성 후 프레임 넘김 → 점진적 로딩
                yield return null;
            }
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
                        data.blocks[x, y, z] = sampler.SampleOre(wx, wy, wz);
                    }
                    else
                    {
                        data.blocks[x, y, z] = BlockId.Air;
                    }
                }
            }
        }
    }

    void BuildChunkAsBoxes(GameObject chunkObj, ChunkData data)
    {
        for (int x = 0; x < data.sizeX; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                for (int z = 0; z < data.sizeZ; z++)
                {
                    BlockId id = data.blocks[x, y, z];
                    if (id == BlockId.Air) continue;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(
                        data.coord.x * data.sizeX + x,
                        y,
                        data.coord.z * data.sizeZ + z
                    );
                    cube.transform.parent = chunkObj.transform;

                    // 텍스처 적용
                    Texture2D tex = BlockTextureManager.GetTexture(id);
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.mainTexture = tex;
                    cube.GetComponent<Renderer>().material = mat;

                    // BlockLoader 붙여서 파괴 이벤트 처리
                    BlockLoader loader = cube.AddComponent<BlockLoader>();
                    loader.coord = new Vector3Int(x, y, z);
                    loader.world = this;
                }
            }
        }
    }

    public void LoadNeighborBlocks(Vector3Int coord)
    {
        Vector3Int[] dirs = {
            new Vector3Int(1,0,0), new Vector3Int(-1,0,0),
            new Vector3Int(0,1,0), new Vector3Int(0,-1,0),
            new Vector3Int(0,0,1), new Vector3Int(0,0,-1)
        };

        foreach (var d in dirs)
        {
            Vector3Int n = coord + d;

            // 월드 범위 체크 (단일 청크 기준)
            if (n.x >= 0 && n.x < settings.chunkSizeX &&
                n.y >= 0 && n.y < settings.chunkHeight &&
                n.z >= 0 && n.z < settings.chunkSizeZ)
            {

                BlockId id = chunkDatas[Vector3Int.zero].blocks[n.x, n.y, n.z];
                if (id != BlockId.Air)
                {
                    CreateBlock(n.x, n.y, n.z, id);
                }
            }
        }
    }

    void CreateBlock(int x, int y, int z, BlockId id)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(x, y, z);
        cube.transform.parent = this.transform;

        Texture2D tex = BlockTextureManager.GetTexture(id);
        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = tex;
        cube.GetComponent<Renderer>().material = mat;

        BlockLoader loader = cube.AddComponent<BlockLoader>();
        loader.coord = new Vector3Int(x, y, z);
        loader.world = this;
    }
}