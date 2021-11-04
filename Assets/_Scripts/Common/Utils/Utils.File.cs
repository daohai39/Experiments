using System.IO;
using UnityEngine;

public static partial class Utils {
  public static void ClearFile(string filePath) {
    DeleteFile(filePath);
    using (FileStream fs = File.Create(filePath)) {
      fs.Close();
    }
  }

  public static void DeleteFile(string filePath) {
    if (!File.Exists(filePath)) return;
    File.Delete(filePath);
  }

  public static string GetStreamingAssetPath() {
    return Application.streamingAssetsPath;
  }

  public static string GetStreamingAssetsSubFolderPath(string folder) {
    return GetStreamingAssetPath() + Path.DirectorySeparatorChar + folder;
  }

  public static string GetStreamingAssetsFilePath(string fileName) {
    return GetStreamingAssetPath() + Path.DirectorySeparatorChar + fileName;
  }

  public static string GetStreamingAssetsFileInDirectoryPath(string folder, string fileName) {
    return GetStreamingAssetsFilePath(folder + Path.DirectorySeparatorChar + fileName);
  }
}