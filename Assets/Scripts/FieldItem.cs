using UnityEngine;

public class FieldItem : MonoBehaviour
{
    [SerializeField] private ItemType itemType;

    public ItemType Type => itemType;
}

public enum ItemType
{
    Circle = 0,
    Cross = 1
}