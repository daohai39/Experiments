using UnityEngine;

namespace Common.Exceptions {
  public class CustomExceptionHandler : MonoBehaviour
  {
    void Awake()
    {
      Application.logMessageReceived += HandleException;
      DontDestroyOnLoad(gameObject);
    }

    void HandleException(string logString, string stackTrace, LogType type)
    {
      if (type == LogType.Exception) {
        //do something
      }
    }
  }
}