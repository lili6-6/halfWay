using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;

namespace GameJam
{
    // 控制视频播放的脚本
    // 绑定在一个空物体上，rowImage 指向挂载了 VideoPlayer 组件的对象
    [System.Serializable]
    public class CaseData
    {
        public string caseName;
        public Sprite caseImage;
        //public Image caseImage;
        public string sceneName;
        public VideoController videoController;    // 场景里的 VideoPlayer
        //public float Duration = 5f; // 视频时长，单位秒
    }

    public class CaseManager : MonoBehaviour
    {
        public List<CaseData> cases;

        public TextMeshProUGUI titleText;
        public Image caseImage;
        // public Transform loadingContainer;

        private int currentCaseIndex = -1;

        public UnityEvent TransScene;
        public float transDuration = 2f;

        private string DefaultText;

        //private Image CurrentImage;


        public void Start()
        {
            DefaultText = titleText.text;
        }

        public void Update()
        {
            // 按 Esc 键退出游戏
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                quitGame();
            }
        }
        // 点击案件
        public void SelectCase(int index)
        {
            currentCaseIndex = index;
            CaseData data = cases[index];

            // 更新标题和图片
            titleText.text = data.caseName;
            caseImage.sprite = data.caseImage;


            //// 更新loading动画
            //foreach (Transform t in loadingContainer)
            //    Destroy(t.gameObject);


            // 关闭所有视频播放器
            foreach (var c in cases)
            {
                if (c.videoController != null)
                    c.videoController.gameObject.SetActive(false);
            }

            // 激活当前案件的视频播放器
            if (data.videoController != null)
            {
                data.videoController.gameObject.SetActive(true);
            }
        }

        // 点击 Loading
        public void OnClickLoading()
        {
            if (currentCaseIndex < 0) return;

            CaseData data = cases[currentCaseIndex];
            VideoPlayer vp = data.videoController.transform.parent.GetComponent<VideoPlayer>();
            data.videoController.transform.parent.GetComponent<RawImage>().DOFade(1, 2f);
            data.videoController.transform.parent.GetComponent<RawImage>().DOColor(new Color(1, 1, 1, 1), 2f);//
            if (vp == null)
            {
                Debug.LogError("VideoPlayer 组件没找到！");
                SceneManager.LoadScene(data.sceneName);
                return;
            }

            vp.loopPointReached += OnVideoFinished; // 视频播放完自动调用
            data.videoController.PlayVideo();       // 播放视频
        }



        private void OnVideoFinished(VideoPlayer vp)
        {
            vp.loopPointReached -= OnVideoFinished;
            StartCoroutine(TransEvent());



        }

        private IEnumerator TransEvent()
        {
            TransScene.Invoke();
            yield return new WaitForSeconds(transDuration);
            SceneManager.LoadScene(cases[currentCaseIndex].sceneName);
        }


        public void ResetUI()
        {
            // 重置 Text
            if (titleText != null)
            {
                titleText.text = DefaultText;  // 清空文本
            }

            // 重置 Image
            if (caseImage != null)
            {
                caseImage.sprite = null;  // 清空图片
            }
        }

        public void quitGame()
        {
           
         #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
         #else
                Application.Quit();
        #endif
            
        }
    }
}