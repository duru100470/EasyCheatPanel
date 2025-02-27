using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EasyCheatPanel.Editor
{
    public class CheatPanelWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;
        [SerializeField] private StyleSheet _styleSheet;

        [MenuItem("Tools/Cheat Panel")]
        private static void ShowWindow()
        {
            CheatPanelWindow window = GetWindow<CheatPanelWindow>();
            window.titleContent = new GUIContent("Cheat Panel");
        }

        private void OnEnable()
        {
            VisualElement root = rootVisualElement;
            root.Clear();

            if (_styleSheet != null)
            {
                root.styleSheets.Add(_styleSheet);
            }

            var data = CheatPanelUtility.GetCheatMonoDataList();
            root.Add(CheatPanelUtility.CreateCheatPanelView(data));
        }
    }
}
