using UnityEngine;
using System.Collections.Generic;

public class TilesModeNotesController : MonoBehaviour
{
    public static TilesModeNotesController instance;

    public List<NotaNormal> NotaNormalLeft = new();
    public List<NotaNormal> NotaNormalRight = new();
    public List<NotaNormal> NotaNormalMiddle = new();
    public List<NotaNormal> activeNotes = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        foreach (NotaNormal nota in activeNotes)
        {
            nota.UpdateNotePosition();
        }
    }

    public void NotifyToNote(bool acertaste,int noteIndex,CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                NotaNormalLeft[noteIndex].DestroyNote();
                break;
            case CorrespondenciaTecla.Right:
                NotaNormalRight[noteIndex].DestroyNote();
                break;
            case CorrespondenciaTecla.Middle:
                NotaNormalMiddle[noteIndex].DestroyNote();
                break;
        }

        // de momento asi hasta que agregue mas cosas
        if (acertaste)
        {
            //NotaNormal[timeNote].DestroyNote();
        }
        else
        {
            //NotaNormal[timeNote].DestroyNote();
        }
    }
}
