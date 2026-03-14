using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace GameJam
{


    [System.Serializable]
    public class RoomClueAnimators
    {
        [UnityEngine.TooltipAttribute("房间名称，需要和 ChangeRoom 中的 roomDisplayName 完全一致")]
        public string roomName;

        [UnityEngine.TooltipAttribute("该房间对应的所有线索 Animator")]
        public List<Animator> clueAnimators = new List<Animator>();
    }

    public class RandomPlayActiveAnimator : Action
    {
        [UnityEngine.TooltipAttribute("切换房间脚本，用于获取当前房间名字")]
        public ChangeRoom changeRoom;

        [UnityEngine.TooltipAttribute("每个房间对应的线索 Animator 列表")]
        public List<RoomClueAnimators> roomClues = new List<RoomClueAnimators>();

        [UnityEngine.TooltipAttribute("Animator Trigger 参数名")]
        public string triggerName = "Play";

        public override TaskStatus OnUpdate()
        {
            if (changeRoom == null)
            {
                Debug.LogWarning("RandomPlayActiveAnimator：未绑定 ChangeRoom 引用！");
                return TaskStatus.Failure;
            }

            // 获取当前房间名
            string currentRoom = changeRoom.GetCurrentRoomName();
            if (string.IsNullOrEmpty(currentRoom))
            {
                Debug.LogWarning("RandomPlayActiveAnimator：当前房间名为空！");
                return TaskStatus.Failure;
            }

            // 找到当前房间对应的线索列表
            var currentRoomClues = roomClues.Find(r => r.roomName == currentRoom);
            if (currentRoomClues == null || currentRoomClues.clueAnimators.Count == 0)
            {
                Debug.LogWarning($"RandomPlayActiveAnimator：房间 {currentRoom} 没有配置任何线索 Animator！");
                return TaskStatus.Failure;
            }

            // 过滤出当前房间中处于激活状态的线索
            List<Animator> activeClues = new List<Animator>();
            foreach (var animator in currentRoomClues.clueAnimators)
            {
                if (animator != null && animator.gameObject.activeInHierarchy)
                {
                    activeClues.Add(animator);
                }
            }

            if (activeClues.Count == 0)
            {
                Debug.Log($"RandomPlayActiveAnimator：房间 {currentRoom} 中没有激活的线索！");
                return TaskStatus.Failure;
            }

            // 随机选择一个激活的线索并播放动画
            int randomIndex = Random.Range(0, activeClues.Count);
            Animator chosenAnimator = activeClues[randomIndex];

            chosenAnimator.SetTrigger(triggerName);
            Debug.Log($"RandomPlayActiveAnimator：房间 {currentRoom} 播放线索动画 -> {chosenAnimator.name}");

            return TaskStatus.Success;
        }
    }
}