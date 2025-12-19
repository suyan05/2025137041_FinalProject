using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public HotbarInventory inventory;   // 인벤토리 참조
    public Image[] slotImages;          // 슬롯 UI 이미지 배열
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            BlockId block = inventory.GetBlockAt(i);

            // 슬롯 색상 업데이트
            if (i == inventory.selectedIndex)
                slotImages[i].color = selectedColor;
            else
                slotImages[i].color = normalColor;

            // 슬롯 아이콘 업데이트 (머티리얼 → 스프라이트 변환 필요)
            if (block != BlockId.Air)
            {
                // 예시: BlockTextureManager에서 아이콘 스프라이트 가져오기
                Sprite icon = BlockTextureManager.GetIcon(block);
                slotImages[i].sprite = icon;
                slotImages[i].enabled = true;
            }
            else
            {
                slotImages[i].enabled = false;
            }
        }
    }
}