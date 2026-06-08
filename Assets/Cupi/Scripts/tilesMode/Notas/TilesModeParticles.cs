using System.Collections.Generic;
using UnityEngine;

public class TilesModeParticles : MonoBehaviour
{
    [Header("parentParticles")]
    [SerializeField] private GameObject parentParticles;

    [Header("particles Prefab")]
    [SerializeField] private GameObject particleHit;
    [SerializeField] private GameObject particleNoHit;
    //private List<ParticleSystem> ParticulasHit;
    private Dictionary<CorrespondenciaTecla, ParticleSystem> ParticulasHit;
    private Dictionary<CorrespondenciaTecla, ParticleSystem> ParticulasNoHit;

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
        //aprendi a la mala que si o si debes inicializarlos si no estaras sufriendo por nada :c
        ParticulasHit = new();
        ParticulasNoHit = new();
        InstantiateParticles();
    }

    private void InstantiateParticles()
    {
        foreach (var (correspondencia, origenParticulas) in SpawnerNotas.PosicionFinalNotaDic)
        {
            Vector3 posicionParticulas = origenParticulas.transform.position;

            GameObject particulaHit = Instantiate(particleHit, posicionParticulas, Quaternion.identity, parentParticles.transform);
            GameObject particulaNoHit = Instantiate(particleNoHit, posicionParticulas, Quaternion.identity, parentParticles.transform);
            ParticulasHit[correspondencia] = particulaHit.GetComponent<ParticleSystem>();
            ParticulasNoHit[correspondencia] = particulaNoHit.GetComponent<ParticleSystem>();
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
