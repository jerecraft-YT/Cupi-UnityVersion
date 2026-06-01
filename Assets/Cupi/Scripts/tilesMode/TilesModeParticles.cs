using UnityEngine;

public class TilesModeParticles : MonoBehaviour
{
    [Header("Particles Hit")]
    [SerializeField] private ParticleSystem leftParticleHit;
    [SerializeField] private ParticleSystem rightParticleHit;
    [SerializeField] private ParticleSystem middleParticleHit;
    [Header("Particles No Hit")]
    [SerializeField] private ParticleSystem leftParticleNoHit;
    [SerializeField] private ParticleSystem rightParticleNoHit;
    [SerializeField] private ParticleSystem middleParticleNoHit;

    private void OnEnable()
    {
        TilesModeController.NoteHit += ParticleHitNote;
        TilesModeController.NoteNoHit += ParticleNoHitNote;
    }

    private void OnDisable()
    {
        TilesModeController.NoteHit -= ParticleHitNote;
        TilesModeController.NoteNoHit -= ParticleNoHitNote;
    }

    private void ParticleHitNote(CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                PlayParticle(leftParticleHit);
                break;
            case CorrespondenciaTecla.Right:
                PlayParticle(rightParticleHit);
                break;
            case CorrespondenciaTecla.Middle:
                PlayParticle(middleParticleHit);
                break;
        }
    }

    private void ParticleNoHitNote(CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                PlayParticle(leftParticleNoHit);
                break;
            case CorrespondenciaTecla.Right:
                PlayParticle(rightParticleNoHit);
                break;
            case CorrespondenciaTecla.Middle:
                PlayParticle(middleParticleNoHit);
                break;
        }
    }
    private void PlayParticle(ParticleSystem particle)
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
