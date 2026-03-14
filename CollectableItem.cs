using UnityEngine;

namespace GameJam
{
    // 物品数据脚本ableObject

    [System.Serializable]
    public class CollectableItem : MonoBehaviour
    {
        [Header("类别标签")]
        public string categoryTag; // 标签，比如 "水果"、"蔬菜"
        [Header("对应物品数据")]
        public ItemData itemData;  // 你的物品数据（图标、名称等）

        void OnMouseDown()
        {
            if (itemData != null && !itemData.collected)
            {
                itemData.collected = true;
                Debug.Log($"点击收集物品: {itemData.itemName}");

                // 可选：播放收集动画
                // PlayCollectTween();

                // 可选：隐藏物体
                // gameObject.SetActive(false);

                // 打印状态
                //ItemManager.Instance.PrintAllItemStatus();
            }
        }
    }
}
