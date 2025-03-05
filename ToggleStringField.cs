using UnityEngine.UIElements;

namespace EasyCheatPanel
{
    public class ToggleStringField : BaseField<string>
    {
        private Toggle _toggle;

        public ToggleStringField(string label = null) : base(label, null)
        {
            // Toggle 생성
            _toggle = new Toggle();
            // 초기값 설정 (필요시 기본값 "false"를 문자열로 표현)
            value = _toggle.value.ToString();

            // Toggle의 값이 변경될 때 문자열로 변환하여 this.value에 할당하고 알림 전파
            _toggle.RegisterValueChangedCallback(evt =>
            {
                // 새로운 bool값을 문자열로 변환
                string newValue = evt.newValue.ToString();
                // BaseField의 value를 갱신 (알림 없이)
                SetValueWithoutNotify(newValue);
            });

            // 내부에 Toggle을 자식으로 추가
            Add(_toggle);
        }

        // 외부에서 값 설정 시, 문자열을 bool로 변환하여 Toggle에 반영
        public override void SetValueWithoutNotify(string newValue)
        {
            base.SetValueWithoutNotify(newValue);
            if (bool.TryParse(newValue, out bool boolValue))
            {
                _toggle.SetValueWithoutNotify(boolValue);
            }
        }
    }
}
