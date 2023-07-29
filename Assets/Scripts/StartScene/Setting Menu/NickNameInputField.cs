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
            Debug.LogWarning("�г��� �Է�â�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void SetNickName(string nickName)
    {
        MenuManager.NickName = nickName;
    }
}
