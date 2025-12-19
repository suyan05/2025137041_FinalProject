using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [Header("월드 설정")]
    public VoxelSettings settings;
    public GameObject playerPrefab;
    private GameObject playerInstance;

    private WorldSampler sampler;
    private Dictionary<Vector3Int, ChunkData> chunkDatas = new Dictionary<Vector3Int, ChunkData>();
    private Dictionary<Vector3Int, GameObject> loadedChunks = new Dictionary<Vector3Int, GameObject>();

    // 최적화: 프레임당 처리할 블록 개수
    public int blockBatchSize = 10;

    // 오브젝트 풀
    private Queue<GameObject> blockPool = new Queue<GameObject>();
    public int initialPoolSize = 500;

    void Start()
    {
        sampler = new WorldSampler(settings);

        // 초기 풀 생성
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.SetActive(false);
            blockPool.Enqueue(cube);
        }

        StartCoroutine(InitializeWorld());
    }


    IEnumerator InitializeWorld()
    {
        Vector3Int originCoord = new Vector3Int(0, 0, 0);
        PrecomputeChunk(originCoord);
        yield return StartCoroutine(LoadChunkGradually(originCoord));

        Vector3Int[] initialCoords = {
            new Vector3Int(1,0,0), new Vector3Int(-1,0,0),
            new Vector3Int(0,0,1), new Vector3Int(0,0,-1)
        };
        foreach (var coord in initialCoords)
        {
            PrecomputeChunk(coord);
            yield return StartCoroutine(LoadChunkGradually(coord));
        }

        SpawnPlayerAtTop(chunkDatas[originCoord]);
    }

    void Update()
    {
        if (playerInstance == null) return;
        UpdateChunksByDistance();
    }

    IEnumerator LoadChunkGradually(Vector3Int coord)
    {
        if (!chunkDatas.ContainsKey(coord)) yield break;
        if (playerInstance == null) yield break;

        ChunkData data = chunkDatas[coord];
        GameObject chunkObj = new GameObject($"Chunk_{coord.x}_{coord.z}");
        chunkObj.transform.parent = transform;
        loadedChunks[coord] = chunkObj;

        List<GameObject> blocks = new List<GameObject>();

        for (int x = 0; x < data.sizeX; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                for (int z = 0; z < data.sizeZ; z++)
                {
                    BlockId id = data.blocks[x, y, z];
                    if (id == BlockId.Air) continue;

                    Vector3 pos = new Vector3(
                        coord.x * data.sizeX + x,
                        y,
                        coord.z * data.sizeZ + z
                    );

                    GameObject cube = GetBlockFromPool();
                    cube.transform.position = pos;
                    cube.transform.parent = chunkObj.transform;

                    // 태그 자동 지정
                    cube.tag = "Block";

                    // Block 컴포넌트 자동 추가 및 설정
                    Block blockComponent = cube.GetComponent<Block>();
                    if (blockComponent == null)
                    {
                        blockComponent = cube.AddComponent<Block>();
                    }
                    blockComponent.blockId = id;
                    blockComponent.maxHealth = GetBlockHealth(id);

                    // 머티리얼 적용
                    Material mat = BlockTextureManager.GetMaterial(id);
                    if (mat != null) cube.GetComponent<Renderer>().material = mat;

                    cube.SetActive(false);
                    blocks.Add(cube);
                }
            }
        }

        // 가까운 블록부터 순차적으로 활성화
        blocks.Sort((a, b) => {
            float da = Vector3.Distance(playerInstance.transform.position, a.transform.position);
            float db = Vector3.Distance(playerInstance.transform.position, b.transform.position);
            return da.CompareTo(db);
        });

        int count = 0;
        foreach (var block in blocks)
        {
            if (block != null) block.SetActive(true);
            count++;
            if (count % blockBatchSize == 0) yield return null;
        }
    }

    IEnumerator UnloadChunkGradually(Vector3Int coord)
    {
        if (!loadedChunks.ContainsKey(coord)) yield break;
        GameObject chunkObj = loadedChunks[coord];

        List<Transform> blocks = new List<Transform>();
        foreach (Transform child in chunkObj.transform)
        {
            if (child != null) blocks.Add(child);
        }

        blocks.Sort((a, b) => {
            float da = Vector3.Distance(playerInstance.transform.position, a.position);
            float db = Vector3.Distance(playerInstance.transform.position, b.position);
            return db.CompareTo(da);
        });

        int count = 0;
        foreach (var block in blocks)
        {
            if (block != null)
            {
                ReturnBlockToPool(block.gameObject);
            }
            count++;
            if (count % blockBatchSize == 0) yield return null;
        }

        Destroy(chunkObj);
        loadedChunks.Remove(coord);

        // 메모리 관리: chunkDatas에서도 제거
        if (chunkDatas.ContainsKey(coord))
        {
            chunkDatas.Remove(coord);
        }
    }


    void SpawnPlayerAtTop(ChunkData data)
    {
        int maxY = 0;
        for (int x = 0; x < data.sizeX; x++)
        {
            for (int z = 0; z < data.sizeZ; z++)
            {
                for (int y = data.height - 1; y >= 0; y--)
                {
                    if (data.blocks[x, y, z] != BlockId.Air && data.blocks[x, y, z] != BlockId.Water)
                    {
                        if (y > maxY) maxY = y;
                        break;
                    }
                }
            }
        }
        Vector3 spawnPos = new Vector3(settings.chunkSizeX / 2, maxY + 2, settings.chunkSizeZ / 2);
        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        AttachCameraToPlayer(playerInstance);
    }

    void AttachCameraToPlayer(GameObject player)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            float headHeight = 1.6f;
            cam.transform.position = player.transform.position + new Vector3(0, headHeight, 0);
            cam.transform.SetParent(player.transform);

            MouseLook look = cam.GetComponent<MouseLook>();
            if (look != null)
            {
                look.playerBody = player.transform;
            }
        }
    }
    void UpdateChunksByDistance()
    {
        if (playerInstance == null) return;

        Vector3 playerPos = playerInstance.transform.position;
        Vector3 forward = playerInstance.transform.forward;
        Vector3 right = playerInstance.transform.right;

        // 플레이어 현재 청크
        Vector3Int playerChunk = new Vector3Int(
            Mathf.FloorToInt(playerPos.x / settings.chunkSizeX),
            0,
            Mathf.FloorToInt(playerPos.z / settings.chunkSizeZ)
        );

        // 1. 플레이어 주변 2칸 청크 로딩 (기존 1칸 → 확장)
        int around = 2;
        for (int dx = -around; dx <= around; dx++)
        {
            for (int dz = -around; dz <= around; dz++)
            {
                Vector3Int coord = new Vector3Int(playerChunk.x + dx, 0, playerChunk.z + dz);
                if (!chunkDatas.ContainsKey(coord)) PrecomputeChunk(coord);
                if (!loadedChunks.ContainsKey(coord)) StartCoroutine(LoadChunkGradually(coord));
            }
        }

        // 2. 시야 방향 앞으로 3칸 로딩
        for (int i = 1; i <= 3; i++)
        {
            Vector3 targetPos = playerPos + forward.normalized * (i * settings.chunkSizeX);
            Vector3Int coord = new Vector3Int(
                Mathf.FloorToInt(targetPos.x / settings.chunkSizeX),
                0,
                Mathf.FloorToInt(targetPos.z / settings.chunkSizeZ)
            );
            if (!chunkDatas.ContainsKey(coord)) PrecomputeChunk(coord);
            if (!loadedChunks.ContainsKey(coord)) StartCoroutine(LoadChunkGradually(coord));

            // 좌우로 1~3칸 추가 로딩
            for (int offset = 1; offset <= 3; offset++)
            {
                Vector3 leftPos = targetPos - right.normalized * (offset * settings.chunkSizeX);
                Vector3 rightPos = targetPos + right.normalized * (offset * settings.chunkSizeX);

                Vector3Int leftCoord = new Vector3Int(
                    Mathf.FloorToInt(leftPos.x / settings.chunkSizeX), 0,
                    Mathf.FloorToInt(leftPos.z / settings.chunkSizeZ)
                );
                Vector3Int rightCoord = new Vector3Int(
                    Mathf.FloorToInt(rightPos.x / settings.chunkSizeX), 0,
                    Mathf.FloorToInt(rightPos.z / settings.chunkSizeZ)
                );

                if (!chunkDatas.ContainsKey(leftCoord)) PrecomputeChunk(leftCoord);
                if (!loadedChunks.ContainsKey(leftCoord)) StartCoroutine(LoadChunkGradually(leftCoord));

                if (!chunkDatas.ContainsKey(rightCoord)) PrecomputeChunk(rightCoord);
                if (!loadedChunks.ContainsKey(rightCoord)) StartCoroutine(LoadChunkGradually(rightCoord));
            }
        }

        // 3. 멀어진 청크는 점진적으로 제거 (기존 로직 그대로)
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (var kv in loadedChunks)
        {
            Vector3Int coord = kv.Key;
            if (coord == new Vector3Int(0, 0, 0)) continue;

            bool keep = Mathf.Abs(coord.x - playerChunk.x) <= around &&
                        Mathf.Abs(coord.z - playerChunk.z) <= around;

            // 시야 방향 3칸 + 좌우 1~3칸 유지
            if (!keep)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Vector3 targetPos = playerPos + forward.normalized * (i * settings.chunkSizeX);
                    Vector3Int fcoord = new Vector3Int(
                        Mathf.FloorToInt(targetPos.x / settings.chunkSizeX), 0,
                        Mathf.FloorToInt(targetPos.z / settings.chunkSizeZ)
                    );
                    if (coord == fcoord) { keep = true; break; }

                    for (int offset = 1; offset <= 3; offset++)
                    {
                        Vector3 leftPos = targetPos - right.normalized * (offset * settings.chunkSizeX);
                        Vector3 rightPos = targetPos + right.normalized * (offset * settings.chunkSizeX);

                        Vector3Int leftCoord = new Vector3Int(
                            Mathf.FloorToInt(leftPos.x / settings.chunkSizeX), 0,
                            Mathf.FloorToInt(leftPos.z / settings.chunkSizeZ)
                        );
                        Vector3Int rightCoord = new Vector3Int(
                            Mathf.FloorToInt(rightPos.x / settings.chunkSizeX), 0,
                            Mathf.FloorToInt(rightPos.z / settings.chunkSizeZ)
                        );

                        if (coord == leftCoord || coord == rightCoord) { keep = true; break; }
                    }
                }
            }

            if (!keep) toRemove.Add(coord);
        }

        foreach (var coord in toRemove)
        {
            StartCoroutine(UnloadChunkGradually(coord));
        }
    }



    /// <summary>
    /// 청크 데이터만 계산
    /// </summary>
    void PrecomputeChunk(Vector3Int coord)
    {
        ChunkData data = new ChunkData(coord, settings.chunkSizeX, settings.chunkSizeZ, settings.chunkHeight);
        FillGroundOnly(data);
        chunkDatas[coord] = data;
    }


    /// <summary>
    /// 기본 지형 생성 (땅/흙/잔디/모래/물)
    /// </summary>
    void FillGroundOnly(ChunkData data)
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

                    if (wy <= settings.seaLevel && wy > height)
                    {
                        data.blocks[x, y, z] = BlockId.Water;
                        continue;
                    }

                    if (wy == height)
                    {
                        if (sampler.IsSandy(wx, wz, height))
                            data.blocks[x, y, z] = BlockId.Sand;
                        else
                            data.blocks[x, y, z] = BlockId.Grass;
                    }
                    else if (wy < height && wy >= height - soilDepth)
                    {
                        data.blocks[x, y, z] = BlockId.Dirt;
                    }
                    else
                    {
                        data.blocks[x, y, z] = BlockId.Air;
                    }
                }
            }
        }
    }


    void ReturnBlockToPool(GameObject block)
    {
        block.SetActive(false);
        block.transform.SetParent(null);
        blockPool.Enqueue(block);
    }

    GameObject GetBlockFromPool()
    {
        if (blockPool.Count > 0)
        {
            return blockPool.Dequeue();
        }
        else
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.tag = "Block"; // 태그 자동 지정
            cube.SetActive(false);

            // Block 컴포넌트 자동 추가
            Block blockComponent = cube.AddComponent<Block>();
            blockComponent.blockId = BlockId.Air;
            blockComponent.maxHealth = 1;

            return cube;
        }
    }

    int GetBlockHealth(BlockId blockId)
    {
        switch (blockId)
        {
            case BlockId.Stone: return 5;
            case BlockId.Dirt: return 2;
            case BlockId.Sand: return 2;
            case BlockId.Wood: return 3;
            default: return 1;
        }
    }
}