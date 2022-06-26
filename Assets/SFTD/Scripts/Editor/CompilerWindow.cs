using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CompilerWindow : EditorWindow {
    private string mDialogueFilePath = "";
    private bool mMultiple = false;

    [MenuItem("Window/SFTD/Compile")]
    public static void ShowWindow() {
        EditorWindow window = EditorWindow.GetWindow<CompilerWindow>();
        window.Show();
    }

    private void CompileAll(string path) {
        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++) {
            string file = files[i];
            FileInfo fileInfo = new FileInfo(file);
            if ((fileInfo.Attributes & FileAttributes.Directory) != 0) {
                CompileAll(file);
            }
            else {
                Compiler.CompileDialogue(file);
            }
        }
    }

    private void OnGUI() {
        GUILayout.Space(16);
        bool temp = mMultiple;
        mMultiple = GUILayout.Toggle(mMultiple, "Compile all files in the direcotory");
        if (mMultiple != temp) {
            mDialogueFilePath = "";
        }

        GUILayout.Space(16);
        GUILayout.Label("Dialogue Path:");

        GUILayout.BeginHorizontal();
        {
            mDialogueFilePath = GUILayout.TextField(mDialogueFilePath, new GUILayoutOption[] { GUILayout.MaxWidth(this.maxSize.x * 0.8f) });
            if (GUILayout.Button("Browse...")) {
                if (mMultiple) {
                    mDialogueFilePath = EditorUtility.OpenFolderPanel("Select Dialogue Path", "Assets", "Assets");
                }
                else {
                    mDialogueFilePath = EditorUtility.OpenFilePanel("Select Dialogue Path", "Assets", "dialogue");
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8);
        if (GUILayout.Button("Compile")) {
            if (mDialogueFilePath == null || mDialogueFilePath.Length == 0) {
                Debug.LogError("Please select the file or directory.");
            }
            else {
                if (mMultiple) {
                    CompileAll(mDialogueFilePath);
                }
                else {
                    Compiler.CompileDialogue(mDialogueFilePath);
                }
            }
        }
    }
}
