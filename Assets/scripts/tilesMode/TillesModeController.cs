using UnityEngine;
using UnityEngine.InputSystem;

public class TillesModeController : MonoBehaviour
{
    public float toleraciaError = 0.1f;
    public int actualViewLeftPadNotes = 0;
    public int actualViewRigthPadNotes = 0;
    public int actualViewMidlePadNotes = 0;
    public ParticleSystem leftParticle;
    public ParticleSystem rightParticle;
    public ParticleSystem midleParticle;

    public void Update()
    {
        DetectMissLeft();
        DetectMissRigth();
        DetectMissMidle();
    }

    private void DetectMissLeft()
    {
        if (actualViewLeftPadNotes >= SpawnerNotas.instance.timeArriveLeftNotes.Count) return;
        if (SpawnerNotas.instance.timeArriveLeftNotes[actualViewLeftPadNotes] + toleraciaError < TimeController.instance.ActualTime)
        {
            actualViewLeftPadNotes++;
        }
    }

    private void DetectMissRigth()
    {
        if (actualViewRigthPadNotes >= SpawnerNotas.instance.timeArriveRigthNotes.Count) return;
        if (SpawnerNotas.instance.timeArriveRigthNotes[actualViewRigthPadNotes] + toleraciaError < TimeController.instance.ActualTime)
        {
            actualViewRigthPadNotes++;
        }
    }

    private void DetectMissMidle()
    {
        if (actualViewMidlePadNotes >= SpawnerNotas.instance.timeArriveMidleNotes.Count) return;
        if (SpawnerNotas.instance.timeArriveMidleNotes[actualViewMidlePadNotes] + toleraciaError < TimeController.instance.ActualTime)
        {
            actualViewMidlePadNotes++;
        }
    }

    public void OnLeftPad(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (actualViewLeftPadNotes >= SpawnerNotas.instance.timeArriveLeftNotes.Count) return;

            bool MinRange = SpawnerNotas.instance.timeArriveLeftNotes[actualViewLeftPadNotes] >= TimeController.instance.ActualTime - toleraciaError;
            bool MaxRange = SpawnerNotas.instance.timeArriveLeftNotes[actualViewLeftPadNotes] <= TimeController.instance.ActualTime + toleraciaError;
            if (MinRange && MaxRange)
            {
                //print(SpawnerNotas.instance.timeArriveLeftNotes[actualViewLeftPadNotes] - TimeController.instance.ActualTime);
                actualViewLeftPadNotes++;
                leftParticle.Play();
            }
        }
    }

    public void OnRigthpad(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (actualViewRigthPadNotes >= SpawnerNotas.instance.timeArriveRigthNotes.Count) return;

            bool MinRange = SpawnerNotas.instance.timeArriveRigthNotes[actualViewRigthPadNotes] >= TimeController.instance.ActualTime - toleraciaError;
            bool MaxRange = SpawnerNotas.instance.timeArriveRigthNotes[actualViewRigthPadNotes] <= TimeController.instance.ActualTime + toleraciaError;
            if (MinRange && MaxRange)
            {
                //print(SpawnerNotas.instance.timeArriveRigthNotes[actualViewRigthPadNotes] - TimeController.instance.ActualTime);
                actualViewRigthPadNotes++;
                rightParticle.Play();
            }
        }
    }

    public void OnMidlePad(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (actualViewMidlePadNotes >= SpawnerNotas.instance.timeArriveMidleNotes.Count) return;

            bool MinRange = SpawnerNotas.instance.timeArriveMidleNotes[actualViewMidlePadNotes] >= TimeController.instance.ActualTime - toleraciaError;
            bool MaxRange = SpawnerNotas.instance.timeArriveMidleNotes[actualViewMidlePadNotes] <= TimeController.instance.ActualTime + toleraciaError;
            if (MinRange && MaxRange)
            {
                //print(SpawnerNotas.instance.timeArriveMidleNotes[actualViewMidlePadNotes] - TimeController.instance.ActualTime);
                actualViewMidlePadNotes++;
                midleParticle.Play();
            }
        }
    }
}
