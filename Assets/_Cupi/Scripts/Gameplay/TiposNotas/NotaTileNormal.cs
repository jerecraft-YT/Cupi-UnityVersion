using UnityEngine;

public class NotaTileNormal : NotaTileBaseLogic
{
    private void NoteHit()
    {
        SetNoteVisibility(false);
        //DestroyNote();
    }
    private void NoteMiss()
    {
        //SetNoteVisibility(false);
        //DestroyNote();
    }

    //el engine deshizo la nota porque el tiempo retrocedio por detras de ella
    private void NoteReset()
    {
        SetNoteVisibility(true);
    }

    public override void ChangeNoteState(EstadoPuntuacion puntuacion, EstadoNota estado)
    {
        switch (estado)
        {
            case EstadoNota.None:
                NoteReset();
                break;
            case EstadoNota.Fallada:
                //Debug.Log("nota fallada");
                NoteMiss();
                break;
            case EstadoNota.Procesada:
                //Debug.Log("nota acertada");
                NoteHit();
                break;
            default:
                break;
        }
    }
}
