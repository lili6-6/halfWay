using UnityEngine;
using DG.Tweening;

namespace GameJam
{
    public class TargetAnimate : MonoBehaviour
    {
        [SerializeField] public GameObject target;
        [SerializeField] public float duration = 1;
        [SerializeField] private bool isItem;

        private Vector3 originalPosition;
        private Vector3 originalScale;
        private float originalAlpha;
        private SpriteRenderer sr;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                originalAlpha = sr.color.a;
            }
            originalPosition = transform.position;
            originalScale = transform.localScale;
        }

        public void StartMove()
        {
            int originalOrder = 0;
            if (sr != null)
            {
                originalOrder = sr.sortingOrder;
                sr.sortingOrder = 20; // 临时置顶
            }

            // 用 Sequence 组合动画
            Sequence seq = DOTween.Sequence();

            // 移动
            seq.Join(transform.DOMove(target.transform.position, duration).SetEase(Ease.Linear));

            // 如果是物品 → 缩放 + 渐隐
            if (isItem && sr != null)
            {
                seq.Join(transform.DOScale(0, duration));
                seq.Join(sr.DOFade(0, duration));
            }

            // 动画播放完毕时执行
            seq.OnComplete(() =>
            {
                // 通知 ItemManager → 标记已收集
                if (isItem && sr != null && ItemManager.Instance != null)
                {
                    ItemManager.Instance.MarkItemCollected(sr.sprite);
                }

                // 恢复原始状态
                transform.position = originalPosition;
                transform.localScale = originalScale;
                if (sr != null)
                {
                    sr.sortingOrder = originalOrder;
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, originalAlpha);
                }

                // 隐藏物体
                gameObject.SetActive(false);
            });
        }
    }
}
