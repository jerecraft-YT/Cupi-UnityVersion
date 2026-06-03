using System;
using UnityEngine;

public class NotaTileSostenida : MonoBehaviour
{
    public NotaTileNormal notaTileMaster;
    public LineRenderer lineNote;

    private void OnEnable()
    {
        notaTileMaster.UpdateNote += DrawLine;
        lineNote.positionCount = 2;
    }

    public void DrawLine(Vector2 finalPosNote)
    {
        
    }
}
