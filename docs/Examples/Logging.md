# Logging
To log messages, you can use the `Log` class.

Logs are written to the console if the log level for strackers loader permits it.
Additionally, all logs are written to `nativePC/plugins/SharpPluginLoader.log`.

```csharp
// Log a message with the default log level
Log.Info("Hello World!");

// Log a debug message
Log.Debug("My message that people won't see unless they enable debug logs");

// Log a warning
Log.Warn("Something went wrong!");

// Log an error
Log.Error("Something went horribly wrong!");
```