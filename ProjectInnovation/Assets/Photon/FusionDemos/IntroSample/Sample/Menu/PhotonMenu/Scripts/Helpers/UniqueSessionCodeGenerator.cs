using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Photon.Menu
{
  public static class UniqueSessionCodeGenerator
  {
    private const string VALID = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
    private const string REGEX_PATTERN = @"^[A-Z0-9]{6}$";

    public static string GetRandomCode(int length)
    {
      length = Math.Max(0, Math.Min(16, length));

      StringBuilder res = new StringBuilder();
      using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
      {
        while (res.Length != length)
        {
          byte[] bytes = new byte[8];
          provider.GetBytes(bytes);
          foreach (var b in bytes)
          {
            // 238 = highest multiple of 34 in 255
            if (b >= 238 || res.Length == length) continue;

            char character = VALID[b % 34];
            res.Append(character);
          }
        }
      }
      return res.ToString();
    }

    public static bool IsSessionNameValid(string sessionName)
    {
      if (string.IsNullOrEmpty(sessionName))
      {
        return false;
      }

      return Regex.IsMatch(sessionName, REGEX_PATTERN);
    }
  }
}
