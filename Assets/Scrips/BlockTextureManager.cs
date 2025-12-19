using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BlockMaterialEntry
{
    public BlockId blockId;       // 블록 종류
    public Material material;     // 3D 월드용 머티리얼
    public Sprite icon;           // UI용 아이콘 (Inspector 연결)
}

public class BlockTextureManager : MonoBehaviour
{
    [Header("Block Materials & Icons (Inspector 연결)")]
    public List<BlockMaterialEntry> blockMaterials = new List<BlockMaterialEntry>();

    private static Dictionary<BlockId, Material> materials = new Dictionary<BlockId, Material>();
    private static Dictionary<BlockId, Sprite> icons = new Dictionary<BlockId, Sprite>();

    private static Material defaultMaterial;
    private static Sprite defaultIcon;

    void Awake()
    {
        materials.Clear();
        icons.Clear();

        // Inspector에서 연결된 블록-머티리얼-아이콘 매핑을 Dictionary에 등록
        foreach (var entry in blockMaterials)
        {
            if (entry.material != null && !materials.ContainsKey(entry.blockId))
                materials[entry.blockId] = entry.material;

            if (entry.icon != null && !icons.ContainsKey(entry.blockId))
                icons[entry.blockId] = entry.icon;
        }

        // 기본 머티리얼 (등록되지 않은 경우 보라색 표시)
        defaultMaterial = new Material(Shader.Find("Standard"));
        defaultMaterial.color = Color.magenta;

        // 기본 아이콘 (등록되지 않은 경우 null → UI에서 빈칸 처리)
        defaultIcon = null;
    }

    // 블록 ID로 머티리얼 가져오기
    public static Material GetMaterial(BlockId id)
    {
        if (materials.ContainsKey(id)) return materials[id];
        return defaultMaterial;
    }

    // 블록 ID로 아이콘 가져오기
    public static Sprite GetIcon(BlockId id)
    {
        if (icons.ContainsKey(id)) return icons[id];
        return defaultIcon;
    }
}