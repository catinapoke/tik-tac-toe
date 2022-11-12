using UnityEngine;
using UnityEngine.Assertions;

public class FieldTaper : MonoBehaviour
{
    [SerializeField] private MatchInput _matchInput;
    
    private Camera inputCamera;
    
    private void Start()
    {
        inputCamera = Camera.main;
        Assert.IsNotNull(inputCamera);
    }

    void Update () {
        if (Input.GetMouseButtonDown(0) ) {
            Vector3 mousePos = inputCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Slot")) {
                Debug.Log(hit.collider.gameObject.name);
                
                FieldSlot slot = hit.collider.gameObject.GetComponent<FieldSlot>();
                if (slot == null)
                {
                    Debug.LogError("Can't get slot!");
                    return;
                }
                    
                _matchInput.Tap(slot);
            }
        }
    }
}
