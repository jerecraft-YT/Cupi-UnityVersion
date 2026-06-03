using UnityEngine;
using System.Collections.Generic;

public class TilesModeNotesController : MonoBehaviour
{
    public static TilesModeNotesController instance;

    public List<NotaTileNormal> NotasTileLeft = new();
    public List<NotaTileNormal> NotasTileRight = new();
    public List<NotaTileNormal> NotasTileMiddle = new();
    public List<NotaTileNormal> NotasActivas = new();

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
        foreach (NotaTileNormal nota in NotasActivas)
        {
            nota.UpdateNotePosition();
        }
    }

    public void NotifyToNote(bool acertaste,int noteIndex,CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                NotasTileLeft[noteIndex].DestroyNote();
                break;
            case CorrespondenciaTecla.Right:
                NotasTileRight[noteIndex].DestroyNote();
                break;
            case CorrespondenciaTecla.Middle:
                NotasTileMiddle[noteIndex].DestroyNote();
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
