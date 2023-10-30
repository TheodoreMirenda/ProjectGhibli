using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace TJ.Utilities
{
public static class JSONFileHandler
{
    public static void SaveToJSON<T> (T toSave, string filename, bool local)
    {
        string content = JsonUtility.ToJson (toSave);
        WriteFile (local ? GetPathLocal(filename):GetPath(filename), content);
    }
    public static void SaveToJSONFormatted<T> (T toSave, string filename, bool local, List<string> stringsToRemove)
    {
        string content = JsonUtility.ToJson (toSave);
        foreach(string partToRemove in stringsToRemove)
            content = content.Replace(partToRemove, "");

        // remove all instances of "name" from the string
        WriteFile (local ? GetPathLocal(filename):GetPath(filename), content);
    }
    public static T ReadListFromJSON<T> (string filename)
    {
        string content = ReadFile (GetPathLocal (filename));
        if(content=="") {
            Debug.Log($"File {filename} not found");
            return default;
        }
        return JsonUtility.FromJson<T>(content);
    }
    private static string GetPath (string filename)
    {
        return Application.persistentDataPath + "/" + filename;
    }
    private static string GetPathLocal (string filename)
    {
        return Application.dataPath + "/" + filename;
    }
    private static void WriteFile (string path, string content)
    {
        FileStream fileStream = new (path, FileMode.Create);
        using StreamWriter writer = new (fileStream);
        writer.Write(content);
    }
    private static string ReadFile (string path)
    {
        if (File.Exists (path))
        {
            using StreamReader reader = new StreamReader(path);
            string content = reader.ReadToEnd();
            return content;
        }
        return "";
    }

}
}

