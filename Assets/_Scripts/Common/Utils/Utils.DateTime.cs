public static partial class Utils {
  /// <summary>
  /// Will return the current timestamp in UNIX/Epoch time.
  /// </summary>
  public static double GetCurrentTimeStamp() {
    return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
  }

  /// <summary>
  /// Will return the input unix timestamp to DateTime
  /// </summary>
  public static System.DateTime UnixTimeStampToDateTime(long unixTimeStamp) {
    System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    dtDateTime = dtDateTime.AddSeconds((double)unixTimeStamp).ToLocalTime();
    return dtDateTime;
  }

  /// <summary>
  /// Will return the input DateTime to UNIX/Epoch time
  /// </summary>
  public static long DateTimeToUnixTimeStamp(System.DateTime dateTime) {
    System.DateTime utc = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    return (long)(dateTime - utc).TotalSeconds;
  }

  /// <summary>
  /// Return a string representation of time in seconds in form of "0d 0h 0m 0s"
  /// </summary>
  /// <param name="seconds"></param>
  /// <returns></returns>
  public static string FormatSecondsToDhmsFormat(int seconds) {
    string space = " ";
    if (seconds <= 0) {
      return string.Empty;
    }

    System.TimeSpan timeSpan = new System.TimeSpan(0, 0, seconds);
    System.Text.StringBuilder formattedString = new System.Text.StringBuilder();

    if (timeSpan.Days > 0) {
      formattedString.Append(timeSpan.Days.ToString()).Append("d");
    }

    if (timeSpan.Hours > 0) {
      if (formattedString.Length > 0) formattedString.Append(space);
      formattedString.Append(timeSpan.Hours).Append("h");
    }

    if (timeSpan.Minutes > 0) {
      if (formattedString.Length > 0) formattedString.Append(space);
      formattedString.Append(timeSpan.Minutes).Append("m");
    }

    if (timeSpan.Seconds > 0) {
      if (formattedString.Length > 0) formattedString.Append(space);
      formattedString.Append(timeSpan.Seconds).Append("s");
    }

    return formattedString.ToString();
  }
}