using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static partial class Utils {
#region search

  public static GameObject FindGameObject(string name) {
    GameObject gameObject = GameObject.Find(name);
    if (gameObject == null) {
      throw new System.Exception($"Could not find a GameObject named: {name}");
    }

    return gameObject;
  }

  public static T FindGameObject<T>(string gameObjectName, bool includeChildren) where T : Component {
    GameObject gameObject = FindGameObject(gameObjectName);
    T component = gameObject.GetComponent<T>();
    if (!component && includeChildren) {
      component = gameObject.GetComponentInChildren<T>();
    }

    if (!component) {
      throw new System.Exception($"Could not find a Component of type {typeof(T).Name} on GameObject {gameObjectName}.");
    }

    return component;
  }

  /// <summary>
  /// Find all transforms which are tagged with specified tag
  /// It does not matter how many children are nested, unlike Unity's own Transform.FindChild
  /// </summary>
  public static List<GameObject> FindChildrenWithTag(GameObject gameObject, string tag) {
    if (!gameObject) {
      throw new NullReferenceException("Null GameObject input");
    }

    Component[] transforms = gameObject.GetComponentsInChildren(typeof(Transform), true);
    List<GameObject> foundedGOs = new List<GameObject>();
    foreach (var transform in transforms) {
      if (transform.gameObject && transform.gameObject.CompareTag(tag)) {
        foundedGOs.Add(transform.gameObject);
      }
    }

    return foundedGOs;
  }

  public static GameObject FindChildRecursive(GameObject gameObject, string childName) {
    if (!gameObject) {
      throw new NullReferenceException("Null GameObject input");
    }

    Component[] transforms = gameObject.GetComponentsInChildren(typeof(Transform), true);
    foreach (var transform in transforms) {
      if (transform.gameObject && string.Equals(transform.gameObject.name, childName, StringComparison.InvariantCulture)) {
        return transform.gameObject;
      }
    }

    return null;
  }

  public static T FindChildRecursive<T>(GameObject gameObject, string childName) where T : Component {
    if (!gameObject) {
      throw new NullReferenceException("Null GameObject input");
    }

    GameObject child = FindChildRecursive(gameObject, childName);
    if (!child) {
      return null;
    }

    T component = child.GetComponent<T>();
    return component ? component : null;
  }

  public static List<GameObject> FindChildrenContains(GameObject gameObject, string keywords) {
    if (!gameObject) {
      throw new NullReferenceException("Null GameObject input");
    }

    Component[] transforms = gameObject.GetComponentsInChildren(typeof(Transform), true);
    List<GameObject> foundedGOs = new List<GameObject>();
    foreach (var transform in transforms) {
      if (transform.gameObject && transform.gameObject.name.Contains(keywords)) {
        foundedGOs.Add(transform.gameObject);
      }
    }

    return foundedGOs;
  }

  public static List<T> FindComponentInChildren<T>(GameObject gameObject) where T : Component {
    if (!gameObject) {
      throw new NullReferenceException("Null GameObject input");
    }

    int childCount = gameObject.transform.childCount;
    List<T> components = new List<T>();
    for (int i = 0; i < childCount; i++) {
      if (gameObject.transform.GetChild(i).TryGetComponent(out T component)) {
        components.Add(component);
      }
    }

    return components;
  }

#endregion

#region instatiate

  private static int uniqueId = 0;

  /// <summary>
  /// Will Instantiate a GameObject and append a unique '_#' to the end of the provided name.
  /// </summary>
  public static GameObject InstantiateGameObjectWithUniqueID(GameObject prefab, string name) {
    GameObject rGameObject;
    if (!prefab) {
      rGameObject = new GameObject("Empty GameObject");
      return rGameObject;
    }

    rGameObject = GameObject.Instantiate(prefab) as GameObject;
    rGameObject.name = $"{name}_#{uniqueId}";
    uniqueId++;
    return rGameObject;
  }

  /// <summary>
  /// Will Instantiate a GameObject as a child of the specified parent and append a unique '_#' to the end of the provided name.
  /// Assumes Vector3.zero for local position and Quaternion.Identity for local rotation.
  /// </summary>
  public static GameObject InstantiateGameObjectAsChildWithUniqueID(GameObject prefab, string name, Transform parent) {
    GameObject rGameObject;
    if (!prefab) {
      rGameObject = new GameObject("Empty GameObject");
      return rGameObject;
    }

    rGameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent) as GameObject;
    rGameObject.name = $"{name}_#{uniqueId}";
    // Override Unity's default behavior of modifying the scale and preserve the scale from the prefab
    rGameObject.transform.localScale = prefab.transform.localScale;
    uniqueId++;
    return rGameObject;
  }

  /// <summary>
  /// Will Instantiate a GameObject as a child of the specified parent and append a unique '_#' to the end of the provided name.
  /// </summary>
  public static GameObject InstantiateGameObjectAsChildWithUniqueID(GameObject prefab, string name, Transform parent, Vector3 localPosition,
                                                                    Quaternion localRotation) {
    GameObject rGameObject;
    if (!prefab) {
      rGameObject = new GameObject("Empty GameObject");
      return rGameObject;
    }

    rGameObject = GameObject.Instantiate(prefab, localPosition, localRotation, parent) as GameObject;
    rGameObject.name = $"{name}_#{uniqueId}";
    // Override Unity's default behavior of modifying the scale and preserve the scale from the prefab
    rGameObject.transform.localScale = prefab.transform.localScale;
    uniqueId++;
    return rGameObject;
  }

  /// <summary>
  /// Instantiate a GameObject as a child of the specified parent. If prefab is null, will create an empty GameObject. 
  /// Assumes Vector3.zero for local position and Quaternion.Identity for local rotation.
  /// </summary>
  public static GameObject InstantiateAsChild(GameObject prefab, Transform parent) {
    return InstantiateAsChild(prefab, parent, Vector3.zero, Quaternion.identity);
  }

  /// <summary>
  /// Instantiate a GameObject as a child of the specified parent. If prefab is null, will create an empty GameObject. 
  /// </summary>
  public static GameObject InstantiateAsChild(GameObject prefab, Transform parent, Vector3 localPosition, Quaternion localRotation) {
    GameObject rGameObject;
    if (!prefab) {
      rGameObject = new GameObject("Empty GameObject");
      return rGameObject;
    }

    rGameObject = GameObject.Instantiate(prefab, localPosition, localRotation, parent) as GameObject;
    // Override Unity's default behavior of modifying the scale and preserve the scale from the prefab
    rGameObject.transform.localScale = prefab.transform.localScale;
    return rGameObject;
  }

  /// <summary>
  /// Instantiate a GameObject as a child of the specified parent. Will return the component type.
  /// </summary>
  public static T InstantiateAsChild<T>(GameObject prefab, Transform parent, Vector3 localPosition, Quaternion localRotation)
    where T : Component {
    GameObject prefabInstance = InstantiateAsChild(prefab, parent, localPosition, localRotation);
    T component = prefabInstance.GetComponent<T>();
    return component;
  }

  /// <summary>
  /// Instantiate a GameObject as a child of the specified parent. Will return the component type. Assumes Vector3.zero for
  /// local position and Quaternion.Identity for local rotation.
  /// </summary>
  public static T InstantiateAsChild<T>(GameObject prefab, Transform parent) where T : Component {
    return InstantiateAsChild<T>(prefab, parent, Vector3.zero, Quaternion.identity);
  }

#endregion

#region destroy

  /// <summary>
  /// Destroy all children game object
  /// </summary>
  public static void DestroyAllChildren(this Transform t) {
    foreach (Transform child in t) {
      child.gameObject.Destroy();
    }
  }

  public static void Destroy(this GameObject go) {
    Object.Destroy(go);
  }

  public static bool IsDestroyed(this GameObject go) {
    return go == null && !ReferenceEquals(go, null);
  }

#endregion
}