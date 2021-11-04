using UnityEngine;

namespace Common.Base {
  public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance;

    public static T Instance {
      get {
        if (_instance == null) {
          _instance = (T)FindObjectOfType(typeof(T));
        }

        if (_instance == null)
        {
          _instance = new GameObject($"Instance of {typeof(T)}").AddComponent<T>();
        }

        return _instance;
      }
    }

    public static bool HasInstance => _instance != null;

    private void Destroy() {
      if (_instance == this) {
        _instance = null;
      }
    }

    protected virtual void Awake() {
      if (_instance == null) {
        _instance = this as T;
      } else {
        Destroy();
      }
    }

    protected virtual void OnDestroy() {
      Destroy();
    }
  }
}