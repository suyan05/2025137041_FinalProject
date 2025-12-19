using UnityEngine;
using System.Collections.Generic;

public class HotbarInventory : MonoBehaviour
{
    [Header("핫바 설정")]
    public int hotbarSize = 9;          // 슬롯 개수 (기본 9칸)
    public int selectedIndex = 0;       // 현재 선택된 칸

    private List<BlockId> slots;        // 슬롯별 블록 저장

    void Start()
    {
        // 시작 시 빈 슬롯으로 초기화
        slots = new List<BlockId>(new BlockId[hotbarSize]);
    }

    void Update()
    {
        HandleInput();
    }

    // 숫자 키(1~9)와 마우스 휠로 슬롯 이동
    void HandleInput()
    {
        // 숫자 키 입력
        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedIndex = i;
            }
        }

        // 마우스 휠 입력
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedIndex = (selectedIndex + 1) % hotbarSize;
        }
        else if (scroll < 0f)
        {
            selectedIndex = (selectedIndex - 1 + hotbarSize) % hotbarSize;
        }
    }

    // 블록 추가 (캐기 시 호출)
    public void AddBlock(BlockId block)
    {
        if (slots[selectedIndex] == BlockId.Air)
        {
            slots[selectedIndex] = block;
        }
        else
        {
            for (int i = 0; i < hotbarSize; i++)
            {
                if (slots[i] == BlockId.Air)
                {
                    slots[i] = block;
                    return;
                }
            }
        }
    }

    // 블록 사용 (설치 시 호출)
    public BlockId UseBlock()
    {
        BlockId block = slots[selectedIndex];
        if (block != BlockId.Air)
        {
            slots[selectedIndex] = BlockId.Air;
            return block;
        }
        return BlockId.Air;
    }

    // 현재 선택된 블록 확인
    public BlockId GetSelectedBlock()
    {
        return slots[selectedIndex];
    }

    // 특정 슬롯의 블록 가져오기 (UI에서 사용)
    public BlockId GetBlockAt(int index)
    {
        if (index >= 0 && index < slots.Count)
            return slots[index];
        return BlockId.Air;
    }
}