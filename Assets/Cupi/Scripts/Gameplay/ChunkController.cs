using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    //public Dictionary<float,List<NotaInstance>> chunksNotes = new();

    public Dictionary<ChunkSeparation,Dictionary<ChunkInfo,List<NotaInstance>>> chunks = new();

    public float chunkSize = 1.0f;

    public List<int> chuncksToGenerate = new();

    ChunkSeparation grupoChunk = new(TipoNota.None, ModoNota.None);

    public void GenerateBulletChuncks(List<NotaInstance> listaNotas)
    {
        //replantear toda esta vaina
        /*
        float expectedSize = chunkSize;

        List<NotaInstance> actualChunk = new();

        Debug.Log("-----GENERANDO CHUNKS-----");

        bool groupCheck = false;

        foreach(NotaInstance notaActual in listaNotas)
        {
            if (!groupCheck)
            {
                grupoChunk.tipoNota = notaActual.tipoNota;
                grupoChunk.modoNota = notaActual.modoNota;
                groupCheck = true;
            }

            while(notaActual.timeToArrive > expectedSize || !NotaEsValida(notaActual.tipoNota,notaActual.modoNota))
            {
                expectedSize += chunkSize;
                if (actualChunk.Count != 0)
                {
                    print("creando Chunk");
                    //chunksNotes.Add(actualChunk[0].timeToArrive, new List<NotaInstance>(actualChunk));
                    int intialChunkInfo = Mathf.FloorToInt(actualChunk[0].timeToArrive);
                    int finalChunkInfo = Mathf.FloorToInt(actualChunk[actualChunk.Count - 1].timeToArrive);

                    ChunkInfo chunkInfo = new ChunkInfo(intialChunkInfo, finalChunkInfo);
                    
                    Dictionary<ChunkInfo, List<NotaInstance>> chunkContent = new()
                    {
                        {chunkInfo, new List<NotaInstance>(actualChunk)}
                    };

                    if (!chunks.TryGetValue(grupoChunk,out var chunkGroup))
                    {
                        chunkGroup = new Dictionary<ChunkInfo, List<NotaInstance>>();
                        chunks.Add(new ChunkSeparation(grupoChunk),chunkGroup);
                    }

                    chunkGroup.Add(chunkInfo, new List<NotaInstance>(actualChunk));
                    //chunks.Add(new ChunkSeparation(grupoChunk), chunkContent);

                    actualChunk.Clear();
                    grupoChunk.modoNota = ModoNota.None;
                    grupoChunk.tipoNota = TipoNota.None;

                    actualChunk.Add(notaActual);
                    grupoChunk.tipoNota = notaActual.tipoNota;
                    grupoChunk.modoNota = notaActual.modoNota;
                }
                continue;
            }

            actualChunk.Add(notaActual);
        }

        if (actualChunk.Count != 0)
        {
            print("creando Chunk, al final");
            //chunksNotes.Add(actualChunk[0].timeToArrive, new List<NotaInstance>(actualChunk));
            //actualChunk.Clear();
            int intialChunkInfo = Mathf.FloorToInt(actualChunk[0].timeToArrive);
            int finalChunkInfo = Mathf.FloorToInt(actualChunk[actualChunk.Count - 1].timeToArrive);

            ChunkInfo chunkInfo = new ChunkInfo(intialChunkInfo, finalChunkInfo);

            Dictionary<ChunkInfo, List<NotaInstance>> chunkContent = new()
                    {
                        {chunkInfo, new List<NotaInstance>(actualChunk)}
                    };

            chunks.Add(new ChunkSeparation(grupoChunk), chunkContent);

            actualChunk.Clear();
            grupoChunk.modoNota = ModoNota.None;
            grupoChunk.tipoNota = TipoNota.None;
            groupCheck = false;


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

    private bool NotaEsValida(TipoNota tipoNota,ModoNota modoNota)
    {
        if (tipoNota != grupoChunk.tipoNota || modoNota != grupoChunk.modoNota)
        {
            return false;
        }

        return true;
    }
}
