using System;

public static partial class Utils {
  public static string Base64Encode(string plainText) {
    byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
    return System.Convert.ToBase64String(plainTextBytes);
  }

  public static string Base64Decode(string base64EncodedData) {
    byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
    return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
  }

  private const int QuickEncryptLength = 220;

  public static byte[] GetQuickXorBytes(byte[] bytes, byte[] code) {
    return GetXorBytes(bytes, 0, QuickEncryptLength, code);
  }

  public static void GetQuickSelfXorBytes(byte[] bytes, byte[] code) {
    GetSelfXorBytes(bytes, 0, QuickEncryptLength, code);
  }

  public static byte[] GetXorBytes(byte[] bytes, byte[] code) {
    if (bytes == null) {
      return null;
    }

    return GetXorBytes(bytes, 0, bytes.Length, code);
  }

  public static byte[] GetXorBytes(byte[] bytes, int startIndex, int length, byte[] code) {
    if (bytes == null) {
      return null;
    }

    int bytesLength = bytes.Length;
    byte[] result = new byte[bytesLength];
    Array.Copy(bytes,0,result,0,bytesLength);
    GetSelfXorBytes(result, startIndex, length, code);
    return result;
  }

  public static void GetSelfXorBytes(byte[] bytes, byte[] code) {
    GetSelfXorBytes(bytes,0,bytes.Length, code);
  }
  
  public static void GetSelfXorBytes(byte[] bytes, int startIndex, int length, byte[] code) {
    if (bytes == null) {
      return;
    }

    if (code == null) {
#if ENABLE_DEBUG_EXCEPTION || UNITY_EDITOR
      throw new Exception("[Utils.Encryption.GetSelfXorBytes] Code is invalid");
#endif
      return;
    }

    int codeLength = code.Length;
    if (codeLength <= 0) {
#if ENABLE_DEBUG_EXCEPTION || UNITY_EDITOR
      throw new Exception("[Utils.Encryption.GetSelfXorBytes] Code length is invalid");
#endif
      return;
    }

    if (startIndex < 0 || length < 0 || startIndex + length > bytes.Length) {
#if ENABLE_DEBUG_EXCEPTION || UNITY_EDITOR
      throw new Exception("[Utils.Encryption.GetSelfXorBytes] Start index or length is invalid");
#endif
      return;
    }

    int codeIndex = startIndex % codeLength;
    for (int i = startIndex; i < length; i++) {
      bytes[i] ^= code[codeIndex++];
      codeIndex %= codeLength;
    }
  }
}