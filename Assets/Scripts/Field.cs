using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Field : MonoBehaviour
{
    [SerializeField] private FieldSlot[] points;
    [SerializeField] private FieldItem[] signs;

    public Action<ItemType> OnVictory;

    private void OnValidate()
    {
        Assert.IsTrue(points.Length == 9);
        Assert.IsTrue(signs.Length == 2);
    }

    public void Tap(FieldSlot slot, ItemType itemType)
    {
        int row, column;
        (row, column) = GetSlotPosition(slot);
        Tap(row, column, itemType);
        
        slot.TrySet(signs[itemType == ItemType.Circle ? 0 : 1]);
        CheckWinStateGreedy(row, column);
    }
    
    public void Tap(int row, int column, ItemType itemType)
    {
        Assert.IsTrue(row is >-1 and <3 && column is >-1 and <3);

        FieldSlot slot = points[row * 3 + column];
        slot.TrySet(signs[itemType == ItemType.Circle ? 0 : 1]);
        CheckWinStateGreedy(row, column);
    }

    // Returns (row, column) of slot
    public (int, int) GetSlotPosition(FieldSlot slot)
    {
        int slotNumber = -1;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == slot)
            {
                slotNumber = i;
                break;
            }
        }

        if (slotNumber == -1)
            return (-1, -1);

        return (slotNumber / 3, slotNumber % 3);
    }

    private void CheckWinStateGreedy(int row, int column)
    {
        if (row is < -1 or > 2 || column is < -1 or > 2)
            return;

        FieldItem[] items = new FieldItem[3];
        if (CheckSet(FillArray(items, Row(row)), out ItemType type))
        {
            Debug.Log($"Won in row#{row}: {type.ToString()}");
            OnVictory?.Invoke(type);
            return;
        }

        if (CheckSet(FillArray(items, Column(column)), out type))
        {
            Debug.Log($"Won in column#{column}: {type.ToString()}");
            OnVictory?.Invoke(type);
            return;
        }

        if (row == column && CheckSet(FillArray(items, Cross(true)), out type))
        {
            Debug.Log($"Won in diagonal#0: {type.ToString()}");
            OnVictory?.Invoke(type);
            return;
        }

        if (row + column == 2 && CheckSet(FillArray(items, Cross(false)), out type))
        {
            Debug.Log($"Won in diagonal#1: {type.ToString()}");
            OnVictory?.Invoke(type);
            return;
        }
    }

    private void CheckWinState()
    {
        FieldItem[] items = new FieldItem[3];
        for (int i = 0; i < 3; i++)
        {
            FillArray(items, Row(i));
            if (CheckSet(items, out ItemType type))
            {
                Debug.Log($"Won in row#{i}: {type.ToString()}");
                return;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            FillArray(items, Column(i));
            if (CheckSet(items, out ItemType type))
            {
                Debug.Log($"Won in column#{i}: {type.ToString()}");
                return;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            FillArray(items, Cross(i == 0));
            if (CheckSet(items, out ItemType type))
            {
                Debug.Log($"Won in diagonal#{i}: {type.ToString()}");
                return;
            }
        }
    }

    private bool CheckSet(FieldItem[] set, out ItemType winType)
    {
        FieldItem first = set[0];
        if (first == null)
        {
            winType = ItemType.Circle;
            return false;
        }

        winType = first.Type;
        foreach (var item in set)
        {
            if (item == null || item.Type != winType)
                return false;
        }

        return true;
    }

    FieldItem[] FillArray(FieldItem[] array, IEnumerable<FieldItem> slots)
    {
        int i = 0;
        foreach (var slot in slots)
        {
            array[i++] = slot;
        }

        return array;
    }

    IEnumerable<FieldItem> Row(int i)
    {
        if (i is < -1 or > 2) yield break;

        for (int j = 0; j < 3; j++)
        {
            yield return points[i * 3 + j].Item;
        }
    }

    IEnumerable<FieldItem> Column(int i)
    {
        if (i is < -1 or > 2) yield break;

        for (int j = 0; j < 3; j++)
        {
            yield return points[j * 3 + i].Item;
        }
    }

    IEnumerable<FieldItem> Cross(bool first)
    {
        for (int j = 0; j < 3; j++)
        {
            int k = first ? j : 2 - j;
            yield return points[k * 3 + k].Item;
        }
    }
}