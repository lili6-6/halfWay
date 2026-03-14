
//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;
//using Michsky.MUIP;
//using TMPro;
//using UnityEngine.Events;

//namespace GameJam
//{
//    /// <summary>
//    /// 房间信息结构，用于存储房间对象、显示名称和进入事件
//    /// </summary>
//    [System.Serializable]
//    public class RoomInfo
//    {
//        [Header("房间基本信息")]
//        public GameObject roomObject;            // 房间对应的物体
//        public string roomDisplayName;           // 房间显示的名称

//        [Header("进入事件")]
//        public UnityEvent firstEnterScene;       // ✅ 第一次进入触发
//        public UnityEvent everyEnterScene;       // ✅ 每次进入都会触发

//        [HideInInspector]
//        public bool hasEntered = false;          // 是否已经进入过该房间
//    }

//    /// <summary>
//    /// 房间切换脚本，允许玩家在不同房间之间移动
//    /// </summary>
//    public class ChangeRoom : MonoBehaviour
//    {
//        [Header("主摄像头")]
//        [SerializeField] private Camera mainCamera;

//        [Header("按序拖入并取名")]
//        [SerializeField] private List<RoomInfo> rooms = new List<RoomInfo>();

//        [Header("向左向右")]
//        [SerializeField] private ButtonManager leftButton;
//        [SerializeField] private ButtonManager rightButton;

//        [Header("房间名Text")]
//        [SerializeField] private TextMeshProUGUI RoomName;

//        private GameObject currentRoom;

//        void Start()
//        {
//            currentRoom = GetClosestRoom(mainCamera.transform.position);
//            TriggerRoomEvents(currentRoom);   // 初始房间触发事件
//            UpdateRoomName();
//            UpdateButtonStates();
//        }

//        /// <summary>
//        /// 更新左右按钮可点击状态
//        /// </summary>
//        void UpdateButtonStates()
//        {
//            leftButton.Interactable(GetAdjacentRoom(Vector3.left) != null);
//            rightButton.Interactable(GetAdjacentRoom(Vector3.right) != null);
//        }

//        /// <summary>
//        /// 向左移动
//        /// </summary>
//        public void LeftMove()
//        {
//            GameObject leftRoom = GetAdjacentRoom(Vector3.left);
//            if (leftRoom != null)
//            {
//                currentRoom = leftRoom;
//                MoveCameraToRoom(leftRoom);
//                TriggerRoomEvents(leftRoom);  // 触发事件
//                UpdateRoomName();
//            }
//        }

//        /// <summary>
//        /// 向右移动
//        /// </summary>
//        public void RightMove()
//        {
//            GameObject rightRoom = GetAdjacentRoom(Vector3.right);
//            if (rightRoom != null)
//            {
//                currentRoom = rightRoom;
//                MoveCameraToRoom(rightRoom);
//                TriggerRoomEvents(rightRoom); // 触发事件
//                UpdateRoomName();
//            }
//        }

//        /// <summary>
//        /// 平滑移动摄像机到目标房间
//        /// </summary>
//        private void MoveCameraToRoom(GameObject targetRoom)
//        {
//            mainCamera.transform.DOMoveX(targetRoom.transform.position.x, 1f)
//                .SetEase(Ease.InOutQuad)
//                .OnComplete(UpdateButtonStates);
//        }

//        /// <summary>
//        /// 获取离当前位置最近的房间
//        /// </summary>
//        private GameObject GetClosestRoom(Vector3 position)
//        {
//            GameObject closest = null;
//            float minDist = float.MaxValue;

//            foreach (var info in rooms)
//            {
//                if (info.roomObject == null) continue;
//                float dist = Vector3.Distance(position, info.roomObject.transform.position);
//                if (dist < minDist)
//                {
//                    minDist = dist;
//                    closest = info.roomObject;
//                }
//            }
//            return closest;
//        }

//        /// <summary>
//        /// 获取指定方向上的相邻房间
//        /// </summary>
//        private GameObject GetAdjacentRoom(Vector3 direction)
//        {
//            GameObject closest = null;
//            float minDist = float.MaxValue;

//            foreach (var info in rooms)
//            {
//                if (info.roomObject == null || info.roomObject == currentRoom) continue;

//                Vector3 dirToRoom = (info.roomObject.transform.position - currentRoom.transform.position).normalized;
//                if (Vector3.Dot(dirToRoom, direction) > 0.5f) // 方向判断
//                {
//                    float dist = Vector3.Distance(currentRoom.transform.position, info.roomObject.transform.position);
//                    if (dist < minDist)
//                    {
//                        minDist = dist;
//                        closest = info.roomObject;
//                    }
//                }
//            }
//            return closest;
//        }

//        /// <summary>
//        /// 触发房间事件
//        /// </summary>
//        private void TriggerRoomEvents(GameObject room)
//        {
//            var info = rooms.Find(r => r.roomObject == room);
//            if (info != null)
//            {
//                // 每次进入都触发
//                info.everyEnterScene?.Invoke();

