using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public class DebugLaunch : EditorWindow
    {
        private const string SessionKey = "LoadDebugLevel";
        private static LevelDataSO _levelDataSo;
        private static MarbleData _marbleData;
        public List<MarbleData> playerDeck;
        private static DebugLevelData _debugLevelData;
        
        private Vector2 scrollPosition;
        private SerializedObject serializedTarget;
        
        private void Play()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                SessionState.SetBool(SessionKey, true);

                EditorApplication.isPlaying = true;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnPlayModeStateChanged()
        {
            if (SessionState.GetBool(SessionKey, false))
            {
                LoadDebugLevel();
                
                SessionState.SetBool(SessionKey, false);
            }
        }
    
        private static void LoadDebugLevel()
        {
            // load debug level
            Debug.Log("Load debug level");
            DebugLevelData levelData = GetOrCreateDebugLevelData();
            GameObject nodeObject = new GameObject("MapManager");
            DontDestroyOnLoad(nodeObject);
            NodeManager nodeManager = nodeObject.AddComponent<NodeManager>();
            NodeManagerSO debugSaveData = ScriptableObject.CreateInstance<NodeManagerSO>();
            
            LevelDataSO debugLevelInfo = CreateInstance<LevelDataSO>();
            debugLevelInfo.SetLevelInfo(levelData.enemyDifficulty, levelData.enemyAggressionLevel,
                    levelData.enemyDeckType, levelData.enemyName, levelData.arena, levelData.levelDifficulty);
            debugSaveData.GetLevels().Add(debugLevelInfo);
            
            List<MarbleData> debugPlayerDeck = new List<MarbleData>(levelData.playerDeck);
            debugSaveData.UpdatePlayerDeck(debugPlayerDeck);
            
            nodeManager.SetSaveData(debugSaveData);
        }
    
        [MenuItem("Tools/Debug Launch")]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one
            GetWindow<DebugLaunch>("Debug Launch");
        }

        private void CreateGUI()
        {
            _debugLevelData = GetOrCreateDebugLevelData();
            serializedTarget = new SerializedObject(_debugLevelData);
        }

        private void OnGUI()
        {
            serializedTarget?.UpdateIfRequiredOrScript();
            GUILayout.BeginVertical();
            GUILayout.Label("Debug Player Deck", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if(serializedTarget != null)
                EditorGUILayout.PropertyField(serializedTarget.FindProperty(nameof(_debugLevelData.playerDeck)), new GUIContent("Debug Deck"), true);
            EditorGUILayout.EndScrollView();
            
        
            /*
            _marbleData = (MarbleData)EditorGUILayout.ObjectField(
                "Marble:", 
                _marbleData, 
                typeof(MarbleData), 
                false
            );
            
            if (GUILayout.Button("Add Marble To Deck"))
            {
                AddMarbleToDeck();
            }
            */
        
            GUILayout.Label("Debug Level Settings", EditorStyles.boldLabel);
            if (serializedTarget != null)
            {
                EditorGUILayout.PropertyField(serializedTarget.FindProperty(nameof(_debugLevelData.enemyDifficulty)));
                EditorGUILayout.PropertyField(
                    serializedTarget.FindProperty(nameof(_debugLevelData.enemyAggressionLevel)));
                EditorGUILayout.PropertyField(serializedTarget.FindProperty(nameof(_debugLevelData.enemyDeckType)));
                EditorGUILayout.PropertyField(serializedTarget.FindProperty(nameof(_debugLevelData.enemyName)));
                EditorGUILayout.PropertyField(serializedTarget.FindProperty(nameof(_debugLevelData.arena)));
                EditorGUILayout.PropertyField(serializedTarget.FindProperty(nameof(_debugLevelData.levelDifficulty)));
                serializedTarget.ApplyModifiedProperties();
            }

            _levelDataSo = (LevelDataSO)EditorGUILayout.ObjectField(
                "Level:", 
                _levelDataSo, 
                typeof(LevelDataSO), 
                false
            );
            
            if (GUILayout.Button("Load level from data"))
            {
                LoadDebugLevelDataFromAsset();
            }
        
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.Label("Launch Debug Level with Debug Deck", EditorStyles.boldLabel);
            if (GUILayout.Button("Start Game"))
            {
                Play();
            }
        }

        private void LoadDebugLevelDataFromAsset()
        {
            _debugLevelData.enemyDifficulty = _levelDataSo.GetEnemyDifficulty();
            _debugLevelData.enemyAggressionLevel = _levelDataSo.GetAggressionLevel();
            _debugLevelData.enemyDeckType = _levelDataSo.GetEnemyDeckType();
            _debugLevelData.enemyName = _levelDataSo.GetEnemyName();
            _debugLevelData.arena = _levelDataSo.GetArena();
            _debugLevelData.levelDifficulty = _levelDataSo.GetLevelDifficulty();
            
            AssetDatabase.SaveAssets();
        }
        
        private void AddMarbleToDeck()
        {
            _debugLevelData.playerDeck.Add(_marbleData);
            AssetDatabase.SaveAssets();
        }
        
        public static DebugLevelData GetOrCreateDebugLevelData()
        {
            string assetPath = "Assets/Scripts/Editor/DebugLevelData.asset";
            DebugLevelData settings = AssetDatabase.LoadAssetAtPath<DebugLevelData>(assetPath);
            
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<DebugLevelData>();
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }
    }
}
