using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class PlayerBlockInteraction : MonoBehaviour
{
    public float interactRange = 5f;
    public Camera playerCamera;
    public HotbarInventory inventory;

    public ToolType currentTool = ToolType.Hand;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (inventory == null)
            inventory = GetComponent<HotbarInventory>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭: 블록 캐기
        {
            TryBreakBlock();
        }

        if (Input.GetMouseButtonDown(1)) // 우클릭: 블록 설치
        {
            TryPlaceBlock();
        }
    }

    void TryBreakBlock()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            Block block = hit.collider.GetComponent<Block>();
            if (block != null)
            {
                int damage = GetToolDamage(currentTool, block.blockId);
                block.TakeDamage(damage, inventory);
            }
        }
    }

    void TryPlaceBlock()
    {
        BlockId selectedBlock = inventory.UseBlock();
        if (selectedBlock == BlockId.Air) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            Vector3 placePos = hit.point + hit.normal * 0.5f;
            placePos = new Vector3(
                Mathf.Round(placePos.x),
                Mathf.Round(placePos.y),
                Mathf.Round(placePos.z)
            );

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = placePos;
            cube.tag = "Block";

            Material mat = BlockTextureManager.GetMaterial(selectedBlock);
            if (mat != null) cube.GetComponent<Renderer>().material = mat;

            Block blockComponent = cube.AddComponent<Block>();
            blockComponent.blockId = selectedBlock;

            // 블록마다 체력 차등 적용
            blockComponent.maxHealth = GetBlockHealth(selectedBlock);
        }
    }

    // 도구별 데미지 계산
    int GetToolDamage(ToolType tool, BlockId blockId)
    {
        switch (tool)
        {
            case ToolType.Pickaxe:
                if (blockId == BlockId.Stone) return 2;
                return 1;
            case ToolType.Shovel:
                if (blockId == BlockId.Dirt || blockId == BlockId.Sand) return 2;
                return 1;
            case ToolType.Axe:
                if (blockId == BlockId.Wood) return 2;
                return 1;
            default: // 맨손
                return 1;
        }
    }

    // 블록별 체력 설정
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

public enum ToolType
{
    Hand,
    Pickaxe,
    Shovel,
    Axe
}