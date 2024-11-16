using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogDisplay : MonoBehaviour
{
    public string output = "";
    public string stack = "";

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string StackTrace, LogType type)
    {
        output = logString;
        stack = StackTrace;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(200, 5, 800, 60), output);
        GUI.Label(new Rect(200, 65, 800, 60), stack);
    }
}
