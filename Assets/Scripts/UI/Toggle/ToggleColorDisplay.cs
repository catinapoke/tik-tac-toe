using GameUI.Toggle;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(IndexedToggle))]
public class ToggleColorDisplay : MonoBehaviour
{
    [SerializeField] private Graphic _graphic;
    [SerializeField] private Color _enabledColor;
    [SerializeField] private Color _disabledColor;
    
    private IndexedToggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<IndexedToggle>();
        SetColor(_toggle.State);
    }

    private void OnEnable()
    {
        _toggle.OnChangeState += SetColor;
    }

    private void OnDisable()
    {
        _toggle.OnChangeState -= SetColor;
    }

    private void SetColor(bool state)
    {
        _graphic.color = state ? _enabledColor : _disabledColor;
    }
}
