using UnityEngine;
using UnityEngine.Video;

namespace GameJam
{


    public class VideoController : MonoBehaviour
    {
        [SerializeField] private GameObject rowImage;   // 挂载了 VideoPlayer 的对象
        private VideoPlayer videoPlayer;

        void Start()
        {
            if (rowImage != null)
            {
                videoPlayer = rowImage.GetComponent<VideoPlayer>();
            }
        }

        // 播放视频
        public void PlayVideo()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
        }

        // 暂停视频
        public void PauseVideo()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Pause();
            }
        }
    }
}
