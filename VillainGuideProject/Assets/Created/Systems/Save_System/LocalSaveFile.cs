using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// The files in Saves/ are independent of the machine, account and installation.
internal static class LocalSaveFile
{
    private const int CurrentVersion = 1;
    private static string DirectoryPath => Path.Combine(Application.persistentDataPath, "Saves");
    private static string Slot => Path.Combine(DirectoryPath, "game.json");
    private static string Backup => Slot + ".bak";
    public static bool Exists => File.Exists(Slot) || File.Exists(Backup);

    [Serializable]
    private sealed class SaveDocument
    {
        public int version;
        public string checksum;
        public string payload;
    }

    private static string Checksum(string payload)
    {
        using var hash = SHA256.Create();
        return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(payload)));
    }

    public static bool TryLoad(out GameProgress progress, out bool fromBackup)
    {
        fromBackup = false;
        progress = null;
        try
        {
            if (Read(Slot, out progress)) return true;
            fromBackup = Read(Backup, out progress);
            return fromBackup;
        }
        catch (NotSupportedException exception)
        {
            // Loading an older backup here could overwrite a save from a newer build.
            Debug.LogWarning(exception.Message);
            return false;
        }
    }

    private static bool Read(string path, out GameProgress progress)
    {
        progress = null;
        if (!File.Exists(path)) return false;
        try
        {
            var document = JsonUtility.FromJson<SaveDocument>(File.ReadAllText(path));
            if (document != null && document.version > CurrentVersion)
                throw new NotSupportedException("This save was written by a newer game version.");

            if (document == null || document.version != CurrentVersion ||
                string.IsNullOrEmpty(document.payload) || document.checksum != Checksum(document.payload))
                throw new InvalidDataException("Invalid save header or checksum.");

            progress = JsonUtility.FromJson<GameProgress>(document.payload);
            if (progress?.gameplay == null || progress.quests == null)
                throw new InvalidDataException("Incomplete game progress.");
            return true;
        }
        catch (Exception exception) when (exception is IOException || exception is ArgumentException ||
                                          exception is UnauthorizedAccessException || exception is InvalidDataException)
        {
            Debug.LogWarning($"Could not read '{path}': {exception.Message}");
            progress = null;
            return false;
        }
    }

    public static void Write(GameProgress progress, bool startNewSlot, bool preserveBackup)
    {
        string payload = JsonUtility.ToJson(progress);
        string json = JsonUtility.ToJson(new SaveDocument
        {
            version = CurrentVersion,
            checksum = Checksum(payload),
            payload = payload
        });
        Directory.CreateDirectory(DirectoryPath);
        // Install a complete file on the same volume. A failed write leaves the old slot intact.
        ReplaceFile(Slot, json, startNewSlot || preserveBackup ? null : Backup);
        if (startNewSlot || !File.Exists(Backup))
            ReplaceFile(Backup, json, null);
    }

    private static void ReplaceFile(string path, string contents, string backupPath)
    {
        string temporary = path + ".tmp";
        using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
        {
            writer.Write(contents);
            writer.Flush();
            stream.Flush(true);
        }

        if (File.Exists(path))
            File.Replace(temporary, path, backupPath);
        else
            File.Move(temporary, path);
    }
}
