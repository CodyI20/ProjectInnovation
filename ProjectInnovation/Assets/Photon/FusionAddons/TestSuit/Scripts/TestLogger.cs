using System;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fusion.Addons.TestSuit {
  
  /// <summary>
  /// Used internally for all Fusion tests as the custom Logger
  /// </summary>
  public class TestLogger : ILogger {
    private readonly StringBuilder _builder = new StringBuilder();

    public LogType LogLevel = LogType.Debug;

    void ILogger.Log(LogType logType, object message, in LogContext logContext) {
      var prefix = logContext.Prefix;
      var source = logContext.Source;

      // prefixed logs are allowed conditionally
      if ((int)logType > (int)LogLevel && (string.IsNullOrEmpty(prefix) || prefix == "RealtimeSDK")) {
        return;
      }

      _builder.Append("[Fusion");

      if (!string.IsNullOrEmpty(prefix)) {
        _builder.Append("/");
        _builder.Append(prefix);
      }

      _builder.Append("] ");

      if (source is ILogDumpable dumpable) {
        dumpable.Dump(_builder);
      }

      _builder.Append(message);

      var fullMessage = _builder.ToString();
      _builder.Clear();

      var obj = source as Object;
      switch (logType) {
        case LogType.Error:
          Debug.LogError(fullMessage, obj);
          break;
        case LogType.Warn:
          Debug.LogWarning(fullMessage, obj);
          break;
        default:
          Debug.Log(fullMessage, obj);
          break;
      }
    }

    void ILogger.LogException(Exception ex, in LogContext logContext) {
      Debug.LogException(ex);
    }
  }
}