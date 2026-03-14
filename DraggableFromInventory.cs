using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameJam
{
   
    
        public class DraggableFromInventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        {
            private Vector3 originalPosition;
            private Transform originalParent;
            private Vector3 originalScale; // 记录原始缩放
            private int originalSlotIndex = -1;
            private CanvasGroup canvasGroup;
            private Canvas rootCanvas;
            private Image selfImage;

            [Header("不可,继续游戏后自动开启")]
            public bool canDrag = false;
        public static DraggableFromInventory Instance { get; private set; }

        public void SetDraggable(bool enable)
            {
                canDrag = enable;
            }

            private void Awake()
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
                selfImage = GetComponent<Image>();
                rootCanvas = GetComponentInParent<Canvas>();
            }

            public void OnBeginDrag(PointerEventData eventData)
            {
                if (!canDrag) return;

                // 记录原父物体、位置和缩放
                originalPosition = transform.position;
                originalParent = transform.parent;
                originalScale = transform.localScale;

                var invSlot = GetComponentInParent<InventorySlot>();
                if (invSlot != null)
                {
                    originalSlotIndex = invSlot.slotIndex;
                }
                else
                {
                    for (int i = 0; i < InventoryManagerHelper.Instance.slotImages.Length; i++)
                    {
                        if (InventoryManagerHelper.Instance.slotImages[i].gameObject == gameObject)
                        {
                            originalSlotIndex = i;
                            break;
                        }
                    }
                }

                canvasGroup.blocksRaycasts = false;
                //if (rootCanvas != null) transform.SetParent(rootCanvas.transform, true);
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (!canDrag) return;
                transform.position = eventData.position;
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                if (!canDrag) return;
                canvasGroup.blocksRaycasts = true;

                CategorySlot targetSlot = null;
                if (eventData.pointerEnter != null)
                {
                    targetSlot = eventData.pointerEnter.GetComponentInParent<CategorySlot>();
                Debug.Log("拖拽结束，目标槽：" + (targetSlot != null ? targetSlot.name : "无"));
            }

                if (targetSlot == null)
                {
                    var results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(eventData, results);
                    foreach (var r in results)
                    {
                        var cs = r.gameObject.GetComponentInParent<CategorySlot>();
                        if (cs != null)
                        {
                            targetSlot = cs;
                            break;
                        }
                    }
                }

                string originalTag = InventoryManagerHelper.Instance.GetSlotTag(originalSlotIndex);

                if (targetSlot != null && originalTag == targetSlot.categoryTag)
                {
                    // 检查槽是否已有物品
                    if (targetSlot.transform.childCount > 0)
                    {
                        // 已有物品，不放置，直接返回
                        ReturnToOriginal();
                        SetSizeToSlot(originalSlotIndex);
                        return;
                    }

                    // 槽是空的，可以放置
                    Sprite draggedSprite = InventoryManagerHelper.Instance.GetSlotSprite(originalSlotIndex);

                    GameObject placedItemGO = new GameObject("PlacedItem");
                    placedItemGO.transform.SetParent(targetSlot.transform, false);

                    Image placedImage = placedItemGO.AddComponent<Image>();
                    placedImage.sprite = draggedSprite;

                //RectTransform rt = placedItemGO.GetComponent<RectTransform>();
                //rt.sizeDelta = ((RectTransform)targetSlot.transform).sizeDelta;

                //InventoryManagerHelper.Instance.ClearSlot(originalSlotIndex);
                //Destroy(gameObject);
                // ✅ 获取RectTransform并调整
                RectTransform rt = placedItemGO.GetComponent<RectTransform>();
                RectTransform slotRT = targetSlot.GetComponent<RectTransform>();

                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero; // 居中

                // 大小缩放到槽的一半
                rt.sizeDelta = slotRT.sizeDelta * 0.5f;
                placedItemGO.transform.localScale = Vector3.one; // UI缩放统一用sizeDelta控制

                // 清除原来的格子
                InventoryManagerHelper.Instance.ClearSlot(originalSlotIndex);

                // 销毁拖拽物体
                //Destroy(gameObject);
                StartCoroutine(GameManager.Instance.Class());
            }
                else
                {
                    // 拖错，返回收集槽
                    ReturnToOriginal();
                    SetSizeToSlot(originalSlotIndex);
                }
            Debug.Log((targetSlot != null ? targetSlot.categoryTag : "无目标槽") + "---" + originalTag);
        }

            private void ReturnToOriginal()
            {
                transform.SetParent(originalParent, false); // 保持局部坐标
                transform.localPosition = originalParent.InverseTransformPoint(originalPosition);
                transform.localScale = originalScale; // 恢复原始缩放
            }

            private void SetSizeToSlot(int slotIndex)
            {
                if (slotIndex < 0) return;

                var slotImage = InventoryManagerHelper.Instance.slotImages[slotIndex];
                if (slotImage != null)
                {
                    RectTransform slotRect = slotImage.GetComponent<RectTransform>();
                    RectTransform selfRect = GetComponent<RectTransform>();

                    if (slotRect != null && selfRect != null)
                    {
                        selfRect.sizeDelta = slotRect.sizeDelta;
                    }
                }
            }
        

        public class InventoryManagerHelper
        {
            private static InventoryManager _instance;
            public static InventoryManager Instance
            {
                get
                {
                    if (_instance == null) _instance = GameObject.FindObjectOfType<InventoryManager>();
                    return _instance;
                }
            }

            public static Image[] slotImages => Instance.slotImages;
            public static string GetSlotTag(int idx) => Instance.GetSlotTag(idx);
            public static Sprite GetSlotSprite(int idx) => Instance.GetSlotSprite(idx);
            public static bool RemoveItemAtIndex(int idx) => Instance.RemoveItemAtIndex(idx);
            public static void ClearSlot(int idx) => Instance.ClearSlot(idx);
        }

        // 可选管理类
        public class InventoryControl : MonoBehaviour
        {
            public void EnableDraggingOnCollectedItems()
            {
                for (int i = 0; i < InventoryManagerHelper.Instance.slotImages.Length; i++)
                {
                    var slotGO = InventoryManagerHelper.Instance.slotImages[i].gameObject;
                    var draggable = slotGO.GetComponent<DraggableFromInventory>();
                    if (draggable != null)
                    {
                        draggable.SetDraggable(true);
                    }
                }
            }

            public void DisableDraggingOnCollectedItems()
            {
                for (int i = 0; i < InventoryManagerHelper.Instance.slotImages.Length; i++)
                {
                    var slotGO = InventoryManagerHelper.Instance.slotImages[i].gameObject;
                    var draggable = slotGO.GetComponent<DraggableFromInventory>();
                    if (draggable != null)
                    {
                        draggable.SetDraggable(false);
                    }
                }
            }
        }
    }



    public class InventoryManagerHelper
    {
        private static InventoryManager _instance;
        public static InventoryManager Instance
        {
            get
            {
                if (_instance == null) _instance = GameObject.FindObjectOfType<InventoryManager>();
                return _instance;
            }
        }

        public static Image[] slotImages => Instance.slotImages;
        public static string GetSlotTag(int idx) => Instance.GetSlotTag(idx);
        public static Sprite GetSlotSprite(int idx) => Instance.GetSlotSprite(idx);
        public static bool RemoveItemAtIndex(int idx) => Instance.RemoveItemAtIndex(idx);
        public static void ClearSlot(int idx) => Instance.ClearSlot(idx);
    }

    // 新增管理类方法 示例调用
    public class InventoryControl : MonoBehaviour
    {
        public void EnableDraggingOnCollectedItems()
        {
            for (int i = 0; i < InventoryManagerHelper.Instance.slotImages.Length; i++)
            {
                var slotGO = InventoryManagerHelper.Instance.slotImages[i].gameObject;
                var draggable = slotGO.GetComponent<DraggableFromInventory>();
                if (draggable != null)
                {
                    draggable.SetDraggable(true);
                }
            }
        }

        public void DisableDraggingOnCollectedItems()
        {
            for (int i = 0; i < InventoryManagerHelper.Instance.slotImages.Length; i++)
            {
                var slotGO = InventoryManagerHelper.Instance.slotImages[i].gameObject;
                var draggable = slotGO.GetComponent<DraggableFromInventory>();
                if (draggable != null)
                {
                    draggable.SetDraggable(false);
                }
            }
        }
    }
}
