using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.UI {
  [RequireComponent(typeof(Canvas))]
  public abstract class UIScreen : MonoBehaviour {
    public class Id {
      readonly string name;
      public readonly string defaultPrefabName;

      public Id(string name) {
        this.name = name;
      }

      public Id(string name, string defaultPrefabName) {
        this.name = name;
        this.defaultPrefabName = defaultPrefabName;
      }

      public override int GetHashCode() {
        return name.GetHashCode();
      }

      public static bool operator == (Id x, Id y) {
        if (ReferenceEquals(x, null))
          return ReferenceEquals(y, null);
        return x.Equals(y);
      }

      public static bool operator !=(Id x, Id y) {
        return !(x == y);
      }

      public override bool Equals(object obj) {
        Id other = obj as Id;
        if (ReferenceEquals(other, null))
          return false;
        return name == other.name;
      }

      public override string ToString() {
        return name;
      }
    }

    
    /// <summary>
    /// Data container that is passed along to Screens that are being pushed.
    /// Screens can use these to setup themselves with custom data provided at run-time
    /// </summary>
    public class Data {
      private Dictionary<string, object> _data;

      public Data() {
        _data = new Dictionary<string, object>();
      }

      public Data(int capacity) {
        _data = new Dictionary<string, object>(capacity);
      }

      public void Add(string key, object data) {
        _data.Add(key, data);
      }

      public T Get<T>(string key, T defaultValue) {
        object datum = Get(key);
        try {
          return (T)datum;
        } catch {
          //fire exception in development
#if ENABLE_DEBUG_EXCEPTION || UNITY_EDITOR
          throw new Exception($"[UIScreen.Data] Could not case data object {key} to type {typeof(T).Name}");
#endif
        }
        return defaultValue;
      }

      private object Get(string key) {
        if (!_data.TryGetValue(key, out var datum))
          throw new Exception($"[UIScreen.Data] No object found for key {key}");
        return datum;
      }

      public bool TryGet<T>(string key, out T datum, T defaultValue) {
        if (_data.TryGetValue(key, out var datumObj)) {
          try {
            datum = (T)datumObj;
            return true;
          } catch {
            // fire exception in development
#if ENABLE_DEBUG_EXCEPTION || UNITY_EDITOR
            throw new Exception($"[UIScreen.Data] Could not cast object {key} to type {typeof(T).Name}");
#endif
          }
        }

        datum = defaultValue;
        return false;
      }

      private bool TryGet(string key, out object datum) {
        return _data.TryGetValue(key, out datum);
      }
    }
    
    public Id id {get; private set;}
    public string PrefabName {get; private set;}

    public bool keepCached;
    public bool overrideManagedSorting;
    public int overrideSortValue;

    public delegate void ScreenDelegate(UIScreen screen);

    public event ScreenDelegate onPushFinished;
    public event ScreenDelegate onPopFinished;

    public void Setup(Id id, string prefabName) {
      this.id = id;
      PrefabName = prefabName;

      OnSetup();
    }

    /// <summary>
    /// Setup is called after instantiate a Screen prefab.
    /// It is only called once for the lifecycle of Screen
    /// </summary>
    public abstract void OnSetup();

    /// <summary>
    /// Called by the UIManager when this screen is pushed to the screen stack.
    /// Be sure to call PushFinished when your screen is done pushing.
    /// Delaying the PushFinished call allows the screen to delay execution of the UIManager's screen queue.
    /// </summary>
    public abstract void OnPush(Data data);

    /// <summary>
    /// Called by the UIManager when this screen is popped from the screen stack.
    /// Be sure to call PopFinished when your screen is done popping.
    /// Delaying the PopFinished call allows the screen to delay execution of the UIManager's screen queue.
    /// </summary>
    public abstract void OnPop();

    /// <summary>
    /// Called by the UIManager when this Screen becomes the top most screen in the stack.
    /// </summary>
    public abstract void OnFocus();

    /// <summary>
    /// Called by the UIManager when this Screen is no longer the top most screen in the stack.
    /// </summary>
    public abstract void OnFocusLost();

    protected void PushFinished() {
      onPushFinished?.Invoke(this);
    }

    protected void PopFinished() {
      onPopFinished?.Invoke(this);
    }
  }
}