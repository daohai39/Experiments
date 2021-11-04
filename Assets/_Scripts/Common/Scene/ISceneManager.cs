public interface ISceneManager {
  void Load(string sceneName);
  void LoadAsync(string sceneName);
  void LoadAdditive(string sceneName);
  void LoadAdditiveAsync(string sceneName);
  void Unload(string sceneName);
}