//                // 只在第一次进入时触发
//                if (!info.hasEntered)
//                {
//                    info.hasEntered = true;
//                    info.firstEnterScene?.Invoke();
//                }
//            }
//        }

//        /// <summary>
//        /// 获取房间名称
//        /// </summary>
//        private string GetRoomName(GameObject room)
//        {
//            var info = rooms.Find(r => r.roomObject == room);
//            return info != null ? info.roomDisplayName : "未知房间";
//        }

//        /// <summary>
//        /// 更新房间名显示
//        /// </summary>
//        private void UpdateRoomName()
//        {
//            RoomName.text = GetRoomName(currentRoom);
//        }

//        /// <summary>
//        /// 返回当前房间的显示名称
//        /// </summary>
//        public string GetCurrentRoomName()
//        {
//            return GetRoomName(currentRoom);
//        }

//        /// <summary>
//        /// 重置所有房间的第一次进入状态
//        /// </summary>
//        public void ResetFirstEnterFlags()
//        {
//            foreach (var room in rooms)
//            {
//                room.hasEntered = false;
//            }
//        }

//        /// <summary>
//        /// 跳转到指定序号的房间（不会影响其他功能）
//        /// </summary>
//        /// <param name="roomIndex">房间在列表中的索引</param>
//        /// <param name="smooth">是否平滑移动（true=平滑，false=瞬移）</param>
//        public void GoToRoomByIndex(int roomIndex, bool smooth = true)
//        {
//            // 1. 检查列表
//            if (rooms == null || rooms.Count == 0)
//            {
//                Debug.LogError("房间列表为空，无法跳转！");
//                return;
//            }

//            if (roomIndex < 0 || roomIndex >= rooms.Count)
//            {
//                Debug.LogError($"房间索引 {roomIndex} 超出范围 (0 ~ {rooms.Count - 1})");
//                return;
//            }

//            // 2. 获取目标房间
//            GameObject targetRoom = rooms[roomIndex].roomObject;
//            if (targetRoom == null)
//            {
//                Debug.LogError($"房间索引 {roomIndex} 对应的对象为空！");
//                return;
//            }

//            currentRoom = targetRoom;

//            // 3. 平滑移动或瞬移
//            if (smooth)
//            {
//                mainCamera.transform.DOMoveX(targetRoom.transform.position.x, 1f)
//                    .SetEase(Ease.InOutQuad)
//                    .OnComplete(() =>
//                    {
//                        TriggerRoomEvents(targetRoom);
//                        UpdateRoomName();
//                        UpdateButtonStates();
//                    });
//            }
//            else
//            {
//                mainCamera.transform.position = new Vector3(targetRoom.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
//                TriggerRoomEvents(targetRoom);
//                UpdateRoomName();
//                UpdateButtonStates();
//            }
//        }

//        /// <summary>
//        /// 回到初始房间（第一个房间）
//        /// </summary>
//        /// <param name="smooth">是否平滑移动</param>
//        public void GoToFirstRoom(bool smooth = true)
//        {
//            GoToRoomByIndex(0, smooth);
//        }
//    }
//}
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Michsky.MUIP;
using TMPro;
using UnityEngine.Events;
using System.Collections;

namespace GameJam
{
    /// <summary>
    /// 房间信息结构，用于存储房间对象、显示名称和进入事件
    /// </summary>
    [System.Serializable]
    public class RoomInfo
    {
        [Header("房间基本信息")]
        public GameObject roomObject;            // 房间对应的物体
        public string roomDisplayName;           // 房间显示的名称

        [Header("进入事件")]
        public UnityEvent firstEnterScene;       // ✅ 第一次进入触发
        public UnityEvent everyEnterScene;       // ✅ 每次进入都会触发

        [HideInInspector]
        public bool hasEntered = false;          // 是否已经进入过该房间
    }

    /// <summary>
    /// 房间切换脚本，允许玩家在不同房间之间移动
    /// </summary>
    public class ChangeRoom : MonoBehaviour
    {
        [Header("主摄像头")]
        [SerializeField] private Camera mainCamera;

        [Header("按序拖入并取名")]
        [SerializeField] private List<RoomInfo> rooms = new List<RoomInfo>();

        [Header("向左向右")]
        [SerializeField] private ButtonManager leftButton;
        [SerializeField] private ButtonManager rightButton;

        [Header("房间名Text")]
        [SerializeField] private TextMeshProUGUI RoomName;

        [Header("瞬移延迟设置")]
        [Tooltip("调用后延迟多久才瞬移镜头")]
        [SerializeField] private float teleportDelay = 0.5f;

        private GameObject currentRoom;

        void Start()
        {
            currentRoom = GetClosestRoom(mainCamera.transform.position);
            TriggerRoomEvents(currentRoom);   // 初始房间触发事件
            UpdateRoomName();
            UpdateButtonStates();
            UpdateRoomActiveState();
        }

        /// <summary>
        /// 更新左右按钮可点击状态
        /// </summary>
        void UpdateButtonStates()
        {
            leftButton.Interactable(GetAdjacentRoom(Vector3.left) != null);
            rightButton.Interactable(GetAdjacentRoom(Vector3.right) != null);
        }

