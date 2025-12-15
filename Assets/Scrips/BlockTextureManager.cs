using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 블록별 텍스처를 관리하는 매니저
/// </summary>
public static class BlockTextureManager
{
    private static Dictionary<BlockId, Texture2D> textures = new Dictionary<BlockId, Texture2D>();

    /// <summary>
    /// 블록 텍스처 로드
    /// </summary>
    public static void LoadTextures()
    {
        textures[BlockId.Grass] = Resources.Load<Texture2D>("Textures/Blocks/Grass");
        textures[BlockId.Dirt] = Resources.Load<Texture2D>("Textures/Blocks/Dirt");
        textures[BlockId.Stone] = Resources.Load<Texture2D>("Textures/Blocks/Stone");
        textures[BlockId.Sand] = Resources.Load<Texture2D>("Textures/Blocks/Sand");
        textures[BlockId.Water] = Resources.Load<Texture2D>("Textures/Blocks/Water");

        textures[BlockId.CoalOre] = Resources.Load<Texture2D>("Textures/Blocks/CoalOre");
        textures[BlockId.IronOre] = Resources.Load<Texture2D>("Textures/Blocks/IronOre");
        textures[BlockId.GoldOre] = Resources.Load<Texture2D>("Textures/Blocks/GoldOre");
        textures[BlockId.DiamondOre] = Resources.Load<Texture2D>("Textures/Blocks/DiamondOre");
    }

    /// <summary>
    /// 블록 텍스처 가져오기
    /// </summary>
    public static Texture2D GetTexture(BlockId id)
    {
        if (textures.ContainsKey(id)) return textures[id];
        return Resources.Load<Texture2D>("Textures/Blocks/Default");
    }
}