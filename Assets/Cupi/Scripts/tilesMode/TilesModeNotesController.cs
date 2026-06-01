using UnityEngine;
using System.Collections.Generic;

public class TilesModeNotesController : MonoBehaviour
{
    public static TilesModeNotesController instance;

    public List<NotaNormal> NotasNormalLeft = new();
    public List<NotaNormal> NotasNormalRight = new();
    public List<NotaNormal> NotasNormalMiddle = new();
    public List<NotaNormal> NotasActivas = new();

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
        foreach (NotaNormal nota in NotasActivas)
        {
            nota.UpdateNotePosition();
        }
    }

    public void NotifyToNote(bool acertaste,int noteIndex,CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                NotasNormalLeft[noteIndex].DestroyNote();
                break;
            case CorrespondenciaTecla.Right:
                NotasNormalRight[noteIndex].DestroyNote();
                break;
            case CorrespondenciaTecla.Middle:
                NotasNormalMiddle[noteIndex].DestroyNote();
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
