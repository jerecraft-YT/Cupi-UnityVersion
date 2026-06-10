using System.Collections.Generic;
using UnityEngine;

public class TilesModeParticlesController : MonoBehaviour
{
    [Header("parentParticles")]
    [SerializeField] private GameObject parentParticles;

    [Header("particles Prefab")]
    [SerializeField] private GameObject particleHit;
    [SerializeField] private GameObject particleNoHit;

    //aprendi a la mala que si o si debes inicializarlos si no estaras sufriendo por nada :c
    private Dictionary<CorrespondenciaTecla, ParticleSystem> ParticulasHit = new();
    private Dictionary<CorrespondenciaTecla, ParticleSystem> ParticulasNoHit = new();

    private void OnEnable()
    {
        TilesModeNotesController.NoteHit += ParticleHitNote;
        TilesModeNotesController.NoteNoHit += ParticleNoHitNote;
    }

    private void OnDisable()
    {
        TilesModeNotesController.NoteHit -= ParticleHitNote;
        TilesModeNotesController.NoteNoHit -= ParticleNoHitNote;
    }

    private void Start()
    {
        InstantiateParticles();
    }

    private void InstantiateParticles()
    {
        foreach (var (tecla, origenParticulas) in SpawnerNotas.PosicionFinalNotaTile)
        {
            Vector3 posicionParticulas = origenParticulas.transform.position;

            GameObject particulaHit = Instantiate(particleHit, posicionParticulas, Quaternion.identity, parentParticles.transform);
            GameObject particulaNoHit = Instantiate(particleNoHit, posicionParticulas, Quaternion.identity, parentParticles.transform);
            
            ParticulasHit[tecla] = particulaHit.GetComponent<ParticleSystem>();
            ParticulasNoHit[tecla] = particulaNoHit.GetComponent<ParticleSystem>();
        }
    }

    private void ParticleHitNote(CorrespondenciaTecla tecla)
    {
        ParticulasHit[tecla].Play();
    }

    private void ParticleNoHitNote(CorrespondenciaTecla tecla)
    {
        ParticulasNoHit[tecla].Play();
    }
}
