using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Halabang.Plugin
{
    [TaskCategory("Halabang")]
    [TaskDescription("UI RectTransform ±ä»»ÐÐÎª")]
    public class BD_Action_UITransform : Action
    {
        public enum ACTION_TYPE
        {
            None,
            Move,
            Scale,
            Rotation
        }

        public ACTION_TYPE action;
        public RectTransform target;
        public Vector2 targetAnchoredPos;
        public Vector3 targetScale = Vector3.one;
        public Vector3 targetRotation;
        public TweenSetting tweenSetting;

        public override void OnStart()
        {
            if (target == null) return;

            switch (action)
            {
                case ACTION_TYPE.Move:
                    target.DOAnchorPos(targetAnchoredPos, tweenSetting.Duration)
                        .SetDelay(tweenSetting.Delay)
                        .SetLoops(tweenSetting.LoopCycle, tweenSetting.LoopType)
                        .SetEase(tweenSetting.EaseType);
                    break;

                case ACTION_TYPE.Scale:
                    target.DOScale(targetScale, tweenSetting.Duration)
                        .SetDelay(tweenSetting.Delay)
                        .SetLoops(tweenSetting.LoopCycle, tweenSetting.LoopType)
                        .SetEase(tweenSetting.EaseType);
                    break;

                case ACTION_TYPE.Rotation:
                    target.DOLocalRotate(targetRotation, tweenSetting.Duration)
                        .SetDelay(tweenSetting.Delay)
                        .SetLoops(tweenSetting.LoopCycle, tweenSetting.LoopType)
                        .SetEase(tweenSetting.EaseType);
                    break;
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
