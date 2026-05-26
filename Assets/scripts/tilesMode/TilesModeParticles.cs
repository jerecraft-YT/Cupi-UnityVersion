using UnityEngine;

public class TilesModeParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftParticle;
    [SerializeField] private ParticleSystem rightParticle;
    [SerializeField] private ParticleSystem middleParticle;
    [SerializeField] private TilesModeController controller;

    private void OnEnable()
    {
        controller.NoteHit += ParticleHitNote;
    }

    void ParticleHitNote(CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                PlayParticle(leftParticle);
                break;
            case CorrespondenciaTecla.Right:
                PlayParticle(rightParticle);
                break;
            case CorrespondenciaTecla.Middle:
                PlayParticle(middleParticle);
                break;
        }
    }

    void PlayParticle(ParticleSystem particle)
    {
        if (particle != null)
        {
            particle.Play();
        }
        else
        {
            Debug.LogWarning("asset de particula no establecido");
        }
    }
}
