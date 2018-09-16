// Inspired by http://forum.unity3d.com/threads/automatic-version-increment-script.144917/

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class IncrementVersionOnSave : UnityEditor.AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        if (paths.Length == 1)
        {
            if (AssetDatabase.LoadAssetAtPath<VersionIncrementor>(paths[0]) != null)
                return paths;            
        }
        if (VersionIncrementor.Instance.IncreaseBuildNumberOnSave)
        {
            VersionIncrementor.Instance.BuildVersion++;
            VersionIncrementor.Instance.UpdateVersionNumber();
        }

        return paths;
    }
}

[InitializeOnLoad]
public class VersionIncrementor : ResourceSingleton<VersionIncrementor>
{
    public int MajorVersion;
    public int MinorVersion = 1;
    public int BuildVersion;
    public string CurrentVersion;
    public bool IncreaseBuildNumberOnSave = true;

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("Build v" + Instance.CurrentVersion);
        IncreaseBuild();
    }

    private void IncrementVersion(int majorIncr, int minorIncr, int buildIncr)
    {
        MajorVersion += majorIncr;
        MinorVersion += minorIncr;
        BuildVersion += buildIncr;

        UpdateVersionNumber();
    }

    [MenuItem("Build/Create Version File")]
    private static void Create()
    {
        Instance.Make();
    }

    [MenuItem("Build/Increase Major Version")]
    private static void IncreaseMajor()
    {
        Instance.MajorVersion++;
        Instance.MinorVersion = 0;
        Instance.BuildVersion = 0;
        Instance.UpdateVersionNumber();
    }

    [MenuItem("Build/Increase Minor Version")]
    private static void IncreaseMinor()
    {
        Instance.MinorVersion++;
        Instance.BuildVersion = 0;
        Instance.UpdateVersionNumber();
    }

    private static void IncreaseBuild()
    {
        Instance.BuildVersion++;
        Instance.UpdateVersionNumber();
    }

    public void UpdateVersionNumber()
    {
        //Make your custom version layout here.
        CurrentVersion = MajorVersion.ToString("0") + "." + MinorVersion.ToString("00") + "." +
                         BuildVersion.ToString("000");

        PlayerSettings.Android.bundleVersionCode = MajorVersion * 10000 + MinorVersion * 1000 + BuildVersion;
        PlayerSettings.bundleVersion = CurrentVersion;
    }
}

public abstract class ResourceSingleton<T> : ScriptableObject
    where T : ScriptableObject
{
    private static T m_Instance;
    private const string AssetPath = "Assets/Resources";

    public static T Instance
    {
        get
        {
            if (ReferenceEquals(m_Instance, null))
            {
                m_Instance = Resources.Load<T>(AssetPath + typeof(T).Name);
#if UNITY_EDITOR
                if (m_Instance == null)
                {
                    //Debug.LogError("ResourceSingleton Error: Fail load at " + "Singletons/" + typeof(T).Name);
                    CreateAsset();
                }
                else
                {
                    //Debug.Log("ResourceSingleton Loaded: " + typeof (T).Name);
                }
#endif
                var inst = m_Instance as ResourceSingleton<T>;
                if (inst != null)
                {
                    inst.OnInstanceLoaded();
                }
            }

            return m_Instance;
        }
    }

    public void Make()
    {
    }

    private static void CreateAsset()
    {
        m_Instance = ScriptableObject.CreateInstance<T>();
        string path = Path.Combine(AssetPath, typeof(T).ToString() + ".asset");
        if (!System.IO.Directory.Exists(AssetPath))
        {
            System.IO.Directory.CreateDirectory(AssetPath);
        }

        AssetDatabase.CreateAsset(m_Instance, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = m_Instance;
    }

    protected virtual void OnInstanceLoaded()
    {
    }
}