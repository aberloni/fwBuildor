using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveFileSystemWindows : SaveFileSystem
{

    public override bool checkFolderExists(string path)
    {

        if (path == null)
        {
            Debug.LogError("empty path given");
            return false;
        }

        path = path.Trim();

        if (path.Length < 3)
        {
            Debug.LogError("path is too small ? " + path.Length);
            return false;
        }

        //TiniesLogs.logSave(path);

        if (path.Contains("."))
        {
            // folder path from full file path
            path = path.Substring(0, path.LastIndexOf("/"));
        }

        if (!Directory.Exists(path))
        {
            Debug.Log("creating folder (" + path.Length + ") @" + path);
            Directory.CreateDirectory(path);
        }

        return true;
    }

    public override bool writeBytes(string path, byte[] bytes)
    {
        if (bytes == null)
        {
            Debug.LogError("bytes[] is null ?");
            return false;
        }

        checkFolderExists(path);
        File.WriteAllBytes(path, bytes);
        return true;
    }

    override public bool readBytes(string path, out byte[] result)
    {
        checkFolderExists(path);

        //result = new byte[];
        try
        {
            result = File.ReadAllBytes(path);
        }
        catch
        {
            Debug.LogWarning("can't read bytes of " + path);
            result = null;
        }

        return result != null;
    }

    public override bool delete(string path)
    {
        File.Delete(path);
        return true;
    }

    public override bool hasPhysicalFile(string absPath)
    {
        return System.IO.File.Exists(absPath);
    }

}
