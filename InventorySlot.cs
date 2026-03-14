using UnityEngine;

namespace GameJam
{
    // 挂在 slotImages[i].gameObject 上（可在运行时动态添加或在编辑器里预先添加）
    public class InventorySlot : MonoBehaviour
    {
        [HideInInspector]public int slotIndex = -1;
        [HideInInspector]public string categoryTag;
    }
}
