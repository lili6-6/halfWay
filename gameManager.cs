using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;

namespace GameJam
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CountdownTimerWithText downTimer;
        [SerializeField] private InventoryManager inventoryManager;
        [Header("错误点击")]
        [SerializeField] private Animator wrongAnimator;
        [SerializeField] private GameObject wrong;
        [SerializeField] private string wrongAnimName = "WrongAnimation";
        [SerializeField]private AudioSource wrongAudioSource;
        [Header("结算面板")]
        [SerializeField] public GameObject panel; // 结束面板
        [HideInInspector] public CanvasGroup canvasGroup;
        [Header("收集槽满")]
        [SerializeField] public UnityEvent fullevent;
        [Header("收集槽空")]
        public UnityEvent emptyevent;
        [Header("已满text")]
        [SerializeField] public TextMeshProUGUI[] isfull=new TextMeshProUGUI[3];
        [Header("分类不足事件")]
        [SerializeField]public UnityEvent notEnoughEvent;
        [Header("分类结束事件")]
        [SerializeField] public UnityEvent classFinish;
        [Header("三个类")]
        public classManager[] classManagers = new classManager[3];

        public static GameManager Instance { get; private set; }

        private bool gameEnded = false;

        private void Awake()
        {
            Instance = this;
            canvasGroup = panel.GetComponent<CanvasGroup>();
           // ResetEndPanel();
        }
        
        

        private void OnEnable()
        {
            // 确保只订阅一次，防止多次累加导致重复触发
            inventoryManager.OnInventoryFull -= HandleInventoryFull;
            inventoryManager.OnInventoryFull += HandleInventoryFull;
            //InventoryManager.Instance.OnInventoryEmpty -= HandleInventoryEmpty;
            //InventoryManager.Instance.OnInventoryEmpty += HandleInventoryEmpty;

            downTimer.onTimerFinished.RemoveListener(HandleTimerFinished);
            downTimer.onTimerFinished.AddListener(HandleTimerFinished);
        }

        private void OnDisable()
        {
            // 取消订阅避免潜在内存泄漏
            inventoryManager.OnInventoryFull -= HandleInventoryFull;
            //inventoryManager.OnInventoryEmpty -= HandleInventoryEmpty;
            downTimer.onTimerFinished.RemoveListener(HandleTimerFinished);
        }

        void Start()
        {
            gameEnded = false;
           // ResetEndPanel();
            inventoryManager.ResetInventory();
            downTimer.StartTimer(downTimer.duration);
        }

        private void HandleInventoryFull()// 处理仓库满的事件
        {
            if (gameEnded) return;

            Debug.Log("HandleInventoryFull 触发");
            gameEnded = true;
            downTimer.StopTimer();
            fullevent?.Invoke();
            // ShowEndPanel();
        }
        void HandleInventoryEmpty()
        {
            if (gameEnded) return;

            Debug.Log("HandleInventoryEmpty 触发");
            gameEnded = true;
            downTimer.StopTimer();
            emptyevent?.Invoke();
        }
        // 检查收集槽是否为空，并触发事件
        public void CheckInventoryEmpty()
        {
            int filledCount = GetFilledCount();

            if (filledCount == 0)
            {
                Debug.Log("收集槽完全为空，触发 emptyevent");
                emptyevent?.Invoke();
            }
           
        }


        private void HandleTimerFinished()
        {
            if (gameEnded) return;

            Debug.Log("HandleTimerFinished 触发");
            gameEnded = true;
           // ShowEndPanel();
            //fullevent?.Invoke();
        }

        //private void ShowEndPanel()
        //{
        //    canvasGroup.alpha = 1;
        //    canvasGroup.interactable = true;
        //    canvasGroup.blocksRaycasts = true;
        //}

        //private void ResetEndPanel()
        //{
        //    canvasGroup.alpha = 0;
        //    canvasGroup.interactable = false;
        //    canvasGroup.blocksRaycasts = false;
        //}

        public void RestartGame()
        {
            Debug.Log("RestartGame 被调用");
            gameEnded = false;
            //ResetEndPanel();
            inventoryManager.ResetInventory();
            downTimer.StartTimer(downTimer.duration);


            foreach(var candrag in InventoryManager.Instance.slotImages)
            {
                if (candrag == null) continue;
                var Iscandrag = candrag.GetComponent<DraggableFromInventory>();
                if (Iscandrag != null)
                {
                    Iscandrag.canDrag = false; // 禁止拖拽物品
                }
            }
            foreach (var img in InventoryManager.Instance.slotImages)
            {
                if (img == null) continue;

                RectTransform rt = img.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = Vector2.zero; // UI 坐标归零
                    rt.localPosition = Vector3.zero;    // 本地位置归零
                }
            }
            foreach (var classManager in classManagers)
            {
                classManager.ResetSlots();
            }
        }

        public void OnWrongClick()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            wrong.transform.position = Input.mousePosition;
            wrong.SetActive(true);
            var cg = wrong.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = false;

            wrongAnimator.Play(wrongAnimName, 0, 0f);
            wrongAudioSource.Play();
            StartCoroutine(HideAfterAnimation(wrongAnimator, wrongAnimName));

        }

        private IEnumerator HideAfterAnimation(Animator anim, string clipName)
        {
            float length = anim.runtimeAnimatorController.animationClips.First(c => c.name == clipName).length;
            yield return new WaitForSeconds(length);
            wrong.SetActive(false);
        }
        
      
        //获取当前已填充的槽数量
        public int GetFilledCount()
        {
            int count = 0;
            //foreach (var img in InventoryManager.Instance.slotImages)
            //{
            //    if (img != null && img.sprite != null)
            //        count++;
            //}
            foreach (var img in InventoryManager.Instance.slotImages)
            {
                if (img == null) continue;

                var imageComp = img.GetComponent<Image>();
                if (imageComp != null && imageComp.enabled)
                {
                    count++;
                }
            }
           // Debug.Log("Class() 被调用，当前已填充收集槽数量: " +count);
            return count;
        }

        //public IEnumerator Class()
        //{
        //    yield return null; // 等待一帧，确保所有UI更新完成
        //    Debug.Log("Class() 被调用，当前已填充收集槽数量: " + GetFilledCount());
        //    if (GetFilledCount() == 0)
        //    {
        //        foreach (var fu in isfull)
        //        {
        //            Debug.Log("isfull 元素: " + fu);
        //            if (fu.gameObject.activeSelf)
        //            {
        //                // 发现至少有一个启用，直接调用
        //                classFinish.Invoke();
        //                Debug.Log("至少有一个类已启用，调用 classFinish 事件");
        //                yield break; // 结束方法
        //            }
        //        }
        //        //foreach (var fu in isfull)
        //        //{
        //        //    if (fu == null)
        //        //    {
        //        //        Debug.Log("isfull 中有 null 元素");
        //        //        continue;
        //        //    }

        //        //    Debug.Log($"检查 {fu.name}，activeSelf={fu.gameObject.activeSelf}");

        //        //    if (fu.gameObject.activeSelf)
        //        //    {
        //        //        classFinish.Invoke();
        //        //        Debug.Log("至少有一个类已启用，调用 classFinish 事件");
        //        //        yield return null;
        //        //    }
        //        //}

        //        // 如果循环走完都没发现启用
        //        notEnoughEvent.Invoke();
        //    }
        //}
        public IEnumerator Class()
        {
            yield return null; // 确保UI状态刷新

            int filledCount = GetFilledCount();
            Debug.Log("Class() 调用，当前已填充数量：" + filledCount);

            // 只有在分类槽完全为空时才进行判断
            if (filledCount == 0)
            {
                bool hasActiveFull = false;
                foreach (var fu in isfull)
                {
                    if (fu != null && fu.gameObject.activeSelf)
                    {
                        hasActiveFull = true;
                        break;
                    }
                }

                if (hasActiveFull)
                {
                    Debug.Log("至少一个分类已启用，触发 classFinish");
                    classFinish.Invoke();
                }
                else
                {
                    Debug.Log("分类不足，触发 notEnoughEvent");
                    notEnoughEvent.Invoke();
                }
            }
        }


    }
}
