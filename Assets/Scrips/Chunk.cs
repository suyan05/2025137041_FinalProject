using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 청크 오브젝트: 데이터와 메쉬를 관리
/// </summary>
public class Chunk : MonoBehaviour
{
    public ChunkData data;
    private VoxelSettings settings;
    private WorldSampler sampler;

    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private static readonly Vector3Int[] dirs = {
        new Vector3Int( 1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int( 0, 1, 0),
        new Vector3Int( 0,-1, 0),
        new Vector3Int( 0, 0, 1),
        new Vector3Int( 0, 0,-1)
    };

    public void Init(ChunkData d, VoxelSettings settings, WorldSampler worldSampler)
    {
        this.data = d;
        this.settings = settings;
        this.sampler = worldSampler;

        if (GetComponent<MeshFilter>() == null) gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();
    }

    public void BuildMesh(Dictionary<Vector3Int, Chunk> neighborChunks)
    {
        verts.Clear(); tris.Clear(); uvs.Clear();

        for (int x = 0; x < data.sizeX; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                for (int z = 0; z < data.sizeZ; z++)
                {
                    BlockId id = data.blocks[x, y, z];
                    if (id == BlockId.Air) continue;

                    Vector3 basePos = new Vector3(x, y, z);

                    for (int d = 0; d < 6; d++)
                    {
                        int nx = x + dirs[d].x;
                        int ny = y + dirs[d].y;
                        int nz = z + dirs[d].z;

                        BlockId neighbor = BlockId.Air;

                        if (nx >= 0 && nx < data.sizeX &&
                            ny >= 0 && ny < data.height &&
                            nz >= 0 && nz < data.sizeZ)
                        {
                            neighbor = data.blocks[nx, ny, nz];
                        }
                        else if (neighborChunks != null)
                        {
                            Vector3Int neighborCoord = data.coord;

                            if (nx < 0) neighborCoord += new Vector3Int(-1, 0, 0);
                            if (nx >= data.sizeX) neighborCoord += new Vector3Int(1, 0, 0);
                            if (nz < 0) neighborCoord += new Vector3Int(0, 0, -1);
                            if (nz >= data.sizeZ) neighborCoord += new Vector3Int(0, 0, 1);

                            if (neighborChunks.TryGetValue(neighborCoord, out Chunk neighborChunk))
                            {
                                int lx = (nx + data.sizeX) % data.sizeX;
                                int lz = (nz + data.sizeZ) % data.sizeZ;
                                if (ny >= 0 && ny < data.height)
                                {
                                    neighbor = neighborChunk.data.blocks[lx, ny, lz];
                                }
                            }
                        }

                        bool addFace = !IsSolid(neighbor);
                        if (id == BlockId.Water)
                        {
                            addFace = neighbor == BlockId.Air || IsSolid(neighbor);
                        }
                        if (!addFace) continue;

                        AddFace(d, basePos, id);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = verts.Count > 65535 ? UnityEngine.Rendering.IndexFormat.UInt32
                                               : UnityEngine.Rendering.IndexFormat.UInt16;
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private bool IsSolid(BlockId id)
    {
        return id != BlockId.Air && id != BlockId.Water;
    }

    private void AddFace(int dir, Vector3 basePos, BlockId id)
    {
        int vCount = verts.Count;
        Vector3[] faceVerts = GetFaceVertices(dir, basePos);
        verts.AddRange(faceVerts);

        tris.Add(vCount);
        tris.Add(vCount + 1);
        tris.Add(vCount + 2);
        tris.Add(vCount);
        tris.Add(vCount + 2);
        tris.Add(vCount + 3);

        // 블록별 텍스처 UV 매핑
        Texture2D tex = BlockTextureManager.GetTexture(id);
        if (tex != null)
        {
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }
    }

    private Vector3[] GetFaceVertices(int dir, Vector3 basePos)
    {
        switch (dir)
        {
            case 0: return new Vector3[] { basePos + new Vector3(1, 0, 0), basePos + new Vector3(1, 0, 1), basePos + new Vector3(1, 1, 1), basePos + new Vector3(1, 1, 0) };
            case 1: return new Vector3[] { basePos + new Vector3(0, 0, 1), basePos + new Vector3(0, 0, 0), basePos + new Vector3(0, 1, 0), basePos + new Vector3(0, 1, 1) };
            case 2: return new Vector3[] { basePos + new Vector3(0, 1, 0), basePos + new Vector3(1, 1, 0), basePos + new Vector3(1, 1, 1), basePos + new Vector3(0, 1, 1) };
            case 3: return new Vector3[] { basePos + new Vector3(0, 0, 1), basePos + new Vector3(1, 0, 1), basePos + new Vector3(1, 0, 0), basePos + new Vector3(0, 0, 0) };
            case 4: return new Vector3[] { basePos + new Vector3(0, 0, 1), basePos + new Vector3(1, 0, 1), basePos + new Vector3(1, 1, 1), basePos + new Vector3(0, 1, 1) };
            case 5: return new Vector3[] { basePos + new Vector3(1, 0, 0), basePos + new Vector3(0, 0, 0), basePos + new Vector3(0, 1, 0), basePos + new Vector3(1, 1, 0) };
            default: return new Vector3[0];
        }
    }
}