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
                    List<MethodInfo> cheatMethods = new List<MethodInfo>();
                    foreach (MethodInfo method in methods)
                    {
                        if (method.IsDefined(typeof(CheatMethodAttribute), false))
                            cheatMethods.Add(method);
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
                        cheatMonoDataList.Add(new CheatMonoData(mono, mono.gameObject.name, cheatMethodDatas));
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
                VisualElement methodsContainer = new VisualElement();
                methodsContainer.AddToClassList("methods-container");

                int methodCount = cheatData.Methods.Count;
                for (int m = 0; m < methodCount; m++)
                {
                    CheatMethodData methodData = cheatData.Methods[m];

                    // 각 메소드 UI를 담을 컨테이너 생성
                    VisualElement methodContainer = new VisualElement();
                    methodContainer.style.marginBottom = 5;

                    // 메소드의 파라미터 정보를 가져옵니다.
                    ParameterInfo[] parameters = methodData.Method.GetParameters();
                    // 파라미터와 입력 필드를 함께 저장할 리스트 (나중에 값을 가져오기 위해)
                    List<(ParameterInfo, TextField)> paramFields = new List<(ParameterInfo, TextField)>();

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

                            // 텍스트 입력 필드
                            TextField inputField = new TextField();
                            inputField.value = "";
                            inputField.AddToClassList("param-text-field");
                            paramContainer.Add(inputField);

                            // 파라미터와 입력 필드 매핑 저장
                            paramFields.Add((param, inputField));

                            // 메소드 컨테이너에 파라미터 UI 추가
                            methodContainer.Add(paramContainer);
                        }
                    }

                    // 실행 버튼 생성 (버튼 클릭 시 입력 필드의 값을 가져와 파라미터로 사용)
                    Button invokeButton = new Button(() =>
                    {
                        object[] parameterValues = new object[parameters.Length];
                        bool conversionSuccess = true;
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            string inputValue = paramFields[i].Item2.value;
                            try
                            {
                                // 입력값을 해당 파라미터 타입으로 변환 (예: int, float, string 등)
                                parameterValues[i] = Convert.ChangeType(inputValue, parameters[i].ParameterType);
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
                        separator.style.height = 1;
                        separator.style.backgroundColor = new StyleColor(Color.gray);
                        separator.style.marginBottom = 2;
                        methodsContainer.Add(separator);
                    }
                }
                root.Add(methodsContainer);
            }

            return root;
        }
    }
}
