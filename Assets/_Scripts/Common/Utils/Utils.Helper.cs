using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static partial class Utils {
  private static Camera CameraMain;

  /// <summary>
  /// Cache and return main camera for better performance
  /// </summary>
  public static Camera GetMainCamera() {
    if (!CameraMain) CameraMain = Camera.main;
    return CameraMain;
  }

  private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

  /// <summary>
  /// Cache and try to return a corresponding WaitForSeconds instance instead of create a new one everytime
  /// </summary>
  public static WaitForSeconds GetWait(float time) {
    if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
    WaitDictionary[time] = new WaitForSeconds(time);
    return WaitDictionary[time];
  }

  private static PointerEventData _eventDataCurrentPosition;
  private static List<RaycastResult> _results;

  /// <summary>
  /// Check if current input is overlay with any UI
  /// </summary>
  public static bool IsOverUI() {
    _eventDataCurrentPosition = new PointerEventData(EventSystem.current) {position = Input.mousePosition};
    _results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
    return _results.Count > 0;
  }

  /// <summary>
  /// Get world position of any UI Canvas element
  /// </summary>
  public static Vector2 GetWorldPositionOfCanvasElement(RectTransform el) {
    RectTransformUtility.ScreenPointToWorldPointInRectangle(el, el.position, CameraMain, out var result);
    return result;
  }
}