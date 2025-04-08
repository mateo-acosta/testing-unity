using UnityEngine;
using UnityEditor;
using TMPro;

namespace BurgerGame.Editor
{
    [CustomEditor(typeof(ReceiptQueueManager))]
    public class ReceiptPrefabCustomizer : UnityEditor.Editor
    {
        private TextMeshProUGUI prefabTitle;
        private string previewTitle = "Order 1";
        private bool showCustomizeTool = false;
        
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();
            
            // Get the target
            ReceiptQueueManager manager = (ReceiptQueueManager)target;
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Receipt Prefab Customization", EditorStyles.boldLabel);
            
            showCustomizeTool = EditorGUILayout.Foldout(showCustomizeTool, "Receipt Title Tool", true);
            
            if (showCustomizeTool)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Field to drag in the TextMeshPro component from the prefab
                prefabTitle = EditorGUILayout.ObjectField("Prefab Title Text", prefabTitle, 
                    typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                
                if (prefabTitle != null)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Preview Title:", EditorStyles.boldLabel);
                    previewTitle = EditorGUILayout.TextField("Title", previewTitle);
                    
                    EditorGUILayout.Space(5);
                    if (GUILayout.Button("Apply to Prefab"))
                    {
                        // Update the prefab text
                        prefabTitle.text = previewTitle;
                        
                        // Mark the prefab as dirty
                        EditorUtility.SetDirty(prefabTitle);
                        
                        // If this is part of a prefab, mark the prefab asset dirty too
                        PrefabUtility.RecordPrefabInstancePropertyModifications(prefabTitle);
                        
                        Debug.Log($"Updated prefab title to: {previewTitle}");
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "Drag the Title TextMeshPro component from your receipt prefab here to customize it.", 
                        MessageType.Info);
                }
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Order Number Settings", EditorStyles.boldLabel);
            
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField($"Current Order Number: {manager.GetCurrentOrderNumber()}");
                
                EditorGUILayout.BeginHorizontal();
                int nextNumber = EditorGUILayout.IntField("Set Next Order Number", 
                    manager.GetCurrentOrderNumber() + 1);
                if (GUILayout.Button("Apply", GUILayout.Width(60)))
                {
                    manager.SetNextOrderNumber(nextNumber);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode to manage order numbers.", 
                    MessageType.Info);
            }
        }
    }
} 