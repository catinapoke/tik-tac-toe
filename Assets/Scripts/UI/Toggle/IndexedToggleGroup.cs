using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameUI.Toggle
{
    public class IndexedToggleGroup : MonoBehaviour
    {
        [Tooltip("Can be chosen only one toggle at the moment")]
        [SerializeField] private bool _uniqueToggle = true;
        
        private List<IndexedToggle> _toggles = new List<IndexedToggle>();

        public event Action<int> OnToggleClick;
        public IReadOnlyCollection<IndexedToggle> Toggles => _toggles;
        public bool IsTogglesUnique => _uniqueToggle;

        public void RegisterToggle(IndexedToggle toggle)
        {
            _toggles.Add(toggle);
        }
        
        public void UnregisterToggle(IndexedToggle toggle)
        {
            _toggles.Remove(toggle);
        }

        public bool IsValueActive(int value)
        {
            foreach (var toggle in _toggles)
            {
                if (toggle.State && toggle.Value == value)
                    return true;
            }

            return false;
        }
        
        public void ToggleClick(IndexedToggle toggle)
        {
            if (!_toggles.Contains(toggle)) return;

            toggle.SetState(true);
            if (_uniqueToggle && toggle.State == true)
            {
                foreach (var item in _toggles)
                {
                    if(item == toggle) continue;
                    item.SetState(false);
                }    
            }

            OnToggleClick?.Invoke(toggle.Value);
        }

        public bool TryForceToggle(int value)
        {
            IndexedToggle toggle = null;
            
            foreach (var item in _toggles)
            {
                if(item.Value == value) 
                    toggle = item;
            }

            if (toggle == null)
                return false;

            // If it already enabled
            if (toggle.State) 
                return true;
            
            ToggleClick(toggle);
            return true;
        }
    }
}