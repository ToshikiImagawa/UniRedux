using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorExWindow : EditorWindow
{
    [MenuItem("Window/EditorEx")]
    static void Open()
    {
        EditorWindow.GetWindow<EditorExWindow>("EditorEx");
    }

    bool toggle = false;
    string textField = "";
    string textArea = "";
    string password = "";
    float horizontalScrollbar = 0.0f;
    float verticalScrollbar = 0.0f;
    float horizontalSlider = 0.0f;
    float verticalSlider = 0.0f;
    int toolbar = 0;
    int selectionGrid = 0;

    void OnGUI()
    {
        EditorGUILayout.LabelField("ようこそ！　Unityエディタ拡張の沼へ！"); // せっかくなので残しておこう。

        GUILayout.Label("Label : GUILayoutはUnityEngine側なので、ランタイムでもそのまま使える系");

        if (GUILayout.Button("Button"))
            Debug.Log("Button!");

        if (GUILayout.RepeatButton("RepeatButton"))
            Debug.Log("RepeatButton!");

        toggle = GUILayout.Toggle(toggle, "Toggle");

        GUILayout.Label("TextField");
        textField = GUILayout.TextField(textField);

        GUILayout.Label("TextArea");
        textArea = GUILayout.TextArea(textArea);

        GUILayout.Label("PasswordField");
        password = GUILayout.PasswordField(password, '*');

        GUILayout.Label("HorizontalScrollbar");
        float horizontalSize = 10.0f;// sizeはバーのサイズ(0～100のスクロールバーで10なので、全体に対して10分の1サイズ)
        horizontalScrollbar = GUILayout.HorizontalScrollbar(horizontalScrollbar, horizontalSize, 0.0f, 100.0f);

        GUILayout.Label("VerticalScrollbar");
        float verticalSize = 10.0f;// sizeはバーのサイズ(0～100のスクロールバーで10なので、全体に対して10分の1サイズ)
        verticalScrollbar = GUILayout.VerticalScrollbar(verticalScrollbar, verticalSize, 0.0f, 100.0f);

        GUILayout.Label("HorizontalSlider");
        horizontalSlider = GUILayout.HorizontalSlider(horizontalSlider, 0.0f, 100.0f);

        GUILayout.Label("VerticalSlider");
        verticalSlider = GUILayout.VerticalSlider(verticalSlider, 0.0f, 100.0f);

        GUILayout.Label("Toolbar");
        toolbar = GUILayout.Toolbar(toolbar, new string[] { "Tool1", "Tool2", "Tool3" });

        GUILayout.Label("SelectionGrid");
        selectionGrid = GUILayout.SelectionGrid(selectionGrid, new string[] { "Grid 1", "Grid 2", "Grid 3", "Grid 4" }, 2);

        GUILayout.Box("Box");

        GUILayout.Label("ここからSpace");
        GUILayout.Space(100);
        GUILayout.Label("ここまでSpace");

        GUILayout.Label("ここからFlexibleSpace");
        GUILayout.FlexibleSpace();
        GUILayout.Label("ここまでFlexibleSpace");
    }
}