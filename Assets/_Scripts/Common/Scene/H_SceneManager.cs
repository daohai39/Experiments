using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

//TODO: Add cache references to loading, unloading, loaded scene
//TODO: Add callback event/delegate for handling loading/unloading scene (success, failed)
public class H_SceneManager : MonoSingleton<H_SceneManager>, ISceneManager {
  
  public void Load(string sceneName) {
    Scene targetScene = SceneManager.GetSceneByName(sceneName);
    if (!targetScene.IsValid()) {
      throw new Exception("[H_SceneManager.LoadScene] Invalid Scene");
    }
    if (targetScene.isLoaded) {
      return;
    }

    LoadScene(sceneName, LoadSceneMode.Single);
  }

  public void LoadAsync(string sceneName) {
    Scene targetScene = SceneManager.GetSceneByName(sceneName);
    if (!targetScene.IsValid()) {
      throw new Exception("[H_SceneManager.LoadScene] Invalid Scene");
    }

    if (targetScene.isLoaded) {
      return;
    }
    LoadSceneAsync(sceneName, LoadSceneMode.Single).Forget();
  }

  private void LoadScene(string sceneName, LoadSceneMode loadSceneMode) {
    SceneManager.LoadScene(sceneName, loadSceneMode);
  }
  
  private async UniTaskVoid LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode) {
    await SceneManager.LoadSceneAsync(sceneName,loadSceneMode);
  }

  public void LoadAdditive(string sceneName) {
    Scene targetScene = SceneManager.GetSceneByName(sceneName);
    if (!targetScene.IsValid()) {
      throw new Exception("[H_SceneManager.LoadScene] Invalid Scene");
    }
    if (targetScene.isLoaded) {
      return;
    }

    LoadScene(sceneName, LoadSceneMode.Additive);
  }
  

  public void LoadAdditiveAsync(string sceneName) {
    Scene targetScene = SceneManager.GetSceneByName(sceneName);
    if (!targetScene.IsValid()) {
      throw new Exception("[H_SceneManager.LoadScene] Invalid Scene");
    }
    if (targetScene.isLoaded) {
      return;
    }
    LoadSceneAsync(sceneName, LoadSceneMode.Additive).Forget();
  }

  public void Unload(string sceneName) {
    Scene targetScene = SceneManager.GetSceneByName(sceneName);
    if (!targetScene.IsValid()) {
      throw new Exception("[H_SceneManager.LoadScene] Invalid Scene");
    }

    if (!targetScene.isLoaded) {
      return;
    }
    UnloadSceneAsync(sceneName).Forget();
  }

  private async UniTask UnloadSceneAsync(string sceneName) {
    await SceneManager.UnloadSceneAsync(sceneName);
  }

  protected override void InternalInit() {
    throw new NotImplementedException();
  }

  protected override void InternalOnDestroy() {
    throw new NotImplementedException();
  }
}