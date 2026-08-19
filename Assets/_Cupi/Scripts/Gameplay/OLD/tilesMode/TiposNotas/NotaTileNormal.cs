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
        SetNoteVisibility(false);
        //DestroyNote();
    }

    private void SetNoteVisibility(bool isVisible)
    {
        spriteNote.enabled = isVisible;
    }

    public override void ChangeNoteState(EstadoPuntuacion puntuacion, EstadoNota estado)
    {
        switch (estado)
        {
            case EstadoNota.Fallada:
                Debug.Log("nota fallada");
                NoteMiss();
                break;
            case EstadoNota.Procesada:
                Debug.Log("nota acertada");
                NoteHit();
                break;
            default:
                break;
        }
    }
}
