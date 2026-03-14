using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("每个收集槽对应显示物品image")]
        public Image[] slotImages;                 // 你的 UI 槽
        private string[] slotTags;                 // 每个槽独立保存标签
        private int nextEmptyIndex = 0;

        public delegate void InventoryFullHandler();
        public event InventoryFullHandler OnInventoryFull;

        //public delegate void InventoryEmptyHandler();
        //public event InventoryEmptyHandler OnInventoryEmpty;


        private ItemData[] slotItems; // 存每格的 ItemData
        public static InventoryManager Instance { get; private set; }


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // 确保只有一个实例
            }
        }

        void Start()
        {
            slotTags = new string[slotImages.Length];
            ResetInventory();
            slotItems = new ItemData[slotImages.Length];
        }

        public bool IsInventoryFull()
        {
            return nextEmptyIndex >= slotImages.Length;
        }

        // 保持原签名（void），仅在成功收集时把标签写入该槽，并确保 InventorySlot 组件存在
        public void CollectItemByGameObject(GameObject clickedObj)
        {
            CollectableItem item = clickedObj.GetComponent<CollectableItem>();
            if (item == null) return;

            if (IsInventoryFull())
            {
                Debug.Log("仓库满了，无法收集更多");
                return;
            }

            // 存图标
            slotImages[nextEmptyIndex].sprite = item.itemData.icon;
            slotImages[nextEmptyIndex].enabled = true;

            // 存物品数据
            slotItems[nextEmptyIndex] = item.itemData;

           // item.itemData.collected = true;

            // 存标签（每个槽独立）
            slotTags[nextEmptyIndex] = item.categoryTag;
            Debug.Log($"收集物品：{item.itemData.itemName}，标签：{item.categoryTag}");
            // 确保这个槽上有 InventorySlot 组件（记录索引和标签），方便拖拽拾取时识别
            var slotGO = slotImages[nextEmptyIndex].gameObject;
            var invSlot = slotGO.GetComponent<InventorySlot>();
            if (invSlot == null) invSlot = slotGO.AddComponent<InventorySlot>();
            invSlot.slotIndex = nextEmptyIndex;
            invSlot.categoryTag = item.categoryTag;

            // 可选：给槽添加可拖拽组件（如果你希望槽的图片本身可拖）
            // if (slotGO.GetComponent<DraggableFromInventory>() == null)
            //     slotGO.AddComponent<DraggableFromInventory>();

            nextEmptyIndex++;

            var collider = clickedObj.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;

            //Debug.Log($"收集成功：{item.itemData.itemName}，标签：{item.categoryTag}");
            PrintCollectedItemNames();

            if (IsInventoryFull())
            {
                Debug.Log("仓库已满，触发事件");
                OnInventoryFull?.Invoke();
            }
        }

        // 获取槽标签（供外部调用）
        public string GetSlotTag(int index)
        {
            if (index < 0 || index >= slotTags.Length) return null;
            return slotTags[index];
        }

        // 获取槽里图标（供创建放置图标使用）
        public Sprite GetSlotSprite(int index)
        {
            if (index < 0 || index >= slotImages.Length) return null;
            return slotImages[index].sprite;
        }

        // 从指定槽索引移除物品并把后续槽左移（更新 slotTags 与 InventorySlot.slotIndex）
        public bool RemoveItemAtIndex(int index)
        {
            if (index < 0 || index >= nextEmptyIndex)
                return false;

            // 左移 UI sprite 与标签、InventorySlot 信息
            for (int i = index; i < nextEmptyIndex - 1; i++)
            {
                // 把后一个槽的图标赋给当前槽
                slotImages[i].sprite = slotImages[i + 1].sprite;
                slotImages[i].enabled = slotImages[i + 1].enabled;

                // 标签数据左移
                slotTags[i] = slotTags[i + 1];

                // 更新 InventorySlot 组件信息
                var slotGO = slotImages[i].gameObject;
                var invSlot = slotGO.GetComponent<InventorySlot>();
                if (invSlot == null) invSlot = slotGO.AddComponent<InventorySlot>();
                invSlot.slotIndex = i;
                invSlot.categoryTag = slotTags[i];
            }

            /// 最后一格清空图标和标签
            int lastIndex = nextEmptyIndex - 1;
            slotImages[lastIndex].sprite = null;
            slotImages[lastIndex].enabled = false;
            slotTags[lastIndex] = null;

            // 清理最后槽的 InventorySlot 信息
            var lastGO = slotImages[lastIndex].gameObject;
            var lastInvSlot = lastGO.GetComponent<InventorySlot>();
            if (lastInvSlot != null)
            {
                lastInvSlot.slotIndex = -1;
                lastInvSlot.categoryTag = null;
            }

            nextEmptyIndex--;
            //// **新增：检测是否完全清空**
            //if (IsInventoryEmpty())
            //{
            //    Debug.Log("收集槽已清空，触发收集阶段为空事件");
            //    OnInventoryEmpty?.Invoke();
            //}

            return true;
        }

        public void ResetInventory()
        {
            for (int i = 0; i < slotImages.Length; i++)
            {
                slotImages[i].sprite = null;
                slotImages[i].enabled = false;
            }

            slotTags = new string[slotImages.Length];
            nextEmptyIndex = 0;

            // 清 InventorySlot 组件数据（如果存在）
            for (int i = 0; i < slotImages.Length; i++)
            {
                var inv = slotImages[i].gameObject.GetComponent<InventorySlot>();
                if (inv != null)
                {
                    inv.slotIndex = -1;
                    inv.categoryTag = null;
                }
            }
        }
        public bool CollectItem(ItemData newItem)
        {
            if (IsInventoryFull()) return false;

            slotImages[nextEmptyIndex].sprite = newItem.icon;
            slotImages[nextEmptyIndex].enabled = true;
            nextEmptyIndex++;

            if (IsInventoryFull())
            {
                Debug.Log("仓库已满，触发事件");
                OnInventoryFull?.Invoke();
            }

            return true;
        }
        public void ClearSlot(int index)
        {
            if (index < 0 || index >= nextEmptyIndex) return;

            slotImages[index].sprite = null;
            slotImages[index].enabled = false;
            slotTags[index] = null;

            // 这里不做左移，保留空槽，方便用户直观看到哪个槽被清空了
        }
        public void EnableDraggingOnCollectedItems()
        {
            // 这里调用 InventoryManagerHelper 来启用拖拽，示例
            Debug.Log("启用拖拽功能");
            for (int i = 0; i < InventoryManagerHelper.Instance.slotImages.Length; i++)
            {
                var slotGO = InventoryManagerHelper.Instance.slotImages[i].gameObject;
                var draggable = slotGO.GetComponent<DraggableFromInventory>();
                if (draggable != null)
                    draggable.SetDraggable(true);
            }
        }
        /// <summary>
        /// 检测收集槽是否全部为空
        /// </summary>
        //public bool IsInventoryEmpty()
        //{
        //    // 如果 nextEmptyIndex 为 0，直接判定为空
        //    if (nextEmptyIndex <= 0) return true;

        //    // 逐个检查是否有任意槽位存放物品
        //    for (int i = 0; i < slotImages.Length; i++)
        //    {
        //        if (slotImages[i].enabled && slotImages[i].sprite != null)
        //        {
        //            return false; // 只要有一格不为空，就返回 false
        //        }
        //    }

        //    return true; // 全部为空
        //}

        public void PrintCollectedItemNames()
        {
            string allNames = "当前已收集的物品：";

            for (int i = 0; i < slotImages.Length; i++)
            {
                string nameToPrint = "（空）";

                // 优先用 slotItems 里的 ItemData
                if (slotItems != null && i < slotItems.Length && slotItems[i] != null)
                {
                    nameToPrint = slotItems[i].itemName;
                }
                // 如果 slotItems 为空或该格没数据，用 Image.sprite 去 ItemManager 查
                else if (slotImages[i] != null && slotImages[i].sprite != null)
                {
                    var itemData = ItemManager.Instance.GetItemBySprite(slotImages[i].sprite);
                    if (itemData != null)
                        nameToPrint = itemData.itemName;
                    else
                        nameToPrint = "（未知物品）";
                }

                allNames += $" [{i}: {nameToPrint}]";
            }

            Debug.Log(allNames);
        }








    }
}
