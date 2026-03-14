
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace GameJam
{
    public class CountdownTimerWithText : MonoBehaviour
    {
        [Header("UI 显示")]
        public TextMeshProUGUI timerText;

        [Header("倒计时设置")]
        public float duration = 10f;

        private float remainingTime;
        private bool isRunning = false;

        // 新增暂停状态标记
        private bool isPaused = false;

        [Header("倒计时结束事件")]
        public UnityEvent onTimerFinished;

        public GameManager GameManager; // 引用GameManager脚本
        [Header("全局?")]
        public bool allgame;

        [Header("音效")]
        public AudioSource audioSource;

        private int lastSecondPlayed = -1; // 记录上一次播放音效的秒数

        void Update()
        {
            // 如果没在运行或处于暂停状态，直接返回
            if (!isRunning || isPaused) return;

            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isRunning = false;
                UpdateTimerText();

                if (allgame)
                {
                    if (GameManager.panel.activeSelf && GameManager.canvasGroup.alpha > 0)
                    {
                        Debug.Log("面板已经打开，跳过显示逻辑");
                        return;
                    }
                }
                if(ItemManager.Instance.AreAllSceneItemsUncollected())
                {
                    Debug.Log("场景物体全部未收集，触发空事件");
                    GameManager.emptyevent.Invoke();
                }
                else
                {
                    Debug.Log("倒计时结束，触发结束事件");
                    // 倒计时结束，触发事件
                    onTimerFinished?.Invoke();
                }

                    
                return;
            }

            UpdateTimerText();

            // 倒计时剩余10秒或更少，每秒播放一次音效
            if (remainingTime <= 10f)
            {
                int currentSecond = Mathf.CeilToInt(remainingTime);
                if (currentSecond != lastSecondPlayed)
                {
                    lastSecondPlayed = currentSecond;
                    if (audioSource != null)
                        audioSource.Play();
                }
            }
        }

        // 更新显示文本
        void UpdateTimerText()
        {
            // 向上取整显示秒数
            if (timerText != null)
                timerText.text = Mathf.Ceil(remainingTime).ToString();
        }

        /// <summary>
        /// 启动倒计时
        /// </summary>
        /// <param name="seconds">倒计时秒数</param>
        public void StartTimer(float seconds)
        {
            duration = seconds;
            remainingTime = duration;
            isRunning = true;
            isPaused = false; // 启动时确保不是暂停状态
            lastSecondPlayed = -1; // 重置音效计时
            UpdateTimerText();
        }

        /// <summary>
        /// 停止倒计时
        /// </summary>
        public void StopTimer()
        {
            isRunning = false;
            isPaused = false;
        }

        /// <summary>
        /// 暂停倒计时
        /// </summary>
        public void PauseTimer()
        {
            if (isRunning && !isPaused)
            {
                isPaused = true;
                Debug.Log("倒计时已暂停");
            }
        }

        /// <summary>
        /// 继续倒计时
        /// </summary>
        public void ResumeTimer()
        {
            if (isRunning && isPaused)
            {
                isPaused = false;
                Debug.Log("倒计时已继续");
            }
        }

        /// <summary>
        /// 判断计时是否正在运行
        /// </summary>
        public bool IsRunning()
        {
            return isRunning;
        }

        /// <summary>
        /// 判断计时是否处于暂停状态
        /// </summary>
        public bool IsPaused()
        {
            return isPaused;
        }
    }
}

