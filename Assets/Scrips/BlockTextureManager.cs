using UnityEngine;
using System.Collections.Generic;

public class BlockTextureManager : MonoBehaviour
{
    [Header("Block Materials (Inspector ¿¬°á)")]
    public Material grassMaterial;
    public Material dirtMaterial;
    public Material sandMaterial;
    public Material waterMaterial;

    private static Dictionary<BlockId, Material> materials = new Dictionary<BlockId, Material>();

    void Awake()
    {
        materials[BlockId.Grass] = grassMaterial;
        materials[BlockId.Dirt] = dirtMaterial;
        materials[BlockId.Sand] = sandMaterial;
        materials[BlockId.Water] = waterMaterial;
    }

    public static Material GetMaterial(BlockId id)
    {
        if (materials.ContainsKey(id)) return materials[id];
        return null;
    }
}