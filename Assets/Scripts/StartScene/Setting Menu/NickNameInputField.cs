using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NickNameInputField : MonoBehaviour
{
    public InputField nickNameInputField;

    private void Start()
    {
        if (nickNameInputField != null)
        {
            nickNameInputField.text = MenuManager.NickName;
        }
        else
        {
            Debug.LogWarning("닉네임 입력창이 할당되지 않았습니다.");
        }
    }

    public void SetNickName(string nickName)
    {
        MenuManager.NickName = nickName;
    }
}
