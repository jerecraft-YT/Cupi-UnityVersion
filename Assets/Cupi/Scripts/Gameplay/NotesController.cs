using UnityEngine;
using System;

public class NotesController : MonoBehaviour
{
    public static NotesController instance;

    public static event Action NotasActivas;

    public static event Action<CorrespondenciaTecla> NoteHit;
    public static event Action<CorrespondenciaTecla> NoteNoHit;

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
        NotasActivas?.Invoke();
    }

    public static void MissNote(CorrespondenciaTecla tecla)
    {
        NoteNoHit?.Invoke(tecla);
    }

    public static void HitNote(CorrespondenciaTecla tecla)
    {
        NoteHit?.Invoke(tecla);
    }
}
