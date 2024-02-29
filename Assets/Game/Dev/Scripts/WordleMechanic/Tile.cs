using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dev.Scripts.WordleMechanic
{
    public class Tile : MonoBehaviour
    {
        private Image fill;
        private Outline outline;
        private TextMeshProUGUI text;
    
        public State state { get; private set; }
        public char letter { get; private set; }

        private void Awake()
        {
            fill = GetComponent<Image>();
            outline = GetComponent<Outline>();
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetLetter(char letter)
        {
            this.letter = letter;
            text.text = letter.ToString();
        }

        public void SetState(State state)
        {
            this.state = state;
            fill.color = state.fillColor;
            outline.effectColor = state.outlineColor;
        }
    
        [System.Serializable]
        public class State
        {
            public Color fillColor;
            public Color outlineColor;
        }
    }
}
