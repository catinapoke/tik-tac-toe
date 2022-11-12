using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameUI.Toggle
{
    [RequireComponent(typeof(Button))]
    public class IndexedToggle : MonoBehaviour
    {
        [SerializeField] private IndexedToggleGroup _group;
        [SerializeField] private int _value;
        [SerializeField] private bool _state;

        public int Value => _value;
        public bool State => _state;
        
        private Button _button;

        public UnityAction<bool> OnChangeState;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _group.RegisterToggle(this);
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _group.UnregisterToggle(this);
            _button.onClick.RemoveListener(OnClick);
        }

        public void SetStateSilent(bool state)
        {
            _state = state;
        }

        public void SetState(bool state)
        {
            SetStateSilent(state);
            OnChangeState?.Invoke(_state);
        }

        private void OnClick()
        {
            if(_group.IsTogglesUnique && _state) return;
            
            SetState(!_state);
            _group.ToggleClick(this);
        }
    }
}