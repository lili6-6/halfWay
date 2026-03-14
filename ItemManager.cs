
using GameJam;
using System.Collections.Generic;
using UnityEngine;
namespace GameJam
{
    public class ItemManager : MonoBehaviour
    {
        [System.Serializable]
        public class ItemStatus
        {
            public int id;        // 编号
            public bool collected; // 是否被收集

            public ItemStatus(int id, bool collected)
            {
                this.id = id;
                this.collected = collected;
            }
        }
        [System.Serializable]
        public class SceneItem
        {
            public ItemData data;      // 对应的物品数据
            public GameObject sceneObj; // 场景中的物体
        }

        // 数据表（图鉴）
        public List<ItemData> allItems;

        // 场景里实际的物品
        public List<GameObject> sceneItems;

        private Dictionary<Sprite, ItemData> spriteToItem;
        public static ItemManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null) Instance = this;

            spriteToItem = new Dictionary<Sprite, ItemData>();
            foreach (var item in allItems)
            {
                if (item.icon != null && !spriteToItem.ContainsKey(item.icon))
                {
                    spriteToItem.Add(item.icon, item);
                }
            }
             ResetAllItems();
        }

        // 根据 sprite 返回对应物品数据
        public ItemData GetItemBySprite(Sprite sprite)
        {
            if (sprite == null) return null;
            spriteToItem.TryGetValue(sprite, out var item);
            return item;
        }

        // 外部调用：收集完成（动画播完时触发）
        public void MarkItemCollected(Sprite sprite)
        {
            var itemData = GetItemBySprite(sprite);
            if (itemData != null)
            {
                itemData.collected = true;
                Debug.Log($"物品收集完成: {itemData.name}");
            }

            //PrintAllItemStatus();
        }

        // 生成一个“编号+是否收集”的列表
        public List<ItemStatus> GetAllItemStatus()
        {
            List<ItemStatus> result = new List<ItemStatus>();
            foreach (var item in allItems)
            {
                result.Add(new ItemStatus(item.index, item.collected));
            }
            return result;
        }

        // 重置所有物品为未收集
        public void ResetAllItems()
        {
            foreach (var item in allItems)
            {
                item.collected = false;
            }
            Debug.Log("所有物品已重置为未收集状态");
        }

        // 一次性打印所有物品状态
        public void PrintAllItemStatus()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== All Item Status ===");

            foreach (var item in allItems)
            {
                int collected = item.collected ? 1 : 0;
                sb.AppendLine($"ID: {item.index}, Name: {item.name}, Collected: {collected}");
            }

            Debug.Log(sb.ToString());
        }
        // 检查所有场景物体是否都被关闭
        public bool AreAllSceneItemsUncollected()
        {
            foreach (var obj in sceneItems)
            {
                if (obj == null) continue; // 跳过空引用

                var collectable = obj.GetComponent<CollectableItem>();
                if (collectable != null && collectable.itemData != null)
                {
                    // 只要有一个 collected = true，就返回 false
                    if (collectable.itemData.collected)
                    {
                        return false;
                    }
                }
            }

            return true; // 全部 collected 都为 false
        }




    }
}
