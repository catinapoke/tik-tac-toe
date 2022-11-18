using TMPro;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

[RequireComponent(typeof(TMP_Text))]
public class TurnDisplay : MonoBehaviour
{
    [SerializeField] private MatchInput _input;
    [SerializeField] private string _localPlayerTurn;
    [SerializeField] private string _otherPlayerTurn;
    private TMP_Text _text;
    
    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public async void OnEnable()
    {
        while (!_input.IsInitialized)
        {
            await Task.Delay(50);
            if(!gameObject.activeSelf) return;
        }
        
        _input.OnTurnChanged += OnTurnChanged;
        UpdateText();
    }

    public void OnDisable()
    {
        _input.OnTurnChanged -= OnTurnChanged;
    }

    private void OnTurnChanged(ItemType prev, ItemType current)
    {
        Debug.Log($"OnTurnChanged({prev}, {current})");
        UpdateText();
    }
    
    private void UpdateText()
    {
        _text.text = _input.IsCurrentPlayerTurn ? _localPlayerTurn : _otherPlayerTurn;
    }
}
