using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameJam
{
    public class SlotHelper : MonoBehaviour
    {
        /// <summary>
        /// 获取这个槽中物品的名字（PlacedItem 的名字或 ItemData.itemName）
        /// </summary>
        /// 
        //[SerializeField]public TooltipManager tooltip;
        [Header("子级用于显示物品的image")]
        [SerializeField] public Image image;
        public ItemManager itemManager; // 场景中的ItemManager
        [Header("同上")]
        public TooltipContent tooltipContent; // 直接拖引用

       
        public void ShowName()
        {
            if (image == null || itemManager == null || tooltipContent == null) return;

            var item = itemManager.GetItemBySprite(image.sprite);
            if (item != null)
            {
                tooltipContent.description = item.itemName;
                tooltipContent.ProcessEnter();
            }
            else
            {
                Debug.LogWarning("未找到对应物品");
            }

        }
    }
}
