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
        // Start is called before the first frame update
        void Start()
        {
            _uIDocument = GetComponent<UIDocument>();

            var root = _uIDocument.rootVisualElement;
            root.Clear();

            var data = CheatPanelUtility.GetCheatMonoDataList();
            root.Add(CheatPanelUtility.CreateCheatPanelView(data));
        }
    }
}
