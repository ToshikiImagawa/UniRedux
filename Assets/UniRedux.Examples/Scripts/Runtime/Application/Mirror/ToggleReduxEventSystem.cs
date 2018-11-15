using UniRedux.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples.Application
{
    public class ToggleReduxEventSystem : MonoBehaviour
    {
        [SerializeField] private ReduxEventSystem _reduxEventSystem;

        private Image _image;
        private Text _text;

        private Image Image => _image != null ? _image : (_image = GetComponent<Image>());
        private Text Text => _text != null ? _text : (_text = GetComponentInChildren<Text>());

        public void Toggle()
        {
            if (_reduxEventSystem == null || _reduxEventSystem.gameObject == null) return;
            _reduxEventSystem.gameObject.SetActive(!_reduxEventSystem.gameObject.activeSelf);
            UpdateDisplay();
        }

        private void Awake()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            Image.color = _reduxEventSystem.gameObject.activeSelf
                ? new Color(0.03f, 0.44f, 1f)
                : new Color(1f, 0.27f, 0.27f);

            Text.text = _reduxEventSystem.gameObject.activeSelf
                ? "Mirror Active"
                : "Mirror Inactive";
        }
    }
}