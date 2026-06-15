using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChunckController : MonoBehaviour
{
    public List<List<NotaInstance>> chuncksNotes = new();

    public float chunckSize = 1.0f;

    public void GenerateBulletChuncks(List<NotaInstance> listaNotas)
    {
        float expectedSize = chunckSize;

        List<NotaInstance> actualChunck = new();

        foreach(NotaInstance notaActual in listaNotas)
        {
            if (notaActual.timeToArrive > expectedSize)
            {
                expectedSize += chunckSize;
                chuncksNotes.Add(new List<NotaInstance>(actualChunck));
                actualChunck.Clear();
                continue;
            }

            actualChunck.Add(notaActual);
        }

        if (actualChunck.Count != 0)
        {
            chuncksNotes.Add(new List<NotaInstance>(actualChunck));
            actualChunck.Clear();
        }

        print(chuncksNotes.Count);
    }
}
