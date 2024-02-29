using System;

namespace Game.Dev.Scripts
{
    [Serializable]
    public class SaveData
    {
        //Set
        public int level;
        public int moneys;

        public bool sound = true;
        public bool haptic = true;

        public int winAmount;
        public int loseAmount;

        public int correctLetterAmount;
        public int wrongSpotLetterAmount;
        
        public GameLanguage gameLanguage = GameLanguage.English;
        

        //Get
        public int GetLevel()
        {
            return level;
        }

        public int GetMoneys()
        {
            return moneys;
        }

        public bool GetSound()
        {
            return sound;
        }

        public bool GetHaptic()
        {
            return haptic;
        }

        public GameLanguage GetGameLanguage()
        {
            return gameLanguage;
        }
    }
}