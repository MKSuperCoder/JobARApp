using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    // Method to quit the application
    public void QuitApplication()
    {
        // Log a message indicating the quit button was pressed
        Debug.Log("Quit button pressed. Application will quit.");

        // Quit the application
        Application.Quit();

#if UNITY_IOS
        // For iOS, forcefully terminate the application
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
}
