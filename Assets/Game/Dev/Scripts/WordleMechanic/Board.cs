using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Template.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Dev.Scripts.WordleMechanic
{
    public class Board : MonoBehaviour
    {
        [Header("Tiles")]
        public Tile.State emptyState;
        public Tile.State occupiedState;
        public Tile.State correctState;
        public Tile.State wrongSpotState;
        public Tile.State incorrectState;
        
        [Space(10)] 
        public List<TextAsset> wordAssets;
        public List<TextAsset> validAssets;
        public List<GameObject> keyboards;
        
        [Space(10)] 
        public Transform board;
        public GameObject invalidWordText;

        [Space(10)]
        [ReadOnly] public string word;
        
        private Row[] rows;
        private int rowIndex;
        private int columnIndex;

        private string[] solutions;
        private string[] validWords;
        
        private GameLanguage gameLanguage;

        private void Awake()
        {
            rows = board.GetComponentsInChildren<Row>();
        }

        private void Start()
        {
            LoadData();
            NewGame();
        }

        private void LoadData()
        {
            gameLanguage = SaveManager.instance.saveData.gameLanguage;
            keyboards.ActivateAtIndex((int)gameLanguage);
            
            solutions = wordAssets[(int)gameLanguage].text.Split("\n");
            validWords = validAssets[(int)gameLanguage].text.Split("\n");
        }

        private void NewGame()
        {
            ClearBoard();
            SetRandomWord();

            enabled = true;
        }

        // public void TryAgain()
        // {
        //     ClearBoard();
        //
        //     enabled = true;
        // }

        private void SetRandomWord()
        {
            word = solutions[Random.Range(0, solutions.Length)];
            word = word.ToLower().Trim();
        }

        public void GetInput(string input)
        {
            Row currentRow = rows[rowIndex];

            if (input == "delete")
            {
                columnIndex = Mathf.Max(columnIndex - 1, 0);

                currentRow.tiles[columnIndex].SetLetter('\0');
                currentRow.tiles[columnIndex].SetState(emptyState);

                invalidWordText.SetActive(false);
            }
            else if (columnIndex >= currentRow.tiles.Length)
            {
                if (input == "enter") 
                {
                    SubmitRow(currentRow);
                }
            }
            else
            {
                var inputChar = Convert.ToChar(input);
                currentRow.tiles[columnIndex].SetLetter(inputChar);
                currentRow.tiles[columnIndex].SetState(occupiedState);

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

            string remaining = word;

            // check correct/incorrect letters first
            for (int i = 0; i < row.tiles.Length; i++)
            {
                Tile tile = row.tiles[i];

                if (tile.letter == word[i])
                {
                    tile.SetState(correctState);

                    remaining = remaining.Remove(i, 1);
                    remaining = remaining.Insert(i, " ");
                }
                else if (!word.Contains(tile.letter))
                {
                    tile.SetState(incorrectState);
                }
            }

            // check wrong spots after
            for (int i = 0; i < row.tiles.Length; i++)
            {
                Tile tile = row.tiles[i];

                if (tile.state != correctState && tile.state != incorrectState)
                {
                    if (remaining.Contains(tile.letter))
                    {
                        tile.SetState(wrongSpotState);

                        int index = remaining.IndexOf(tile.letter);
                        remaining = remaining.Remove(index, 1);
                        remaining = remaining.Insert(index, " ");
                    }
                    else
                    {
                        tile.SetState(incorrectState);
                    }
                }
            }

            if (HasWon(row)) {
                
                BusSystem.CallLevelEnd(true);
                enabled = false;
            }

            rowIndex++;
            columnIndex = 0;

            if (rowIndex >= rows.Length) {
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
                if (row.tiles[i].state != correctState) {
                    return false;
                }
            }

            return true;
        }

        private void ClearBoard()
        {
            for (int row = 0; row < rows.Length; row++)
            {
                for (int col = 0; col < rows[row].tiles.Length; col++)
                {
                    rows[row].tiles[col].SetLetter('\0');
                    rows[row].tiles[col].SetState(emptyState);
                }
            }

            rowIndex = 0;
            columnIndex = 0;
        }
    }
}