        /// <summary>
        /// 向左移动
        /// </summary>
        public void LeftMove()
        {
            GameObject leftRoom = GetAdjacentRoom(Vector3.left);
            if (leftRoom != null)
            {
                StartCoroutine(TeleportAfterDelay(leftRoom));
            }
        }

        /// <summary>
        /// 向右移动
        /// </summary>
        public void RightMove()
        {
            GameObject rightRoom = GetAdjacentRoom(Vector3.right);
            if (rightRoom != null)
            {
                StartCoroutine(TeleportAfterDelay(rightRoom));
            }
        }

        /// <summary>
        /// 延迟后瞬移摄像机到目标房间
        /// </summary>
        private IEnumerator TeleportAfterDelay(GameObject targetRoom)
        {
            // 触发房间事件
            TriggerRoomEvents(targetRoom);
            yield return new WaitForSeconds(teleportDelay);

            // 镜头瞬移
            mainCamera.transform.position = new Vector3(
                targetRoom.transform.position.x,
                mainCamera.transform.position.y,
                mainCamera.transform.position.z
            );

            // 更新当前房间
            currentRoom = targetRoom;

            

            // 更新房间名
            UpdateRoomName();

            // 更新按钮状态
            UpdateButtonStates();

            // 控制房间显隐
            UpdateRoomActiveState();
        }

        /// <summary>
        /// 控制房间显隐（当前房间开启，其他关闭）
        /// </summary>
        private void UpdateRoomActiveState()
        {
            foreach (var info in rooms)
            {
                if (info.roomObject != null)
                {
                    info.roomObject.SetActive(info.roomObject == currentRoom);
                }
            }
        }

        /// <summary>
        /// 获取离当前位置最近的房间
        /// </summary>
        private GameObject GetClosestRoom(Vector3 position)
        {
            GameObject closest = null;
            float minDist = float.MaxValue;

            foreach (var info in rooms)
            {
                if (info.roomObject == null) continue;
                float dist = Vector3.Distance(position, info.roomObject.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = info.roomObject;
                }
            }
            return closest;
        }

        /// <summary>
        /// 获取指定方向上的相邻房间
        /// </summary>
        private GameObject GetAdjacentRoom(Vector3 direction)
        {
            GameObject closest = null;
            float minDist = float.MaxValue;

            foreach (var info in rooms)
            {
                if (info.roomObject == null || info.roomObject == currentRoom) continue;

                Vector3 dirToRoom = (info.roomObject.transform.position - currentRoom.transform.position).normalized;
                if (Vector3.Dot(dirToRoom, direction) > 0.5f) // 方向判断
                {
                    float dist = Vector3.Distance(currentRoom.transform.position, info.roomObject.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = info.roomObject;
                    }
                }
            }
            return closest;
        }

        /// <summary>
        /// 触发房间事件
        /// </summary>
        private void TriggerRoomEvents(GameObject room)
        {
            var info = rooms.Find(r => r.roomObject == room);
            if (info != null)
            {
                // 每次进入都触发
                info.everyEnterScene?.Invoke();

                // 只在第一次进入时触发
                if (!info.hasEntered)
                {
                    info.hasEntered = true;
                    info.firstEnterScene?.Invoke();
                }
            }
        }

        /// <summary>
        /// 获取房间名称
        /// </summary>
        private string GetRoomName(GameObject room)
        {
            var info = rooms.Find(r => r.roomObject == room);
            return info != null ? info.roomDisplayName : "未知房间";
        }

        /// <summary>
        /// 更新房间名显示
        /// </summary>
        private void UpdateRoomName()
        {
            RoomName.text = GetRoomName(currentRoom);
        }

        /// <summary>
        /// 返回当前房间的显示名称
        /// </summary>
        public string GetCurrentRoomName()
        {
            return GetRoomName(currentRoom);
        }

        /// <summary>
        /// 重置所有房间的第一次进入状态
        /// </summary>
        public void ResetFirstEnterFlags()
        {
            foreach (var room in rooms)
            {
                room.hasEntered = false;
            }
        }

        /// <summary>
        /// 跳转到指定序号的房间（延迟瞬移）
        /// </summary>
        public void GoToRoomByIndex(int roomIndex)
        {
            if (rooms == null || rooms.Count == 0)
            {
                Debug.LogError("房间列表为空，无法跳转！");
                return;
            }

            if (roomIndex < 0 || roomIndex >= rooms.Count)
            {
                Debug.LogError($"房间索引 {roomIndex} 超出范围 (0 ~ {rooms.Count - 1})");
                return;
            }

            GameObject targetRoom = rooms[roomIndex].roomObject;
            if (targetRoom == null)
            {
                Debug.LogError($"房间索引 {roomIndex} 对应的对象为空！");
                return;
            }

            StartCoroutine(TeleportAfterDelay(targetRoom));
        }

        /// <summary>
        /// 回到初始房间（第一个房间）
        /// </summary>
        public void GoToFirstRoom()
        {
            GoToRoomByIndex(0);
        }
    }
}
