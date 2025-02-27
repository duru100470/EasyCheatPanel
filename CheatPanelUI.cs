using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyCheatPanel
{
    [RequireComponent(typeof(UIDocument))]
    public class CheatPanelUI : MonoBehaviour
    {
        private UIDocument _uIDocument;
        private VisualElement _root;


        void Awake()
        {
            _uIDocument = GetComponent<UIDocument>();
            _root = _uIDocument.rootVisualElement;

            var closeBtn = _root.Q<Button>("close-btn");
            closeBtn.clicked += () => gameObject.SetActive(false);

            var refreshBtn = _root.Q<Button>("refresh-btn");
            refreshBtn.clicked += LoadPanel;

            LoadPanel();
        }

        void LoadPanel()
        {
            var panel = _root.Q<VisualElement>("panel");
            panel.Clear();

            var data = CheatPanelUtility.GetCheatMonoDataList();
            panel.Add(CheatPanelUtility.CreateCheatPanelView(data));
        }
    }
}
