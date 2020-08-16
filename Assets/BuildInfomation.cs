using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildInfomation : MonoBehaviour
{
    //打包的地址集合
    public static List<string> BuildScenePaths = new List<string>();
    //打包后存储地址
    public static string Pathlevels =  File.ReadAllLines(Application.dataPath + "/Pathlevels.txt", Encoding.Default)[0];
    //打包获取的Scenes地址
    public static string[] levels = new string[] { File.ReadAllLines(Application.dataPath + "/levels.txt", Encoding.Default)[0] };
    //场景的地址
    public static List<string> ScenePaths = new List<string>();
    //打包的场景数组个数
    public static int BuildNum = 0;
    /// <summary>
    /// 回调的函数
    /// </summary>
    public static void BuildBack()
    {

        if (BuildNum> ScenePaths.Count)
        {
            Debug.Log("完成");
        }
        else
        {
            levels = new string[] { ScenePaths[BuildNum] };
            Pathlevels = BuildScenePaths[BuildNum];
            BuildNum++;
        }
     



    }
}
