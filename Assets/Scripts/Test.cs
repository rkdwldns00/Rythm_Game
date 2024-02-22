using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class Test : MonoBehaviour
{
    public Text text;

    private AndroidJavaObject activityContext = null;
    private AndroidJavaClass javaClass = null;
    private AndroidJavaObject javaClassInstance = null;

    void Awake()
    {
        text.text = "awake";
        //일단 아까 plugin의 context를 설정해주기 위해
        //유니티 자체의 UnityPlayerActivity를 가져옵시다.
        AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        text.text = "new";
        activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        text.text = "context";


        //클래스를 불러와줍니다.
        //패키지명 + 클래스명입니다.
        try
        {
            javaClass = new AndroidJavaClass("com.example.project.fileSelector");
            text.text = "load class";
            if (javaClass != null)
            {
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("GetInstance");
                text.text = "getInstance";
            }

        }
        catch (Exception e)
        {
            text.text = "Awake# " + e.Message;
        }
    }

    public void TestRun()
    {
        text.text = "run";
        try
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            else
            {
                javaClassInstance.Call("Test", gameObject.name, "CallBack");
                //ConnectPlugin();
            }
            text.text = "loading";
        }
        catch (Exception e)
        {
            text.text = "TestRun#" + e.Message;
        }
    }

    private void ConnectPlugin()
    {
        javaClassInstance.Call("OpenFileSelectUI", gameObject.name, "CallBack");
    }

    public void CallBack(string data)
    {
        text.text = data;
    }

    private void OpenFilePicker()
    {
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_OPEN_DOCUMENT"));
        intentObject.Call<AndroidJavaObject>("addCategory", intentClass.GetStatic<string>("CATEGORY_OPENABLE"));
        intentObject.Call<AndroidJavaObject>("setType", "*/*");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        int requestCode = 0; // You can define your request code here
        currentActivity.Call("startActivityForResult", intentObject, requestCode);
    }
}