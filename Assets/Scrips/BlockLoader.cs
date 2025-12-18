using UnityEngine;

public class BlockLoader : MonoBehaviour
{
    public Vector3Int coord;      // 블록 좌표
    public WorldManager world;    // 월드 매니저 참조
    void OnDestroy()
    {
        if (world != null)
        {
            //world.LoadNeighborBlocks(coord);
        }
    }
}