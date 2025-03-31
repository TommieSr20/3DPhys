using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.RemoteConfig.Editor.UIComponents
{
    internal class SettingsJsonModal : EditorWindow
    {
        public string currentText;
        public string keyEntityId;
        private TextAsset currentTxtAsset;
        private GameObject currentGameObject;
        private ScriptableObject currentScriptableObject;
        private Vector2 scroll;
        private Object source;
        private bool jsonStringValid;
        private Type parentWindowType;
        private static SettingsJsonModal _instance;
        private SerializedObject serializedObject;
        private int lastSelectedIndex;

        private JArray schemas;
        public string currentSchemaId;
        public string currentSchemaTitle;

        public static SettingsJsonModal CreateInstance(string jsonString = "", string keyName = "", string jsonKeyEntityId = "", string schemaId = null, Type parentType = null)
        {
            if (_instance == null)
            {
                _instance = ScriptableObject.CreateInstance<SettingsJsonModal>();
                _instance.Init(jsonString , keyName, jsonKeyEntityId, schemaId, parentType);
            }
            return _instance;
        }

        public void Init(string jsonString, string keyName, string jsonKeyEntityId, string schemaId, Type parentType)
        {
            titleContent.text = "JSON Editor for key: " + keyName;
            currentText = FormatJson(jsonString);
            minSize = new Vector2(480, 480);
            keyEntityId = jsonKeyEntityId;
            parentWindowType = parentType;

            schemas = RemoteConfigDataStore.schemas;
            currentSchemaId = schemaId;
            currentSchemaTitle = getSchemaTitle(currentSchemaId);

            ShowUtility();
        }

        private void OnGUI()
        {
            var componentDropDownHeight = 0;

            source = EditorGUILayout.ObjectField("  Select JSON object:", source, typeof(Object), true);
            if (source)
            {
                if (source is TextAsset selectedTxtAsset)
                {
                    if (selectedTxtAsset != currentTxtAsset)
                    {
                        currentTxtAsset = selectedTxtAsset;
                        currentText = FormatJson(currentTxtAsset.text);
                    }
                }
                else if (source is ScriptableObject selectedScriptableObject)
                {
                    if (selectedScriptableObject != currentScriptableObject)
                    {
                        currentScriptableObject = selectedScriptableObject;
                        currentText = FormatJson(JsonUtility.ToJson(currentScriptableObject));
                    }
                }
                else if (source is GameObject selectedGameObject)
                {
                    componentDropDownHeight = (int) EditorStyles.popup.fixedHeight;
                    var components = FilterCustomGameObjects(selectedGameObject.GetComponents<Component>());
                    if (selectedGameObject != currentGameObject)
                    {
                        currentGameObject = selectedGameObject;
                        lastSelectedIndex = -1;
                    }

                    var options = new List<string>();
                    for (var i = 0; i < components.Length; i++)
                    {
                        options.Add(components[i].GetType().ToString());
                    }

                    var choiceIndex = lastSelectedIndex == -1 ?
                        EditorGUILayout.Popup(0, options.ToArray()) :
                        EditorGUILayout.Popup(lastSelectedIndex, options.ToArray());

                    var defaultComponentText = "{}";
                    if (components.Length > 0)
                    {
                        defaultComponentText = FormatJson(JsonUtility.ToJson(components[choiceIndex].GetComponent(options[choiceIndex])));
                        if (choiceIndex != lastSelectedIndex)
                        {
                            currentText = defaultComponentText;
                            lastSelectedIndex = choiceIndex;
                        }
                    }
                    else
                    {
                        currentText = defaultComponentText;
                    }
                }
            }

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(position.width-20), GUILayout.Height(position.height-60));
            EditorStyles.textField.wordWrap = false;
            currentText = EditorGUILayout.TextArea(currentText, GUILayout.Height(position.height - 70 - componentDropDownHeight));
            currentText = FormatJson(currentText);
            EditorGUILayout.EndScrollView();

            DrawButtons(position.height - 30);
        }

        private void DrawButtons(float currentY)
        {
            var boundingBoxPadding = 3f;
            var paddedRect = new Rect(position.width - (position.width / 3) - 6, currentY, ((position.width / 3)) - 10,
                20);
            var buttonWidth = (paddedRect.width / 3);

            var validJsonIcon = jsonStringValid ? EditorGUIUtility.FindTexture("d_winbtn_mac_max") : EditorGUIUtility.FindTexture("d_winbtn_mac_close");
            GUI.Label(new Rect(boundingBoxPadding, paddedRect.y, 75, 20), "JSON valid:");
            GUI.DrawTexture(new Rect(boundingBoxPadding + 75, paddedRect.y+3, 12, 12), validJsonIcon);

            Rect ddRect = new Rect(boundingBoxPadding + 105, paddedRect.y+3, buttonWidth+100, 20);
            var tooltip = getSchemaValue(currentSchemaId);
            if (GUI.Button(ddRect, new GUIContent(currentSchemaTitle, tooltip), EditorStyles.popup))
            {
                BuildPopupListForSchemas().DropDown(ddRect);
            }

            if (GUI.Button(new Rect(paddedRect.x, paddedRect.y, buttonWidth, 20), "Format"))
            {
                currentText = FormatJson(currentText);
                GUI.FocusControl(null);
            }

            if (GUI.Button(new Rect(paddedRect.x + buttonWidth + boundingBoxPadding, paddedRect.y, buttonWidth, 20),
                "Cancel"))
            {
                Close();
            }

            EditorGUI.BeginDisabledGroup(!jsonStringValid);
            if (GUI.Button(
                new Rect(paddedRect.x + 2 * (buttonWidth + boundingBoxPadding), paddedRect.y, buttonWidth, 20),
                "Submit"))
            {
                var parentWindow = GetParentWindow(parentWindowType);
                if (parentWindow)
                {
                    parentWindow.SendEvent(EditorGUIUtility.CommandEvent("JsonSubmitted"));
                }
                EditorGUI.EndDisabledGroup();
                Close();
            }
            EditorGUI.EndDisabledGroup();
        }

        private EditorWindow GetParentWindow(Type parentWindowType)
        {
            var pWindows = parentWindowType == null ? Resources.FindObjectsOfTypeAll<RemoteConfigWindow>() : Resources.FindObjectsOfTypeAll(parentWindowType);
            if (pWindows != null && pWindows.Length > 0)
            {
                return (EditorWindow)pWindows[0];
            }
            Debug.LogWarning("Content was not submitted - parent window not found");
            return null;
        }

        private Component[] FilterCustomGameObjects (Component[] components)
        {
            var componentsList = new List<Component>();
            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is MonoBehaviour component)
                {
                    var componentType = component.GetType().ToString();
                    if (!componentType.Contains("UnityEngine."))
                    {
                        componentsList.Add(component);
                    }
                }
            }
            return componentsList.ToArray();;
        }

        private string FormatJson(string jsonString)
        {
            try
            {
                jsonStringValid = true;
                return JToken.Parse(jsonString).ToString(Formatting.Indented);
            }
            catch
            {
                jsonStringValid = false;
                return jsonString;
            }
        }

        private string getSchemaValue(string schemaId)
        {
            if (schemaId == null)
            {
                return "No Schema Value";
            }

            foreach (var item in schemas)
            {
                if (item["id"].ToString().Equals(schemaId))
                {
                    return item["value"].ToString();
                }
            }

            return "No Schema Value";
        }

        private string getSchemaTitle(string schemaId)
        {
            if (schemaId == null)
            {
                return "No Schema";
            }

            foreach (var item in schemas)
            {
                if (item["id"].ToString().Equals(schemaId))
                {
                    if (item["value"]["title"] == null)
                    {
                        return schemaId;
                    }
                    return item["value"]["title"].ToString();
                }
            }

            return "No Schema";
        }

        private string getSchemaId(string schemaTitle)
        {
            if (string.IsNullOrEmpty(schemaTitle) || schemaTitle.Equals("No Schema"))
            {
                return null;
            }

            foreach (var item in schemas)
            {
                if ((item["value"]["title"] != null) && item["value"]["title"].ToString().Equals(schemaTitle))
                {
                    return item["id"].ToString();
                }
            }

            return schemaTitle;
        }

        private GenericMenu BuildPopupListForSchemas()
        {
            var menu = new GenericMenu();

            var popUpSchemas = new JArray();
            var firstElement = new JObject();
            firstElement.Add("id", null);
            popUpSchemas.Add(firstElement);
            popUpSchemas.Merge(schemas);

            foreach (var item in popUpSchemas)
            {
                string schemaId = item["id"].ToString();
                string schemaTitle = getSchemaTitle(schemaId);
                menu.AddItem(new GUIContent(schemaTitle), currentSchemaTitle.Equals(schemaTitle), OnSchemaSelected, schemaTitle);
            }

            return menu;
        }

        private void OnSchemaSelected(object obj)
        {
            currentSchemaTitle = obj.ToString();
            currentSchemaId = getSchemaId(currentSchemaTitle);
        }
    }
}
    