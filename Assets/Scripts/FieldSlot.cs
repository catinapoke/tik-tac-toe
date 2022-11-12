using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSlot : MonoBehaviour
{
    private FieldItem slotItem = null;

    public bool IsAvailable => slotItem == null;
    public FieldItem Item => slotItem;
    
    public bool TrySet(FieldItem item)
    {
        if (!IsAvailable) 
            return false;

        slotItem = Instantiate(item, transform);
        return true;
    }

    public void Reset()
    {
        if(slotItem)
            Destroy(slotItem);
    }
}
