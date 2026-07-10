using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public Dictionary<ChunkSeparation, Dictionary<int,ChunkLevelData>> chunks = new();

    [Tooltip("tamaño de los chunks en segundos")]
    private int _chunkSize;

    private void Awake()
    {
        SetDefaultConfig();

        _chunkSize = math.max(1, _chunkSize);
    }

    private void SetDefaultConfig()
    {
        _chunkSize = LevelDataController.defaultLevelConfig.chunkSize;
    }

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
            Debug.Log("/// INFO DE CHUNKs ///");
            Debug.Log("chunk de modo: " + chunk.Key.modoNota + " y tipo: " + chunk.Key.tipoNota );
            Debug.Log("tiene " + chunk.Value.Count + " chunks");
        }
    }

    private void CreateChunks(List<NotaInstance> listaNotas)
    {
        ChunkSeparation tempSeparation = new();

        foreach (var noteData in listaNotas)
        {
            tempSeparation.modoNota = noteData.modoNota;
            tempSeparation.tipoNota = noteData.tipoNota;

            //si la nota es sostenida se registra dos veces, una para su inicio y otra para su final
            if (noteData.tipoNota == TipoNota.Sostenida)
            {
                AddDataToChunk(tempSeparation, noteData);

                AddDataToChunk(tempSeparation, noteData, noteData.duracion);

                continue;
            }

            //esto seria si la nota es normal entonces se registra una sola vez
            AddDataToChunk(tempSeparation, noteData);
        }
    }

    private void AddDataToChunk(ChunkSeparation tempSeparation, NotaInstance nota, float offsetSpawn = 0.0f)
    {
        //^1 es para consultar al revez osea del final al inicio

        if (chunks[tempSeparation].Count == 0)
        {
            NewChunk(nota, tempSeparation);
        }
        else
        {
            //ChunkData actualChunk = chunks[tempSeparation][^1];

            int startChunk = chunks[tempSeparation].Last().Key;

            float timeNote = nota.timeToArrive + offsetSpawn;

            if (timeNote < startChunk + _chunkSize)
            {
                //Debug.Log("AGREGANDO NOTA NUEVA");
                ChunkNoteData chunkData = new (nota,timeNote);
                chunks[tempSeparation][startChunk].notas.Add(chunkData);
            }
            else
            {
                NewChunk(nota, tempSeparation,offsetSpawn);
            }
        }
    }

    private void NewChunk(NotaInstance nota, ChunkSeparation separation, float offsetSpawn = 0.0f)
    {
        /*
        Debug.Log("-----INFO DE CHUNK-----");
        Debug.Log("AGREGANDO CHUNK NUEVO");
        Debug.Log("MODO CHUNCK " + nota.modoNota);
        Debug.Log("TIPO CHUNCK " + nota.tipoNota);
        Debug.Log("-----FINAL DE INFO DE CHUNK-----");
        */

        float TimeNote = Mathf.FloorToInt(nota.timeToArrive + offsetSpawn);
        int startChunk = 0;

        if (TimeNote >= _chunkSize)
        {
            startChunk = (int)(TimeNote - (TimeNote % _chunkSize));
        }

        //ChunkData chunkData = new ChunkData();
        //chunkData.startChunk = startChunk;

        ChunkNoteData chunkLevelData = new(nota, nota.timeToArrive + offsetSpawn);

        List<ChunkNoteData> notas = new() { chunkLevelData };

        //chunkData.level = new(notas);

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

                Dictionary<int,ChunkLevelData> chunk = new();

                chunks.Add(chunkSeparation, chunk);
            }
        }
    }

    public int ChunkSize => _chunkSize;

    public Dictionary<ChunkSeparation, Dictionary<int, ChunkLevelData>> Chunks => chunks;
}
