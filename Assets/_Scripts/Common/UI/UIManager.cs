using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI {
  public class UIManager : MonoSingleton<UIManager> {
    private abstract class QueuedScreen {
      public UIScreen.Id id;
    }

    private class QueuedScreenPush : QueuedScreen {
      public UIScreen.Data data;
      public string prefabName;
      public PushedDelegate callback;
    }

    private class QueuedScreenPop : QueuedScreen {
      public PoppedDelegate callback;

      public override string ToString() {
        return $"[Pop] {id}";
      }
    }

    public delegate void PushedDelegate(UIScreen screen);
    public delegate void PoppedDelegate(UIScreen.Id id);

    //TODO: make this read path from file
    public string resourcePrefabDirectory;

    public Canvas rootCanvas;
    public Camera uiCamera;

    private CanvasScaler _rootCanvasScalar;
    private Dictionary<string, UIScreen> _cache;
    private Queue<QueuedScreen> _queue;
    private List<UIScreen> _stack;
    private HashSet<UIScreen.Id> _stackIdSet;
    private State _state;

    private PushedDelegate _activePushCallback;
    private PoppedDelegate _activePopCallback;
    
    private enum State {
      Ready,
      Push,
      Pop
    }

    protected override void Awake() {
      base.Awake();
      SetupReferences();
      //remove any objects that may be lingering underneath the root
      rootCanvas.transform.DestroyAllChildren();
    }

    protected override void InternalInit() {
      throw new NotImplementedException();
    }

    protected override void InternalOnDestroy() {
      throw new NotImplementedException();
    }

    private void SetupReferences() {
      _rootCanvasScalar = rootCanvas.GetComponent<CanvasScaler>();
      if (!_rootCanvasScalar) {
        throw new Exception($"{rootCanvas.name} must have a CanvasScalar compomnent attached to it for UIManager.");
      }

      _cache = new Dictionary<string, UIScreen>();
      _queue = new Queue<QueuedScreen>();
      _stack = new List<UIScreen>();
      _state = State.Ready;
    }

    /// <summary>
    /// Queue the screen to be pushed onto the screen stack.
    /// Callback will be invoked when the screen is pushed to the stack.
    /// </summary>
    public void Push(UIScreen.Id id, UIScreen.Data data, string prefabName = null, PushedDelegate callback = null) {
      string prefab = prefabName ?? id.defaultPrefabName;
#if PRINT_QUEUE
      DebugPrintQueue(string.Format("[UIManager] QueuePush id: {0}, prefabName: {1}", id, prefab));
#endif
      if (GetScreen(id) != null) { 
        this.LogWarning($"Screen {id} already existed in the stack. Ignoring push request.");
        return;
      }

      QueuedScreenPush push = new QueuedScreenPush();
      push.id = id;
      push.data = data;
      push.prefabName = prefab;
      push.callback = callback;
      
      _queue.Enqueue(push);

#if PRINT_QUEUE
      DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}, Frame: {1}", push, Time.frameCount));
#endif

      if (CanExecuteNextQueueItem()) {
        ExecuteNextQueueItem();
      }
    }

    /// <summary>
    /// Queue the screen to be popped from the screen stack.
    /// This will pop all screens on top of it as well.
    /// Callback will be invoked when the screen is reached or popped if 'include' is true
    /// </summary>
    public void PopTo(UIScreen.Id id, bool include = false, PoppedDelegate callback = null) {
#if PRINT_QUEUE
      DebugPrintQueue(string.Format("[UIManager] QueuePopTo id: {0}, include: {1}", id, include));
#endif
      bool found = false;

      for (int i = 0; i < _stack.Count; i++) {
        UIScreen screen = _stack[i];
        if (screen.id != id) {
          QueuedScreenPop queuedPop = new QueuedScreenPop();
          queuedPop.id = screen.id;
          _queue.Enqueue(queuedPop);
#if PRINT_QUEUE
          DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", queuedPop));
#endif
        } else {
          if (include) {
            QueuedScreenPop queuedPop = new QueuedScreenPop();
            queuedPop.id = screen.id;
            queuedPop.callback = callback;
            
            _queue.Enqueue(queuedPop);
#if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", queuedPop));
#endif
          }
          found = true;
          callback?.Invoke(screen.id);
          break;
        }
      }

      if (!found) {
          this.LogWarning($"{id} was not in the stack. All screens have been popped.");
      }

      if (CanExecuteNextQueueItem()) {
        ExecuteNextQueueItem();
      }
    }

    /// <summary>
    /// Queue the top-most screen to be popped from the screen stack.
    /// Callback will be invoked when the screen is popped from the stack.
    /// </summary>
    public void Pop(PoppedDelegate callback = null) {
#if PRINT_QUEUE
      this.Log($"Queue Pop");
#endif

      UIScreen topScreen = GetTopScreen();
      if (!topScreen) {
        return;
      }

      QueuedScreenPop pop = new QueuedScreenPop();
      pop.id = topScreen.id;
      pop.callback = callback;
      _queue.Enqueue(pop);

#if PRINT_QUEUE
      DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", pop));
#endif
      if (CanExecuteNextQueueItem()) {
        ExecuteNextQueueItem();
      }
    }

    private int interval = 3;
    
    public void OnUpdate() {
      //wait for interval of 3 frames before do anything
      if (Time.frameCount % interval != 0) return;
      if (CanExecuteNextQueueItem()) {
        ExecuteNextQueueItem();
      }
    }
    
    private UIScreen GetTopScreen() {
      if (_stack.Count > 0) {
        return _stack[0];
      }

      return null;
    }

    private void ExecuteNextQueueItem() {
      QueuedScreen queued = _queue.Dequeue();

#if PRINT_QUEUE
      DebugPrintQueue(string.Format("[UIManager] Dequeued Screen: {0}, Frame: {1}", queued, Time.frameCount));
#endif
      if (queued is QueuedScreenPush) {
        //push screen.
        QueuedScreenPush queuedPush = (QueuedScreenPush)queued;
        UIScreen screenInstance;

        if (_cache.TryGetValue(queuedPush.prefabName, out screenInstance)) {
          //use cached instance of screen
          _cache.Remove(queuedPush.prefabName);

#if PRINT_CACHE
          DebugPrintCache(string.Format("[UIManager] Screen retrieved from Cache: {0}", queuedPush.prefabName));
#endif
          screenInstance.transform.SetAsLastSibling();
          screenInstance.gameObject.SetActive(true);
        } else {
          //instantiate a new instance of screen
          string path = System.IO.Path.Combine(resourcePrefabDirectory, queuedPush.prefabName);
          UIScreen prefab = Resources.Load<UIScreen>(path);

          screenInstance = Utils.InstantiateAsChild<UIScreen>(prefab.gameObject, rootCanvas.transform);
          screenInstance.Setup(queuedPush.id, queuedPush.prefabName);
        }

        UpdateSortOrderOverrides();
        
        //tell previous top screen that it is losing focus
        UIScreen topScreen = GetTopScreen();
        if (topScreen != null) {
#if PRINT_FOCUS
          DebugPrintFocus(string.Format("[UIManager] Lost Focus: {0}", topScreen.id));
#endif
          topScreen.OnFocusLost();
        }
        
        //insert new screen at the top of the stack
        _state = State.Push;
        _stack.Insert(0, screenInstance);

        _activePushCallback = queuedPush.callback;
#if PRINT_STACK
        DebugPrintStack(string.Format("[UIManager] Pushing Screen: {0}, Frame: {1}", queued.id, Time.frameCount));
#endif
        screenInstance.onPushFinished += HandlePushFinished;
        screenInstance.OnPush(queuedPush.data);

        if (_queue.Count == 0) {
#if PRINT_FOCUS
          DebugPrintFocus(string.Format("[UIManager] Gained Focus: {0}", screenInstance.id));
#endif
          // Screen gain focus when it is on top of the screen stack and no other items in the queue.
          screenInstance.OnFocus();
        }
      } else {
        //pop screen
        QueuedScreenPop queuedPop = (QueuedScreenPop)queued;
        UIScreen screenToPop = GetTopScreen();
        if (screenToPop.id != queued.id) {
          throw new Exception($"The top screen does not match the queued pop. TopScreen: {screenToPop.id}, QueuedPop: {queued.id}");
        }
#if PRINT_FOCUS
        DebugPrintFocus(string.Format("[UIManager] Lost Focus: {0}", screenToPop.id));
#endif
        screenToPop.OnFocusLost();

        _state = State.Pop;
        _stack.RemoveAt(0);
        
        //tell new top screen that it is gaining focus
        UIScreen topScreen = GetTopScreen();
        if (topScreen) {
          if (_queue.Count == 0) {
#if PRINT_FOCUS
            DebugPrintFocus(string.Format("[UIManager] Gained Focus: {0}", newTopScreen.id));
#endif
            //Screen gains focus when it is on top of the screen stack and no other items in the queue.
            topScreen.OnFocus();
          }
        }

        _activePopCallback = queuedPop.callback;

#if PRINT_STACK
        DebugPrintStack(string.Format("[UIManager] Popping Screen: {0}, Frame: {1}", queued.id, Time.frameCount));
#endif

        screenToPop.onPopFinished += HandlePopFinished;
        screenToPop.OnPop();
      }
    }

    private void HandlePopFinished(UIScreen screen) {
      screen.onPopFinished -= HandlePopFinished;
      if (screen.keepCached) {
        //store the cache for late use.
        screen.gameObject.SetActive(false);
        
        //TODO: need to have a better cache storage mechanism that supports multiple screens of the same prefab?
        if (!_cache.ContainsKey(screen.PrefabName)) {
          _cache[screen.PrefabName] = screen;
#if PRINT_CACHE
          DebugPrintCache(string.Format("[UIManager] Screen added to Cache: {0}", screen.PrefabName));
#endif
        }
      } else {
        screen.gameObject.Destroy();
      }

      _state = State.Ready;

      _activePopCallback?.Invoke(screen.id);
      _activePopCallback = null;

      if (CanExecuteNextQueueItem()) {
        ExecuteNextQueueItem();
      }
    }

    private void HandlePushFinished(UIScreen screen) {
      screen.onPushFinished -= HandlePopFinished;

      _state = State.Ready;
      
      _activePushCallback?.Invoke(screen);
      _activePushCallback = null;
      
      if (CanExecuteNextQueueItem()) {
        ExecuteNextQueueItem();
      }
    }

    private void UpdateSortOrderOverrides() {
      int managedOrder = 0;
      List<UIScreen> childScreens = Utils.FindComponentInChildren<UIScreen>(rootCanvas.gameObject);
      Canvas canvas;
      for (int i = 0; i < childScreens.Count; i++) {
        if (childScreens[i].TryGetComponent(out canvas)) {
          canvas.overrideSorting = true;
          if (childScreens[i].overrideManagedSorting) {
            canvas.sortingOrder = childScreens[i].overrideSortValue;
          } else {
            canvas.sortingOrder = managedOrder;
            managedOrder++;
          }
        }
      }
      
    }

    private bool CanExecuteNextQueueItem() {
      if (_state == State.Ready) {
        if (_queue.Count > 0) {
          return true;
        }
      }

      return false;
    }

    private UIScreen GetScreen(UIScreen.Id id) {
      int count = _stack.Count;
      for (int i = 0; i < count; i++) {
        if (_stack[i].id == id) {
          return _stack[i];
        }
      }

      return null;
    }

    private T GetScreen<T>(UIScreen.Id id) where T : UIScreen {
      UIScreen screen = GetScreen(id);
      return (T)screen;
    }

    public void SetVisibility(bool visible) {
      CanvasGroup canvasGroup = rootCanvas.GetComponent<CanvasGroup>();
      if (!canvasGroup) {
        canvasGroup = rootCanvas.gameObject.AddComponent<CanvasGroup>();
      }

      canvasGroup.alpha = visible ? 1.0f : 0.0f;
      canvasGroup.interactable = visible;
      canvasGroup.blocksRaycasts = visible;
    }

    public bool IsVisible() {
      CanvasGroup canvasGroup = rootCanvas.GetComponent<CanvasGroup>();

      if (!canvasGroup) {
        return true;
      }
      
      bool isVisible = canvasGroup.alpha > 0.0f &&
                       canvasGroup.interactable &&
                       canvasGroup.blocksRaycasts;
      return isVisible;
    }

#region Debug

    private void PrintStack (string optionalEventMsg)
    {
      var sb = new System.Text.StringBuilder();

      if (!string.IsNullOrEmpty(optionalEventMsg))
        sb.AppendLine(optionalEventMsg);

      sb.AppendLine("[UIManager Screen Stack]");

      for (int i = 0; i < _stack.Count; i++)
      {
        sb.AppendLine($"{_stack[i].id}");
      }

      this.Log(sb.ToString());
    }

    private void DebugPrintQueue (string optionalEventMsg)
    {
      var sb = new System.Text.StringBuilder();

      if (!string.IsNullOrEmpty(optionalEventMsg))
        sb.AppendLine(optionalEventMsg);

      sb.AppendLine("[UIManager Screen Queue]");

      foreach (QueuedScreen queued in _queue)
      {
        sb.AppendLine(queued.ToString());
      }

      this.Log(sb.ToString());
    }

    private void DebugPrintCache (string optionalEventMsg)
    {
      var sb = new System.Text.StringBuilder();

      if (!string.IsNullOrEmpty(optionalEventMsg))
        sb.AppendLine(optionalEventMsg);

      sb.AppendLine("[UIManager Screen Cache]");

      foreach (KeyValuePair<string, UIScreen> cached in _cache)
      {
        sb.AppendLine(cached.Key);
      }

      this.Log(sb.ToString());
    }

    private void DebugPrintFocus(string message) {
      this.Log(message);
    }

#endregion
  }
}