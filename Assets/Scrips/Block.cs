using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockId blockId = BlockId.Air;
    public int maxHealth = 3;   // 블록 기본 체력
    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // 블록이 데미지를 받음
    public void TakeDamage(int amount, HotbarInventory inventory)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            BreakBlock(inventory);
        }
    }

    void BreakBlock(HotbarInventory inventory)
    {
        if (inventory != null)
        {
            inventory.AddBlock(blockId);
        }
        Destroy(gameObject);
    }
}

public enum BlockId
{
    Air, Grass, Dirt, Stone, Sand, Water,
    CoalOre, IronOre, GoldOre, DiamondOre, Wood
}