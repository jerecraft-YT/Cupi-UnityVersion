using System.Collections.Generic;
using UnityEngine;

public class ChunckController : MonoBehaviour
{
    public Dictionary<float,List<NotaInstance>> chunksNotes = new();

    public float chunkSize = 1.0f;

    public List<int> chuncksToGenerate = new();

    public void GenerateBulletChuncks(List<NotaInstance> listaNotas)
    {
        float expectedSize = chunkSize;

        List<NotaInstance> actualChunk = new();

        Debug.Log("-----GENERANDO CHUNKS-----");

        foreach(NotaInstance notaActual in listaNotas)
        {
            while(notaActual.timeToArrive > expectedSize)
            {
                expectedSize += chunkSize;
                if (actualChunk.Count != 0)
                {
                    print("creando Chunk");
                    chunksNotes.Add(actualChunk[0].timeToArrive, new List<NotaInstance>(actualChunk));
                    actualChunk.Clear();
                }
                continue;
            }

            actualChunk.Add(notaActual);
        }

        if (actualChunk.Count != 0)
        {
            print("creando Chunk, al final");
            chunksNotes.Add(actualChunk[0].timeToArrive, new List<NotaInstance>(actualChunk));
            actualChunk.Clear();
        }
        Debug.Log("-----ACABO GENERACION DE CHUNKS-----");

        //esto es mucha info que solo se usara cuando lo necesite
        /*
        Debug.Log("-----INFO CHUNCKS-----");
        foreach (var chunk in chuncksNotes)
        {
            foreach(NotaInstance nota in chunk.Value)
            {
                Debug.Log(chunk.Key + "|" + nota.timeToArrive);
            }
        }
        Debug.Log("-----ACABO INFO CHUNCKS-----");
        */
    }
}
