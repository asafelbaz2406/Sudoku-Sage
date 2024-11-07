using UnityEngine;

[AddComponentMenu("Assets/Scripts/Logging")]
public class Logger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _showLogs;
    [SerializeField] private string _prefix;
    [SerializeField] private Color prefixColor;
    private string hexColor;
    [SerializeField] private LogType logType;
    

#if UNITY_EDITOR
    private void OnValidate() 
    {
        hexColor = "#" + ColorUtility.ToHtmlStringRGBA(prefixColor);    
    }
#endif

    public void Log(string message, Object sender)
    {
        if(!_showLogs) return;

        //Debug.Log($"{_prefix}: {message}", sender);
        Debug.Log($"<color={hexColor}>{logType} {_prefix}: {message}</color>", sender);
    }
}

