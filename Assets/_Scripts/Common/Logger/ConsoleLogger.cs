using Object = UnityEngine.Object;
using System;
using UnityEngine;

public static class ConsoleLogger {
  private static string Color(this string myStr, string color) {
    return $"<color={color}>{myStr}</color>";
  }

  [System.Diagnostics.Conditional("ENABLE_LOG")]
  private static void DoLog(Action<string, Object> LogFunction, string prefix, Object myObj, params object[] msg) {
    var name = (myObj ? myObj.name : "NullObject").Color("magenta");
    LogFunction($"{prefix}[{name}]: {String.Join("; ", msg)}\n ", myObj);
  }

  public static void Log(this Object myObj, params object[] msg) {
    DoLog(Debug.Log, "", myObj, msg);
  }

  public static void LogError(this Object myObj, params object[] msg) {
    DoLog(Debug.LogError, "<!>".Color("red"), myObj, msg);
  }

  public static void LogWarning(this Object myObj, params object[] msg) {
    DoLog(Debug.LogWarning, "⚠️".Color("yellow"), myObj, msg);
  }

  public static void LogSuccess(this Object myObj, params object[] msg) {
    DoLog(Debug.Log, "☻".Color("green"), myObj, msg);
  }
}