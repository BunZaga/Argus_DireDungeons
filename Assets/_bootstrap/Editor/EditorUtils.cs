#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
public static class PurgeLocalDataUtil
{
    [MenuItem("Zaga/Purge Local Data")]
    static public void PurgeLocalData()
    {
        PlayerPrefs.DeleteAll();
        PurgeData(Application.persistentDataPath);
    }

    public static void PurgeData(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Debug.LogWarningFormat("Data not found in {0}.", folder);
            return;
        }
 
        Directory.Delete(folder, true);

        if (!Directory.Exists(folder))
        {
            Debug.LogWarningFormat("Deleted all data in {0}.", folder);
            return;
        }
        Debug.LogErrorFormat("Could not delete all data in {0}.", folder);
    }
}
#endif