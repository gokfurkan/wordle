using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Dev.Scripts.Main
{
    public class Board : MonoBehaviour
    {
        private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
            KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
            KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
            KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
            KeyCode.Y, KeyCode.Z,
        };

        private Row[] rows;
        private int rowIndex;
        private int columnIndex;

        private string[] solutions;
        private string[] validWords;
        [ReadOnly] public string word;

        [Space(10)]
        [Header("Tiles")]
        public Tile.State emptyState;
        public Tile.State occupiedState;
        public Tile.State correctState;
        public Tile.State wrongSpotState;
        public Tile.State incorrectState;

        [Space(10)] 
        public Transform board;
        public TextAsset wordAsset;
        public GameObject invalidWordText;

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
            solutions = wordAsset.text.Split("\n");

            // textFile = Resources.Load("official_wordle_all") as TextAsset;
            // validWords = textFile.text.Split("\n");
        }

        public void NewGame()
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
            return true;
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
