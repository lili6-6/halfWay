using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class CategorySlot : MonoBehaviour
    {
        [Header("分类槽标签")]
        public string categoryTag;      // 分类槽标签
       // [Header("需要数")]
       // public int requiredCount = 1;   // 需要的物品数量
                                        // public CategorySlot rootCategorySlot;

        // 统计当前分类槽下放置的物品数量
        public int GetPlacedItemCount()
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                Image img = child.GetComponent<Image>();
                if (img != null && img.sprite != null)
                {
                    count++;
                }
            }
            return count;
        }

        //public bool IsComplete()
        //{
        //    return GetPlacedItemCount() >= requiredCount;
        //}
    }
}
