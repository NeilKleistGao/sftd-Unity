using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CompilerWindow : EditorWindow {
    private string mDialogueFilePath = "";
    private bool mMultiple = false;

    [MenuItem("Window/SFTD/Compile")]
    public static void ShowWindow() {
        EditorWindow window = EditorWindow.GetWindow<CompilerWindow>();
        window.Show();
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
                    // TODO:
                }
                else {
                    Compiler.CompileDialogue(mDialogueFilePath);
                }
            }
        }
    }
}
