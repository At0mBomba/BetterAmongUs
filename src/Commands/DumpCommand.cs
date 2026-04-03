using BetterAmongUs.Attributes;
using BetterAmongUs.Data;
using BetterAmongUs.Modules;

namespace BetterAmongUs.Commands;

[RegisterCommand]
internal sealed class DumpCommand : BaseCommand
{
    internal override string Name => "dump";
    internal override string Description => "Dump the entire log to the user's desktop";

    internal override bool CanRunCommand(out string reason)
    {
        if (GameState.IsInGamePlay)
        {
            reason = "Only can run in lobby";
            return false;
        }

        return base.CanRunCommand(out reason);
    }

    private static string DecryptLog(string log)
    {
        string newLog = string.Empty;
        string[] logArray = log.Split([Environment.NewLine], StringSplitOptions.None);
        foreach (string text in logArray)
        {
            if (text.Contains("[PrivateLog]"))
            {
                newLog += text.Split(':')[0] + ":" + text.Split(':')[1].Replace("[PrivateLog]", "") + ": " + Encryptor.Decrypt(text.Split(':')[2][1..]) + "\n";
            }
            else
            {
                newLog += text + "\n";
            }
        }
        return newLog;
    }

    internal override void Run()
    {
        string logFilePath = BetterDataManager.Files.logFilePath;
        if (!File.Exists(logFilePath))
        {
            CommandErrorText("Log file not found!");
            return;
        }

        if (!ModInfo.Starlight)
        {
            string log = File.ReadAllText(logFilePath);
            string newLog = DecryptLog(log);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFolderPath = Path.Combine(desktopPath, "BetterLogDumps");
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
            string logFileName = "log-" + BAUPlugin.GetVersionText().Replace(' ', '-').ToLower() + "-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".log";
            string newLogFilePath = Path.Combine(logFolderPath, logFileName);
            File.WriteAllText(newLogFilePath, newLog);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = logFolderPath,
                UseShellExecute = true,
                Verb = "open"
            });
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = newLogFilePath,
                UseShellExecute = true,
                Verb = "open"
            });

            CommandResultText($"Dump logs at <color=#b1b1b1>'{newLogFilePath}'</color>");
        }
        else
        {
            string dataPath = BetterDataManager.Folders.fileFolderPath;
            string logFolderPath = Path.Combine(dataPath, "BetterLogDumps");
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            string androidLogContent = File.ReadAllText(logFilePath);
            string androidNewLog = DecryptLog(androidLogContent);

            string androidLogFileName = "log-" + BAUPlugin.GetVersionText().Replace(' ', '-').ToLower() + "-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".log";
            string androidNewLogFilePath = Path.Combine(logFolderPath, androidLogFileName);
            File.WriteAllText(androidNewLogFilePath, androidNewLog);

            CommandResultText($"Dump logs at <color=#b1b1b1>'{androidNewLogFilePath}'</color>");
        }
    }
}