using System.Collections.Generic;
using Game.Dev.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Scripts
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject soundOff;
        [SerializeField] private GameObject hapticOn;
        [SerializeField] private GameObject hapticOff;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI versionText;

        [Space(10)] 
        public List<Image> flagIcons;
        public List<Sprite> flagSprites;

        private void Start()
        {
            InitSettings();
            InitGameVersion();
            InitLanguage();
        }

        public void ToggleSound()
        {
            bool currentSoundState = SaveManager.instance.saveData.GetSound();
    
            soundOn.SetActive(!currentSoundState);
            soundOff.SetActive(currentSoundState);
            
            AudioListener.volume = !currentSoundState ? 1 : 0;

            SaveManager.instance.saveData.sound = !currentSoundState;
            SaveManager.instance.Save();
        }

        public void ToggleHaptic()
        {
            bool currentHapticState = SaveManager.instance.saveData.GetHaptic();
    
            hapticOn.SetActive(!currentHapticState);
            hapticOff.SetActive(currentHapticState);
            
            MoreMountains.NiceVibrations.MMVibrationManager.SetHapticsActive(!currentHapticState);

            SaveManager.instance.saveData.haptic = !currentHapticState;
            SaveManager.instance.Save();
        }

        private void InitSettings()
        {
            bool currentSoundState = SaveManager.instance.saveData.GetSound();
            bool currentHapticState = SaveManager.instance.saveData.GetHaptic();

            SetActiveState(soundOn, soundOff, currentSoundState);
            SetActiveState(hapticOn, hapticOff, currentHapticState);

            void SetActiveState(GameObject onObject, GameObject offObject, bool state)
            {
                onObject.SetActive(state);
                offObject.SetActive(!state);
            }
        }

        private void InitLanguage()
        {
            GameLanguage gameLanguage = SaveManager.instance.saveData.GetGameLanguage();

            for (int i = 0; i < flagIcons.Count; i++)
            {
                flagIcons[i].sprite = flagSprites[(int)gameLanguage];
            }
        }

        public void ChangeLanguage(int languageIndex)
        {
            SaveManager.instance.saveData.gameLanguage = (GameLanguage)languageIndex;
            BusSystem.CallLevelLoad();
        }

        private void InitGameVersion()
        {
            string title = "version ";
            versionText.text = title + Application.version;
        }
    }
}