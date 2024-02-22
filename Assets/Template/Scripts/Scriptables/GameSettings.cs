using System;
using System.Collections.Generic;
using Game.Dev.Scripts.WordleMechanic;
using Sirenix.OdinInspector;
using Template.Scripts;
using TMPro;
using UnityEngine;

namespace Game.Dev.Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        public LoadOptions loadOptions;
        public GamePlayOptions gamePlayOptions;
        public EconomyOptions economyOptions;
        public UIOptions uiOptions;
        public BoardOptions boardOptions;
    }

    [Serializable]
    public class LoadOptions
    {
        public SceneType nextSceneAfterLoad;
        public List<LoadFillOptions> loadFillOption;
    }

    [Serializable]
    public class GamePlayOptions
    {
        public bool vSyncEnabled;
        public int targetFPS;
    }

    [Serializable]
    public class EconomyOptions
    {
        public bool useMoneyAnimation;
        [ShowIf(nameof(useMoneyAnimation))]
        public float moneyAnimationDuration;

        [Space(10)] 
        public int winIncome;
        public int loseIncome;
        
        [Space(10)] 
        public int spawnMoneyAmount;
    }

    [Serializable]
    public class UIOptions
    {
        public float winPanelDelay;
        public float losePanelDelay;
        public float endContinueDelay;
        
        [Space(10)]
        public string levelText;

        [Space(10)]
        public bool hasEndPanelLevel;
        public string levelCompletedText;
        public string levelFailedText;

        [Space(10)] 
        public TMP_FontAsset  textFont;
    }

    [Serializable]
    public class BoardOptions
    {
        [Space(10)] 
        public List<TextAsset> validAssets;
        public List<TextAsset> wordAssets;
        
        [Space(10)] 
        public Tile.State emptyState;
        public Tile.State occupiedState;
        public Tile.State correctState;
        public Tile.State wrongSpotState;
        public Tile.State incorrectState;
    }
}