using System.Collections.Generic;
using System.Text;

public static partial class Utils {
  /// <summary>
  /// Returns a formatted string containing the contents of the dictionary. Uses .ToString() to retrieve
  /// the contents of each object, so if you have a custom object in your dictionary, make sure to override .ToString().
  /// </summary>
  public static string DictionaryToString<Tkey, Tvalue>(IDictionary<Tkey, Tvalue> dict, string name = "Dictionary") {
    StringBuilder sb = new StringBuilder();
    ICollection<Tkey> keys = dict.Keys;

    sb.AppendLine($"{name}:");
    foreach (Tkey key in keys) {
      Tvalue value = dict[key];
      sb.AppendLine($"\\{{{key}, {value.ToString()}\\}}");
    }

    return sb.ToString();
  }

  /// <summary>
  /// Returns a formatted string containing the contents of the IList. Uses .ToString() to retrieve
  /// the contents of each object, so if you have a custom object in your list, make sure to override .ToString().
  /// </summary>
  public static string ListToString<T>(IList<T> list, string name = "List") {
    StringBuilder sb = new StringBuilder();

    sb.AppendLine($"{name}:");
    for (int i = 0; i < list.Count; i++) {
      sb.AppendLine($"[{i}] {list[i].ToString()}");
    }

    return sb.ToString();
  }

  /// <summary>
  /// Returns a formatted string containing the contents of the Array. Uses .ToString() to retrieve
  /// the contents of each object, so if you have a custom object in your array, make sure to override .ToString().
  /// </summary>
  public static string ArrayToString(object[] array, string name = "Array") {
    StringBuilder sb = new StringBuilder();

    sb.AppendLine($"{name}:");
    for (int i = 0; i < array.Length; i++) {
      sb.AppendLine($"[{i}] {array[i].ToString()}");
    }

    return sb.ToString();
  }

  /// <summary>
  /// Returns a formatted string containing the contents of the ICollection. Uses .ToString() to retrieve
  /// the contents of each object, so if you have a custom object in your collection, make sure to override .ToString().
  /// </summary>
  public static string CollectionToString<T>(ICollection<T> collection, string name = "Collection") {
    StringBuilder sb = new StringBuilder();

    sb.AppendLine($"{name}:");
    foreach (object obj in collection) {
      sb.Append($"[{obj.ToString()}");
    }

    return sb.ToString();
  }

  /// <summary>
  /// Truncate long string to desired length
  /// </summary>
  public static string TruncateString(string text, int capLength) {
    if (text.Length <= capLength) return text;

    int length = text.Length - capLength;
    return new StringBuilder(text, 0, length, length).ToString();
  }
}