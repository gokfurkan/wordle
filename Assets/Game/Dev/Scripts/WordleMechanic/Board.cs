using System;
using System.Collections.Generic;
using Game.Dev.Scripts.Scriptables;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using Template.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Dev.Scripts.WordleMechanic
{
    public class Board : Singleton<Board>
    {
        public Transform board;
        public GameObject invalidWordText;
        public List<GameObject> keyboards;

        [Space(10)]
        [ReadOnly] public string currentWord;
        
        private Row[] rows;
        private int rowIndex;
        private int columnIndex;

        private string[] solutions;
        private string[] validWords;
        
        [ReadOnly] public List<char> correctLetters;
        [ReadOnly] public List<char> wrongSpotLetters;
        
        private GameLanguage gameLanguage;
        private GameSettings gameSettings;

        private void Start()
        {
            InitBoard();
        }

        private void InitBoard()
        {
            rows = board.GetComponentsInChildren<Row>();
            
            LoadData();
            ClearBoard();
            SetRandomWord();
        }

        public void GetInput(string input)
        {
            Row currentRow = rows[rowIndex];

            if (input == "delete")
            {
                columnIndex = Mathf.Max(columnIndex - 1, 0);
                currentRow.tiles[columnIndex].SetLetter('\0');
                currentRow.tiles[columnIndex].SetState(gameSettings.boardOptions.emptyState);
                invalidWordText.SetActive(false);
                
                AudioManager.instance.Play(AudioType.ResetWord);
            }
            else if (input == "enter")
            {
                if (columnIndex >= currentRow.tiles.Length) 
                {
                    SubmitRow(currentRow);
                }
                else
                {
                    AudioManager.instance.Play(AudioType.WrongWord);
                    HapticManager.instance.PlayHaptic(HapticTypes.MediumImpact);
                }
            }
            else
            {
                if (columnIndex >= currentRow.tiles.Length) return;
                
                AudioManager.instance.Play(AudioType.Tap);
                HapticManager.instance.PlayHaptic(HapticTypes.LightImpact);
                
                var inputChar = Convert.ToChar(input);
                currentRow.tiles[columnIndex].SetLetter(inputChar);
                currentRow.tiles[columnIndex].SetState(gameSettings.boardOptions.occupiedState);
                columnIndex++;
            }
        }

        private void SubmitRow(Row row)
        {
            if (!IsValidWord(row.Word))
            {
                invalidWordText.SetActive(true);
                return;
            }

            string remaining = currentWord;
            
            for (int i = 0; i < row.tiles.Length; i++)
            {
                Tile tile = row.tiles[i];

                if (tile.letter == currentWord[i])
                {
                    AddCorrectLetter(tile.letter);
                    tile.SetState(gameSettings.boardOptions.correctState);

                    remaining = remaining.Remove(i, 1);
                    remaining = remaining.Insert(i, " ");
                }
                else if (!currentWord.Contains(tile.letter))
                {
                    tile.SetState(gameSettings.boardOptions.incorrectState);
                }
            }

            // check wrong spots after
            for (int i = 0; i < row.tiles.Length; i++)
            {
                Tile tile = row.tiles[i];

                if (tile.state != gameSettings.boardOptions.correctState && tile.state != gameSettings.boardOptions.incorrectState)
                {
                    if (remaining.Contains(tile.letter))
                    {
                        tile.SetState(gameSettings.boardOptions.wrongSpotState);

                        AddWrongSpotLetter(tile.letter);
                        int index = remaining.IndexOf(tile.letter);
                        remaining = remaining.Remove(index, 1);
                        remaining = remaining.Insert(index, " ");
                    }
                    else
                    {
                        tile.SetState(gameSettings.boardOptions.incorrectState);
                    }
                }
            }

            if (HasWon(row)) {
                
                AudioManager.instance.Play(AudioType.RightWord);
                HapticManager.instance.PlayHaptic(HapticTypes.Success);
                BusSystem.CallLevelEnd(true);
                enabled = false;
            }
            else
            {
                AudioManager.instance.Play(AudioType.WrongWord);
                HapticManager.instance.PlayHaptic(HapticTypes.MediumImpact);
            }

            rowIndex++;
            columnIndex = 0;

            if (rowIndex >= rows.Length) {
                AudioManager.instance.Play(AudioType.GameOver);
                HapticManager.instance.PlayHaptic(HapticTypes.Warning);
                BusSystem.CallLevelEnd(false);
                enabled = false;
            }
        }

        private bool IsValidWord(string word)
        {
            for (int i = 0; i < validWords.Length; i++)
            {
                if (validWords[i] == word) {
                    return true;
                }
            }

            return false;
        }

        private bool HasWon(Row row)
        {
            for (int i = 0; i < row.tiles.Length; i++)
            {
                if (row.tiles[i].state != gameSettings.boardOptions.correctState) {
                    return false;
                }
            }

            return true;
        }

        private void ClearBoard()
        {
            foreach (Row row in rows)
            {
                foreach (Tile tile in row.tiles)
                {
                    tile.SetLetter('\0');
                    tile.SetState(gameSettings.boardOptions.emptyState);
                }
            }

            rowIndex = 0;
            columnIndex = 0;
        }
        
        private void SetRandomWord()
        {
            currentWord = solutions[Random.Range(0, solutions.Length)].Trim().ToLower();
        }
        
        private void LoadData()
        {
            gameSettings = InitializeManager.instance.gameSettings;
            gameLanguage = SaveManager.instance.saveData.GetGameLanguage();
            
            keyboards.ActivateAtIndex((int)gameLanguage);
            solutions = gameSettings.boardOptions.wordAssets[(int)gameLanguage].text.Split("\n");
            validWords = gameSettings.boardOptions.validAssets[(int)gameLanguage].text.Split("\n");
        }

        private void AddCorrectLetter(char letter)
        {
            foreach (var t in correctLetters)
            {
                if (letter == t)
                {
                    return;
                }
            }

            correctLetters.Add(letter);
            BusSystem.CallRefreshLetterLists();
        }
        
        private void AddWrongSpotLetter(char letter)
        {
            foreach (var t in wrongSpotLetters)
            {
                if (letter == t)
                {
                    return;
                }
            }

            wrongSpotLetters.Add(letter);
            BusSystem.CallRefreshLetterLists();
        }
    }
}
