using GameJam;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameJam
{
    public class classManager : MonoBehaviour
    {
        [Header("此类所需物品数")]
        public int requiredCount = 1;  // 需要的物品数
        [Header("已满")]
        public TextMeshProUGUI fullText;
        public static classManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            fullText.gameObject.SetActive(false); // 初始状态下隐藏已满文本
        }


        private void Update()
        {
            // 每帧检查是否已满
            if (IsComplete())
            {
                // 如果已满，执行相关逻辑（例如禁用交互、播放动画等）
                fullText.gameObject.SetActive(true);
               // Debug.Log("CategorySlot 已满！");
            }
        }
        
        // 统计所有子槽下名为PlacedItem的物品总数
        public int GetPlacedItemCount()
        {
            int count = 0;
            // 遍历所有第一层子物体（也就是 Slot1、Slot2、Slot3、Slot4）
            foreach (Transform slot in transform)
            {
                // 找到这个槽下面的 Image 子物体（假设只有一个）
                Transform image = slot.Find("Image");
                if (image == null) continue;

                // 统计这个 Image 下名为 PlacedItem 的子物体数量
                foreach (Transform child in image)
                {
                    if (child.name == "PlacedItem")
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        // 判断是否已满
        public bool IsComplete()
        {
            return GetPlacedItemCount() >= requiredCount;
        }
        // 重置状态（例如清空所有子槽）
        public void ResetSlots()
        {
            // 遍历所有第一层子物体（也就是 Slot1、Slot2、Slot3、Slot4）
            foreach (Transform slot in transform)
            {
                // 找到这个槽下面的 Image 子物体（假设只有一个）
                Transform image = slot.Find("Image");
                if (image == null) continue;
                // 清空这个 Image 下的所有子物体
                foreach (Transform child in image)
                {
                    Destroy(child.gameObject);
                }
            }
            fullText.gameObject.SetActive(false); // 重置已满状态文本
        }
    }

}
