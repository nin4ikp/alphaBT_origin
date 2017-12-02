#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class BM_AndroidBuildPrepartion : IPreprocessBuild
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        // Do the preprocessing here
        /*
        string[] allfiles = AssetDatabase.GetSubFolders("Assets");//Directory.GetFiles("Assets", "*" , SearchOption.AllDirectories);

        foreach( var file in allfiles)
        {
            FileInfo info = new FileInfo(file);
            Debug.Log(info.FullName);
        }*/
        Directory.CreateDirectory("Assets/StreamingAssets/");
        string text = File.ReadAllText("Assets/all_paths.csv");
        string[] splittedText = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        for (int i = 0; i < splittedText.Length -1; i++)
        {
            string[] fileEntries = Directory.GetFiles(splittedText[i], "*.*");
            using (StreamWriter sw = new StreamWriter("Assets/StreamingAssets/streamingdata.txt", true))
            {
                foreach (string filename in fileEntries)
                {
                    if (Path.GetExtension(filename) != "meta" && Path.GetExtension(filename) != "mat")
                    {
                        if (!(Path.GetFileNameWithoutExtension(filename).Contains(".")))
                        {
                            //Debug.Log(Path.GetFileNameWithoutExtension(filename));
                            sw.WriteLine(Path.GetFileNameWithoutExtension(filename));
                        }
                    }
                }
            }
        }
    }
}
#endif