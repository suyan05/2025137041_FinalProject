using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BlockMaterialEntry
{
    public BlockId blockId;
    public Material material;
}

public class BlockTextureManager : MonoBehaviour
{
    [Header("Block Materials (Inspector 연결)")]
    public List<BlockMaterialEntry> blockMaterials = new List<BlockMaterialEntry>();

    private static Dictionary<BlockId, Material> materials = new Dictionary<BlockId, Material>();
    private static Material defaultMaterial;

    void Awake()
    {
        materials.Clear();

        foreach (var entry in blockMaterials)
        {
            if (!materials.ContainsKey(entry.blockId) && entry.material != null)
            {
                materials[entry.blockId] = entry.material;
            }
        }

        // 기본 머티리얼 (없을 경우 빨간색 표시용)
        defaultMaterial = new Material(Shader.Find("Standard"));
        defaultMaterial.color = Color.magenta;
    }

    public static Material GetMaterial(BlockId id)
    {
        if (materials.ContainsKey(id)) return materials[id];
        return defaultMaterial;
    }
}