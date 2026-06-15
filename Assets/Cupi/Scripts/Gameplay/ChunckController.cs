using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

[Serializable]
public class ChunckViewer
{
    public Dictionary<float, List<NotaInstance>> notas;

    public ChunckViewer(Dictionary<float, List<NotaInstance>> notas)
    {
        this.notas = notas;
    }
}

public class ChunckController : MonoBehaviour
{
    public Dictionary<float,List<NotaInstance>> chuncksNotes = new();

    public float chunckSize = 1.0f;

    public List<int> chuncksToGenerate = new();

    public void Update()
    {
        
    }

    public void GenerateBulletChuncks(List<NotaInstance> listaNotas)
    {
        float expectedSize = chunckSize;

        List<NotaInstance> actualChunck = new();

        foreach(NotaInstance notaActual in listaNotas)
        {
            //print(notaActual.timeToArrive);
            while(notaActual.timeToArrive > expectedSize)
            {
                //print(notaActual.timeToArrive);
                expectedSize += chunckSize;
                if (actualChunck.Count != 0)
                {
                    print("creando Chunck");
                    chuncksNotes.Add(actualChunck[0].timeToArrive, new List<NotaInstance>(actualChunck));
                    actualChunck.Clear();
                }
                continue;
            }

            actualChunck.Add(notaActual);
        }

        if (actualChunck.Count != 0)
        {
            print("creando Chunck, al final");
            chuncksNotes.Add(actualChunck[0].timeToArrive, new List<NotaInstance>(actualChunck));
            actualChunck.Clear();
        }

        print(chuncksNotes.Count);


        foreach (var chunks in chuncksNotes)
        {
             print(chunks.Key + "|" + chunks.Value);
        }


       
    }
}
