using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The class for saving modification done to dialouge
/// <summary>
/// when saving changes to dialouge, if changes are applied to source dialoge directly
/// it will cause the original dialogue to be change so we save the changes to an external file
/// using this class which saves the starting dialouge, and any dialouge unlocks
/// </summary>
public class DialougeRecord
{
    public int startindex;
    public Dictionary<string,bool> modified;

    public DialougeRecord(int startindex)
    {
        modified = new Dictionary<string, bool>();
        this.startindex = startindex;
    }
}
