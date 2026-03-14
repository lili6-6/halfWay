using UnityEngine;
using System.Collections.Generic;



namespace GameJam
{

    public class ItemClickCollector : MonoBehaviour
    {
        public InventoryManager inventoryManager;

        // 这是你外部调用的接口，传入被点击的物品GameObject
        public void TryCollectItem(GameObject clickedObj)
        {
            if (clickedObj == null) return;

            CollectableItem item = clickedObj.GetComponent<CollectableItem>();
            if (item != null)
            {
                bool success = inventoryManager.CollectItem(item.itemData);
                if (success)
                {
                    Debug.Log("收集成功: " + item.itemData.itemName);
                    Destroy(clickedObj);
                }
                else
                {
                    Debug.Log("仓库满了，无法收集");
                }
            }
            else
            {
                Debug.Log("点了非收集物体");
            }
        }
    }
}