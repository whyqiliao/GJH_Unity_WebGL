using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace lfy
{
    public class Product : MonoBehaviour
    
    {
        [DllImport("__Internal")]
	    private static extern void OpenPage(string url);
        [Header("产品网址")]
        public string ObjectURL;
        [Header("中文产品名称")]
        public string ChName;
        [Header("英文产品名称"),Multiline()]
        public string EnName;
        [Header("双击检测时间")]
        public float DoubleClickTimeout = 0.2f;
        [Header("中文介绍"),Multiline()]
        public string ChIntro;
        [Header("英文介绍"),Multiline()]
        public string EnIntro;
        [Header("背景图")]
        public Texture texture;
        [Header("中文主标题字体大小")]
        public int MaintTitleFontSize_Ch=28;
        [Header("英文主标题字体大小")]
        public int MaintTitleFontSize_En=16;
        [Header("介绍文字向下偏移量")]
        public int IntroOffset=130;
        [Header("主标题中文偏移量X")]
        public int MainTitleOffset_Ch=0;
        [Header("主标题英文偏移量X")]
        public int MainTitleOffset_En=0;
        [Header("介绍栏画布大小")]
        public int IntroBoxScale=300;
        [Header("展品名称副标题偏移量")]
        public int SubTitleOffset_Name=35;
        [Header("展品简介副标题偏移量")]
        public int SubTitleOffset_Intro=90;
        [Header("所有文字横向偏移量")]
        public int XOffset=30;
        private bool clickEnable = true;
        private bool doubleClick = false;
        private int YOffset=10;
        private int TextXOffset=30;
        private bool isShowInfo;
        private float doubleClickStart = 0;

        private GUIStyle MainTitle_Ch=new GUIStyle();
        private GUIStyle MainTitle_En=new GUIStyle();
        private GUIStyle SubTitle_Ch=new GUIStyle();
        private GUIStyle SubTitle_En_1=new GUIStyle();
        private GUIStyle SubTitle_En_2=new GUIStyle();
        private GUIStyle Body_Ch=new GUIStyle();
        private GUIStyle Body_En=new GUIStyle();
        public Font CustomFont;
        void Start()
        {
            //初始化字体
            MainTitle_Ch.font=CustomFont;
            MainTitle_Ch.font=CustomFont;
            MainTitle_En.font=CustomFont;
            SubTitle_Ch.font=CustomFont;
            SubTitle_En_1.font=CustomFont;
            SubTitle_En_2.font=CustomFont;
            Body_Ch.font=CustomFont;
            Body_En.font=CustomFont;
            //初始化各字体数据
            MainTitle_Ch.fontSize=MaintTitleFontSize_Ch;
            SubTitle_Ch.fontSize=System.Convert.ToInt32(MaintTitleFontSize_Ch*0.6);
            Body_Ch.fontSize=System.Convert.ToInt32(MaintTitleFontSize_Ch*0.4);
            MainTitle_En.fontSize=MaintTitleFontSize_En;
            SubTitle_En_1.fontSize=System.Convert.ToInt32(MaintTitleFontSize_En*0.6);
            SubTitle_En_2.fontSize=System.Convert.ToInt32(MaintTitleFontSize_En*0.33);
            Body_En.fontSize=System.Convert.ToInt32(MaintTitleFontSize_En*0.6);

            MainTitle_Ch.normal.textColor=new Color(1,1,1);
            SubTitle_Ch.normal.textColor=new Color(1,1,1);
            Body_Ch.normal.textColor=new Color(1,1,1);
            MainTitle_En.normal.textColor=new Color(1,1,1);
            SubTitle_En_1.normal.textColor=new Color(1,1,1);
            SubTitle_En_2.normal.textColor=new Color(1,1,1);
            Body_En.normal.textColor=new Color(1,1,1);
        }
        void OnMouseEnter()
        {
            isShowInfo=true;
        }
        void OnMouseExit()
        {
            isShowInfo=false;
        }
        public string geturl()
        {
            return ObjectURL;
        }
        void OnMouseUp ()
        {
            if(clickEnable)    
            {
                clickEnable = false;
                StartCoroutine(trapDoubleClicks(DoubleClickTimeout));

            }
         
        }
        IEnumerator trapDoubleClicks(float timer)
        {
            float endTime = Time.time + timer;
            while (Time.time < endTime)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Double click!");
                    OpenPage("https://"+ObjectURL);
                    yield return new WaitForSeconds(0.4f);
                    clickEnable = true;
                    doubleClick = true;
                }
                yield return 0;
            }
         
            if(!doubleClick)
            {
                //isShowInfo=true;
                Debug.Log("Single click");
            }
            else
            {
                doubleClick = false;
            }
         
            clickEnable = true; 
            yield return 0;
        }

        void OnGUI()
        {
            if(isShowInfo)
            {
                if(texture)
                {
                    GUI.DrawTexture(new Rect(Input.mousePosition.x+XOffset, Screen.height - Input.mousePosition.y-YOffset, 2*IntroBoxScale+2*XOffset, IntroBoxScale+2*YOffset),texture,ScaleMode.StretchToFill, true, 10.0F);
                }
                GUI.Label(new Rect(Input.mousePosition.x+IntroBoxScale-System.Convert.ToInt32(0.5*XOffset)+MainTitleOffset_Ch, Screen.height - Input.mousePosition.y-YOffset+25, 150, 30),ChName,MainTitle_Ch);
                GUI.Label(new Rect(Input.mousePosition.x+IntroBoxScale-System.Convert.ToInt32(0.5*XOffset)+MainTitleOffset_En, Screen.height - Input.mousePosition.y-YOffset+55, 150, 30),EnName,MainTitle_En);

                GUI.Label(new Rect(Input.mousePosition.x+XOffset+TextXOffset,Screen.height - Input.mousePosition.y-YOffset+SubTitleOffset_Name,150,30),"Exhobit ChName",SubTitle_Ch);
                GUI.Label(new Rect(Input.mousePosition.x+XOffset+TextXOffset,Screen.height - Input.mousePosition.y-YOffset+SubTitleOffset_Name+20,150,30),"Exhibit Name",SubTitle_En_1);

                GUI.Label(new Rect(Input.mousePosition.x+XOffset+TextXOffset,Screen.height - Input.mousePosition.y-YOffset+SubTitleOffset_Intro,150,30),"Exhibit Product",SubTitle_Ch);
                GUI.Label(new Rect(Input.mousePosition.x+XOffset+TextXOffset,Screen.height - Input.mousePosition.y-YOffset+SubTitleOffset_Intro+20,150,30),"Introductions to Exhibit",SubTitle_En_2);

                GUI.Label(new Rect(Input.mousePosition.x+XOffset+TextXOffset, Screen.height - Input.mousePosition.y-YOffset+IntroOffset, IntroBoxScale, IntroBoxScale),ChIntro,Body_Ch);
                
                GUI.Label(new Rect(Input.mousePosition.x+XOffset+2*TextXOffset+IntroBoxScale, Screen.height - Input.mousePosition.y-YOffset+IntroOffset, IntroBoxScale, IntroBoxScale),EnIntro,Body_En);
            }
        }
    }
}

