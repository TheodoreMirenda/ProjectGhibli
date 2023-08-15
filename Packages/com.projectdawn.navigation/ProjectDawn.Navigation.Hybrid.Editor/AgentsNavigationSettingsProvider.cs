using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace ProjectDawn.Navigation.Hybrid.Editor
{
    static class AgentsNavigationSettingsProvider
    {
        static class Styles
        {
            public static readonly GUIContent SonarTimeHorizon = EditorGUIUtility.TrTextContent("Sonar Time Horizon", "Changes sonar avoidance radius to be based on velocity and also navmesh collision velocity accounts collision.");
            public static readonly GUIStyle lineStyle = new GUIStyle();
            public static readonly GUIStyle centerStyle = new GUIStyle();

            static Styles()
            {
                centerStyle.alignment = TextAnchor.MiddleCenter;

                // Initialize the line style
                lineStyle = new GUIStyle();
                lineStyle.normal.background = EditorGUIUtility.whiteTexture; // Use a white texture as the line color
                lineStyle.margin = new RectOffset(0, 0, 4, 4); // Add some margin to the line
            }
        }

        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/AgentsNavigation", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Agents Navigation",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.BeginVertical();

                    GUILayout.Space(10);

                    DrawHorizontalLine();

                    using (new EditorGUI.DisabledScope(Application.isPlaying))
                    {
                        foreach (var type in SettingsBehaviour.Types)
                        {
                            DrawHeaderGUILayout(type);

                            var settings = GameObject.FindAnyObjectByType(type);

                            if (settings != null)
                            {
                                var editor = UnityEditor.Editor.CreateEditor(settings);
                                editor.OnInspectorGUI();
                            }
                            else
                            {
                                EditorGUILayout.HelpBox($"Failed to find singleton component of type {type.Name} in the current scene.", MessageType.Warning);
                            }

                            DrawHorizontalLine();
                        }
                    }

                    GUILayout.Space(10);

                    ScriptingDefineToggleField.Draw(Styles.SonarTimeHorizon, "EXPERIMENTAL_SONAR_TIME");
                    EditorGUILayout.HelpBox($"This feature should result better sonar avoidance, but for now it is experimental! Make sure commit changes, before turning it on.", MessageType.Info);

                    GUILayout.Space(10);

                    EditorGUILayout.EndVertical();
                },
            };

            return provider;
        }

        static void DrawHeaderGUILayout(System.Type type, float height = 20)
        {
            string[] scriptAsset = AssetDatabase.FindAssets(type.Name + " t:MonoScript");
            string scriptPath = AssetDatabase.GUIDToAssetPath(scriptAsset[0]);
            var settings = AssetDatabase.LoadAssetAtPath(scriptPath, typeof(UnityEngine.Object));

            Rect r = GUILayoutUtility.GetRect(0, height);

            r.x += 5;
            r.width -= 5;

            Rect iconRect = new Rect(r.x, r.y, height, height);
            GUI.Label(iconRect, AssetPreview.GetMiniThumbnail(settings), Styles.centerStyle);

            Rect titleRect = new Rect(r.x + height + 5, r.y, r.width - height, r.height);
            GUI.Label(titleRect, ExtractComponentName(type), EditorStyles.largeLabel);

            DrawHorizontalLine();
        }

        static void DrawHorizontalLine()
        {
            Rect lineRect = GUILayoutUtility.GetRect(GUIContent.none, Styles.lineStyle, GUILayout.Height(1)); // Set the height of the line to 1

            // Check the current skin and adjust the line color accordingly
            if (EditorGUIUtility.isProSkin)
                GUI.color = new Color(0.10196f, 0.10196f, 0.10196f, 1); // For light skin, use a darker gray color
            else
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 1f); // For dark skin, use a slightly darker gray color

            // Draw the line
            GUI.Box(lineRect, GUIContent.none, Styles.lineStyle);

            // Reset the GUI color
            GUI.color = Color.white;
        }

        static string ExtractComponentName(System.Type componentType)
        {
            var attributes = componentType.GetCustomAttributes(typeof(AddComponentMenu), true);

            if (attributes.Length > 0)
            {
                var addComponentMenuAttribute = attributes[0] as AddComponentMenu;
                if (addComponentMenuAttribute != null)
                {
                    string path = addComponentMenuAttribute.componentMenu;
                    string[] menuItems = path.Split('/');
                    string componentName = menuItems.LastOrDefault();

                    return componentName;
                }
            }

            return ObjectNames.NicifyVariableName(componentType.Name);
        }
    }
}
