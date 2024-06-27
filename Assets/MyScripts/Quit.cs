using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    public void QuitApplication()
    {
        Debug.Log("Quit button pressed. Application will quit.");

        Application.Quit();

#if UNITY_IOS
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
}

