using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    //public Dictionary<ChunkSeparation,List<ChunkData>> chunks = new();
    public Dictionary<ChunkSeparation, Dictionary<int,Level>> chunks = new();

    private int chunkSize = 3;

    public void GenerateBulletChunks(List<NotaInstance> listaNotas)
    {
        Debug.Log("-----GENERANDO CHUNKS-----");

        CreateChunksGroups();

        CreateChunks(listaNotas);

        FilterChunks();

        ChunksDebugInfo();

        Debug.Log("-----ACABO GENERACION DE CHUNKS-----");
    }

    private void FilterChunks()
    {
        List<ChunkSeparation> chunksToRemove = new();

        foreach (var chunk in chunks)
        {
            if (chunk.Value.Count == 0)
            {
                chunksToRemove.Add(chunk.Key);
            }
        }

        foreach(var itemRemove in chunksToRemove)
        {
            chunks.Remove(itemRemove);
        }
    }

    private void ChunksDebugInfo()
    {
        foreach (var chunk in chunks)
        {
            Debug.Log("chunk de modo: " + chunk.Key.modoNota + " y tipo: " + chunk.Key.tipoNota );
            Debug.Log("tiene " + chunk.Value.Count + " chunks");
        }
    }

    private void CreateChunks(List<NotaInstance> listaNotas)
    {
        ChunkSeparation tempSeparation = new();

        foreach (NotaInstance nota in listaNotas)
        {
            tempSeparation.modoNota = nota.modoNota;
            tempSeparation.tipoNota = nota.tipoNota;

            //^1 es para consultar al revez osea del final al inicio

            if (chunks[tempSeparation].Count == 0)
            {
                NewChunk(nota, tempSeparation);
            }
            else
            {
                //ChunkData actualChunk = chunks[tempSeparation][^1];

                int startChunk = chunks[tempSeparation].Last().Key;

                float timeNote = nota.timeToArrive;

                if (timeNote < startChunk + chunkSize)
                {
                    //Debug.Log("AGREGANDO NOTA NUEVA");
                    chunks[tempSeparation][startChunk].notas.Add(nota);
                }
                else
                {
                    NewChunk(nota, tempSeparation);
                }
            }
        }
    }

    private void NewChunk(NotaInstance nota, ChunkSeparation separation)
    {
        /*
        Debug.Log("-----INFO DE CHUNK-----");
        Debug.Log("AGREGANDO CHUNK NUEVO");
        Debug.Log("MODO CHUNCK " + nota.modoNota);
        Debug.Log("TIPO CHUNCK " + nota.tipoNota);
        Debug.Log("-----FINAL DE INFO DE CHUNK-----");
        */

        float TimeNote = Mathf.FloorToInt(nota.timeToArrive);
        int startChunk = 0;

        if (TimeNote >= chunkSize)
        {
            startChunk = (int)(TimeNote - (TimeNote % chunkSize));
        }

        ChunkData chunkData = new ChunkData();
        chunkData.startChunk = startChunk;

        List<NotaInstance> notas = new() { nota };

        chunkData.level = new(notas);

        chunks[separation].Add(startChunk, new(notas));
    }

    private void CreateChunksGroups()
    {
        foreach (var tipoNota in Enum.GetValues(typeof(TipoNota)))
        {
            if ((TipoNota)tipoNota == TipoNota.None) continue;

            foreach (var modoNota in Enum.GetValues(typeof(ModoNota)))
            {
                if ((ModoNota)modoNota == ModoNota.None) continue;

                ChunkSeparation chunkSeparation = new((TipoNota)tipoNota, (ModoNota)modoNota);

                Dictionary<int,Level> chunk = new();

                chunks.Add(chunkSeparation, chunk);
            }
        }
    }

    public int ChunkSize => chunkSize;

    public Dictionary<ChunkSeparation, Dictionary<int, Level>> Chunks => chunks;
}
