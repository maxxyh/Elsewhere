using System;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelection
{
    public class LevelSelectButton : MonoBehaviour
    {
        public GameObject selectableButton;
        public string levelId;

        public Action<string> onButtonClick;

        [SerializeField] private Sprite unlockedSprite;
        [SerializeField] private Sprite completedSprite;
        private Image _selectableButtonImage;

        private void Awake()
        {
            _selectableButtonImage = selectableButton.GetComponent<Image>();
        }

        public void ToggleSelectable(int selectable)
        {
            selectableButton.SetActive(selectable != 0);
            if (selectable == 1)
            {
                _selectableButtonImage.sprite = unlockedSprite;
            }

            if (selectable == 2)
            {
                _selectableButtonImage.sprite = completedSprite;
            }
        }

        public void OnButtonClick()
        {
            onButtonClick.Invoke(levelId);
        }
    }
}