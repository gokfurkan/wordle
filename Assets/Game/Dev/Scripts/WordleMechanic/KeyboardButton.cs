using System;
using Game.Dev.Scripts.Scriptables;
using Template.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dev.Scripts.WordleMechanic
{
    public class KeyboardButton : MonoBehaviour
    {
        [SerializeField] private Image buttonBg;
        [SerializeField] private Image buttonOutline;
        [SerializeField] private TextMeshProUGUI text;

        private char buttonLetter;
        private GameSettings gameSettings;
        
        private void OnEnable()
        {
            BusSystem.OnRefreshLetterLists += OnRefreshLetterLists;
        }

        private void OnDisable()
        {
            BusSystem.OnRefreshLetterLists -= OnRefreshLetterLists;
        }

        private void Awake()
        {
            gameSettings = InitializeManager.instance.gameSettings;
            if (text.text.Length < 2)
            {
                buttonLetter = Convert.ToChar(text.text.ToLower());
            }
        }

        private void OnRefreshLetterLists()
        {
            if (text.text.Length > 1) return;
            
            if (Board.instance.correctLetters.Contains(buttonLetter))
            {
                buttonBg.color = gameSettings.boardOptions.correctState.fillColor;
                buttonOutline.color = gameSettings.boardOptions.correctState.outlineColor;
            }
            else if (Board.instance.wrongSpotLetters.Contains(buttonLetter))
            {
                buttonBg.color = gameSettings.boardOptions.wrongSpotState.fillColor;
                buttonOutline.color = gameSettings.boardOptions.wrongSpotState.outlineColor;
            }
            else
            {
                buttonBg.color = gameSettings.boardOptions.keyboardEmptyState.fillColor;
                buttonOutline.color = gameSettings.boardOptions.keyboardEmptyState.outlineColor;
            }
        }
    }
}