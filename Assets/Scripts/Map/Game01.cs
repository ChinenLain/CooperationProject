using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game01 : MonoBehaviour
{
    public void OnButtonClick()
    {
        DialogBoxManager.instance.OpenDiglogBox("01", 0);
        Debug.Log("1");
    }
}
