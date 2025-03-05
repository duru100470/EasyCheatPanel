using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyCheatPanel
{
    public static class CheatPanelUtility
    {
        public static List<CheatMonoData> GetCheatMonoDataList()
        {
            List<CheatMonoData> cheatMonoDataList = new List<CheatMonoData>();

            // 현재 로드된 모든 어셈블리를 순회합니다.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }
                if (types == null)
                    continue;

                foreach (Type type in types)
                {
                    if (type == null)
                        continue;

                    // MonoBehaviour를 상속받고 있고, 추상 클래스가 아니어야 함
                    if (!typeof(MonoBehaviour).IsAssignableFrom(type) || type.IsAbstract)
                        continue;

                    // 해당 타입의 인스턴스 메소드 중 CheatMethodAttribute가 붙은 메소드를 찾습니다.
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    List<MethodInfo> cheatMethods = new();
                    List<CustomPanelData> customs = new();
                    foreach (MethodInfo method in methods)
                    {
                        if (method.IsDefined(typeof(CheatMethodAttribute), false))
                        {
                            cheatMethods.Add(method);
                            continue;
                        }

                        if (method.IsDefined(typeof(CustomCheatPanelAttribute), false))
                            customs.Add(new CustomPanelData(method));
                    }

                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    List<CheatFieldData> cheatFields = new();

                    foreach (FieldInfo field in fields)
                    {
                        if (field.IsDefined(typeof(CheatFieldAttribute), false))
                        {
                            CheatFieldAttribute attribute = field.GetCustomAttribute<CheatFieldAttribute>();
                            string displayName = attribute != null ? attribute.Display : "";
                            cheatFields.Add(new CheatFieldData(new FieldAccessor(field), displayName));
                        }
                    }

                    foreach (PropertyInfo property in properties)
                    {
                        if (property.IsDefined(typeof(CheatFieldAttribute), false))
                        {
                            CheatFieldAttribute attribute = property.GetCustomAttribute<CheatFieldAttribute>();
                            string displayName = attribute != null ? attribute.Display : "";
                            cheatFields.Add(new CheatFieldData(new PropertyAccessor(property), displayName));
                        }
                    }

                    if (cheatMethods.Count == 0)
                        continue;

                    // 씬에서 해당 MonoBehaviour 타입의 인스턴스를 검색합니다.
                    UnityEngine.Object[] foundObjects = UnityEngine.Object.FindObjectsOfType(type);
                    if (foundObjects == null || foundObjects.Length == 0)
                        continue;

                    // 찾은 각 인스턴스에 대해 CheatMonoData를 생성합니다.
                    foreach (UnityEngine.Object obj in foundObjects)
                    {
                        MonoBehaviour mono = obj as MonoBehaviour;
                        if (mono == null)
                            continue;

                        List<CheatMethodData> cheatMethodDatas = new List<CheatMethodData>();
                        foreach (MethodInfo method in cheatMethods)
                        {
                            // 어트리뷰트에 설정한 Display 값을 가져옵니다.
                            CheatMethodAttribute attribute = method.GetCustomAttribute<CheatMethodAttribute>();
                            string displayName = attribute != null ? attribute.Display : "";
                            cheatMethodDatas.Add(new CheatMethodData(method, displayName));
                        }
                        cheatMonoDataList.Add(new CheatMonoData(mono, mono.gameObject.name, cheatMethodDatas, cheatFields, customs));
                    }
                }
            }

            return cheatMonoDataList;
        }

        public static VisualElement CreateCheatPanelView(List<CheatMonoData> cheatMonoDatas)
        {
            var root = new ScrollView(ScrollViewMode.Vertical);
            root.Clear();

            // CheatPanelUtility에서 CheatMonoData 리스트를 가져옵니다.
            List<CheatMonoData> cheatDataList = CheatPanelUtility.GetCheatMonoDataList();

            foreach (CheatMonoData cheatData in cheatDataList)
            {
                // 게임오브젝트의 이름을 제목으로 표시 (큰 글씨)
                Label titleLabel = new Label(cheatData.Name);
                titleLabel.AddToClassList("title-label");
                root.Add(titleLabel);

                // 해당 MonoBehaviour의 치트 메소드들을 담을 컨테이너 (세로 배치)
                VisualElement methodsContainer = CreateMethodView(cheatData);
                root.Add(methodsContainer);

                VisualElement fieldsContainer = CreateFieldView(cheatData);
                root.Add(fieldsContainer);

                VisualElement customsContainer = CreateCustomPanelView(cheatData);
                root.Add(customsContainer);
            }

            return root;
        }

        private static VisualElement CreateMethodView(CheatMonoData cheatData)
        {
            VisualElement methodsContainer = new VisualElement();
            methodsContainer.AddToClassList("methods-container");

            int methodCount = cheatData.Methods.Count;
            for (int m = 0; m < methodCount; m++)
            {
                CheatMethodData methodData = cheatData.Methods[m];

                // 각 메소드 UI를 담을 컨테이너 생성
                VisualElement methodContainer = new VisualElement();
                methodContainer.AddToClassList("method-container");

                // 메소드의 파라미터 정보를 가져옵니다.
                ParameterInfo[] parameters = methodData.Method.GetParameters();

                var paramFields = CreateParamView(parameters, methodContainer);

                // 실행 버튼 생성 (버튼 클릭 시 입력 필드의 값을 가져와 파라미터로 사용)
                Button invokeButton = new Button(() =>
                {
                    object[] parameterValues = new object[parameters.Length];
                    bool conversionSuccess = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        object inputValue = paramFields[i].Item2.Value;

                        try
                        {
                            if (inputValue == default && paramFields[i].Item1.IsOptional)
                            {
                                // Optional param일 경우 default값으로
                                parameterValues[i] = paramFields[i].Item1.DefaultValue;
                            }
                            else
                            {
                                // 입력값을 해당 파라미터 타입으로 변환 (예: int, float, string 등)
                                parameterValues[i] = Convert.ChangeType(inputValue, parameters[i].ParameterType);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"파라미터 '{parameters[i].Name}' 변환 실패: {ex.Message}");
                            conversionSuccess = false;
                            break;
                        }
                    }
                    if (conversionSuccess)
                    {
                        methodData.Method.Invoke(cheatData.Instance, parameterValues);
                    }
                })
                {
                    text = methodData.DisplayName
                };
                invokeButton.AddToClassList("invoke-button");
                methodContainer.Add(invokeButton);

                // 메소드 컨테이너를 methodsContainer에 추가
                methodsContainer.Add(methodContainer);

                // 각 메소드 사이에 구분선 추가 (마지막 메소드 이후는 생략)
                if (m < methodCount - 1)
                {
                    VisualElement separator = new VisualElement();
                    separator.AddToClassList("separator");
                    methodsContainer.Add(separator);
                }
            }

            return methodsContainer;
        }

        private static List<(ParameterInfo, IFieldWrapper)> CreateParamView(ParameterInfo[] parameters, VisualElement methodContainer)
        {
            // 파라미터와 입력 필드를 함께 저장할 리스트 (나중에 값을 가져오기 위해)
            List<(ParameterInfo, IFieldWrapper)> paramFields = new();

            // 파라미터가 있다면, 입력 필드들을 생성합니다.
            if (parameters.Length > 0)
            {
                foreach (ParameterInfo param in parameters)
                {
                    // 수평으로 배치할 컨테이너 생성
                    VisualElement paramContainer = new VisualElement();
                    paramContainer.AddToClassList("param-container");

                    // 파라미터 이름 레이블
                    Label paramLabel = new Label(param.Name + ":");
                    paramLabel.AddToClassList("param-label");
                    paramContainer.Add(paramLabel);

                    IFieldWrapper fieldWrapper;

                    if (HasAttribute<DynamicDropdownAttribute>(param, out var dynamicDropdownAttr))
                    {
                        IDropdownProvider provider = (IDropdownProvider)Activator.CreateInstance(dynamicDropdownAttr.ProviderType);

                        // DropdownField 생성 및 어트리뷰트의 object[]를 문자열 리스트로 변환하여 아이템으로 사용
                        DropdownField dropdownField = new DropdownField();
                        List<string> choices = new List<string>();
                        foreach (object item in provider.GetItems())
                        {
                            choices.Add(item.ToString());
                        }
                        dropdownField.choices = choices;
                        if (choices.Count > 0)
                        {
                            dropdownField.value = choices[0];
                        }
                        dropdownField.AddToClassList("param-field");
                        fieldWrapper = new GenericFieldWrapper<string>(dropdownField);
                        paramContainer.Add(dropdownField);
                    }
                    else if (HasAttribute<DropdownAttribute>(param, out var dropdownAttr))
                    {
                        // DropdownField 생성 및 어트리뷰트의 object[]를 문자열 리스트로 변환하여 아이템으로 사용
                        DropdownField dropdownField = new DropdownField();
                        List<string> choices = new List<string>();
                        foreach (object item in dropdownAttr.Items)
                        {
                            choices.Add(item.ToString());
                        }
                        dropdownField.choices = choices;
                        if (choices.Count > 0)
                        {
                            dropdownField.value = choices[0];
                        }
                        dropdownField.AddToClassList("param-field");
                        fieldWrapper = new GenericFieldWrapper<string>(dropdownField);
                        paramContainer.Add(dropdownField);
                    }
                    else if (param.ParameterType == typeof(bool))
                    {
                        // Toggle 생성
                        Toggle toggle = new Toggle();
                        toggle.AddToClassList("param-field");
                        paramContainer.Add(toggle);
                        fieldWrapper = new GenericFieldWrapper<bool>(toggle);
                    }
                    else if (param.ParameterType == typeof(int))
                    {
                        // IntegerField 생성
                        IntegerField intField = new IntegerField();
                        intField.value = 0;
                        intField.AddToClassList("param-field");
                        paramContainer.Add(intField);
                        fieldWrapper = new GenericFieldWrapper<int>(intField);
                    }
                    else if (param.ParameterType == typeof(float))
                    {
                        // IntegerField 생성
                        FloatField floatField = new FloatField();
                        floatField.value = 0;
                        floatField.AddToClassList("param-field");
                        paramContainer.Add(floatField);
                        fieldWrapper = new GenericFieldWrapper<float>(floatField);
                    }
                    else if (param.ParameterType.IsEnum)
                    {
                        // enum 타입의 기본값(첫 번째 값)을 가져옵니다.
                        Enum defaultEnumValue = (Enum)Enum.GetValues(param.ParameterType).GetValue(0);

                        // EnumField를 기본값으로 초기화하여 생성합니다.
                        EnumField enumField = new EnumField(defaultEnumValue);
                        enumField.AddToClassList("param-field");
                        paramContainer.Add(enumField);
                        fieldWrapper = new GenericFieldWrapper<Enum>(enumField);
                    }
                    else
                    {
                        // 기본 TextField 생성
                        TextField textField = new TextField();
                        textField.value = "";
                        textField.AddToClassList("param-field");
                        paramContainer.Add(textField);
                        fieldWrapper = new GenericFieldWrapper<string>(textField);
                    }

                    if (param.IsOptional)
                    {
                        paramContainer.AddToClassList("param-optional-container");
                        fieldWrapper.Value = param.DefaultValue;
                    }

                    // 파라미터와 입력 필드 매핑 저장
                    paramFields.Add((param, fieldWrapper));

                    // 메소드 컨테이너에 파라미터 UI 추가
                    methodContainer.Add(paramContainer);
                }
            }

            return paramFields;
        }

        private static bool HasAttribute<T>(ParameterInfo param, out T attribute) where T : Attribute
        {
            attribute = param.GetCustomAttribute<T>();
            return attribute != null;
        }

        private static VisualElement CreateFieldView(CheatMonoData cheatData)
        {
            VisualElement fieldsContainer = new VisualElement();
            fieldsContainer.AddToClassList("methods-container");

            int methodCount = cheatData.Fields.Count;
            for (int m = 0; m < methodCount; m++)
            {
                CheatFieldData fieldData = cheatData.Fields[m];

                // 각 메소드 UI를 담을 컨테이너 생성
                VisualElement fieldContainer = new VisualElement();
                fieldContainer.AddToClassList("field-container");

                var nameLabel = new Label(fieldData.DisplayName);
                nameLabel.AddToClassList("param-label");
                fieldContainer.Add(nameLabel);

                var valueLabel = new Label(fieldData.Accessor.GetValue(cheatData.Instance).ToString());
                valueLabel.AddToClassList("param-field");
                fieldContainer.Add(valueLabel);

                // 메소드 컨테이너를 methodsContainer에 추가
                fieldsContainer.Add(fieldContainer);

                // 각 메소드 사이에 구분선 추가 (마지막 메소드 이후는 생략)
                if (m < methodCount - 1)
                {
                    VisualElement separator = new VisualElement();
                    separator.AddToClassList("separator");
                    fieldsContainer.Add(separator);
                }
            }

            return fieldsContainer;
        }

        private static VisualElement CreateCustomPanelView(CheatMonoData cheatData)
        {
            VisualElement customsContainer = new VisualElement();
            customsContainer.AddToClassList("customs-container");

            int methodCount = cheatData.CustomPanels.Count;
            for (int m = 0; m < methodCount; m++)
            {
                CustomPanelData customData = cheatData.CustomPanels[m];

                // 각 메소드 UI를 담을 컨테이너 생성
                VisualElement customContainer = new VisualElement();
                customContainer.AddToClassList("custom-container");

                var content = customData.Method.Invoke(cheatData.Instance, null) as VisualElement;
                customContainer.Add(content);

                // 메소드 컨테이너를 methodsContainer에 추가
                customsContainer.Add(customContainer);

                // 각 메소드 사이에 구분선 추가 (마지막 메소드 이후는 생략)
                if (m < methodCount - 1)
                {
                    VisualElement separator = new VisualElement();
                    separator.AddToClassList("separator");
                    customsContainer.Add(separator);
                }
            }

            return customsContainer;
        }
    }
}
