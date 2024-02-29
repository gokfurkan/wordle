﻿using System;
using Sirenix.OdinInspector;
using Template.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Game.Dev.Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        [ReadOnly] public GameStatus gameStatus;

        protected override void Initialize()
        {
            base.Initialize();

            OnGameStart();
        }
        
        private void OnEnable()
        {
            BusSystem.OnLevelStart += OnLevelStart;
            BusSystem.OnLevelEnd += OnLevelEnd;

            BusSystem.OnLevelLoad += OnLevelLoad;
        }

        private void OnDisable()
        {
            BusSystem.OnLevelStart -= OnLevelStart;
            BusSystem.OnLevelEnd -= OnLevelEnd;
            
            BusSystem.OnLevelLoad -= OnLevelLoad;
        }

        private void Update()
        {
            InputControl();
        }
        
        private void OnGameStart()
        {
            gameStatus.hasGameStart = true;
        }

        private void OnLevelStart()
        {
            gameStatus.hasLevelPlaying = true;
        }

        private void OnLevelEnd(bool win)
        {
            if (gameStatus.hasLevelEnd) return;
            
            gameStatus.hasLevelPlaying = false;
            gameStatus.hasLevelEnd = true;
            
            if (win)
            {
                SaveManager.instance.saveData.winAmount++;
                
                SaveManager.instance.saveData.level++;
                SaveManager.instance.Save();
                gameStatus.hasLevelWin = true;
            }
            else
            {
                SaveManager.instance.saveData.loseAmount++;
                SaveManager.instance.Save();
                gameStatus.hasLevelLose = true;
            }
        }

        private void OnLevelLoad()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        private void InputControl()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        HandleInput();
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        HandleInput();
                    }
                }

                void HandleInput()
                {
                    if (!gameStatus.hasLevelPlaying)
                    {
                        OnLevelStart();
                        BusSystem.CallLevelStart();
                    }
    
                    BusSystem.CallMouseClickDown();
                }
            }

            if (Input.GetMouseButton(0))
            {
                BusSystem.CallMouseClicking();
            }

            if (Input.GetMouseButtonUp(0))
            {
                BusSystem.CallMouseClickUp();
            }
        }
    }

    [Serializable]
    public class GameStatus
    {
        public bool hasGameStart;
        public bool hasLevelPlaying;
        public bool hasLevelEnd;
        public bool hasLevelWin;
        public bool hasLevelLose;
    }
}