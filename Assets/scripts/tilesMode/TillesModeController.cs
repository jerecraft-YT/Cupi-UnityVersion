using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class TillesModeController : MonoBehaviour
{
    public float toleraciaError = 0.1f;
    public int actualViewLeftPadNotes = 0;
    public int actualViewRigthPadNotes = 0;
    public int actualViewMidlePadNotes = 0;
    public ParticleSystem leftParticle;
    public ParticleSystem rightParticle;
    public ParticleSystem midleParticle;

    public Action<CorrespondenciaTecla> PadClick;


    private void OnEnable()
    {
        PadClick += ButtonClicked;
    }
    private void Update()
    {
        DetectMissLeft();
        DetectMissRigth();
        DetectMissMidle();
    }

    private void DetectMissLeft()
    {
        DetectMissNote(ref actualViewLeftPadNotes, SpawnerNotas.instance.timeArriveLeftNotes);
    }

    private void DetectMissRigth()
    {
        DetectMissNote(ref actualViewRigthPadNotes, SpawnerNotas.instance.timeArriveRigthNotes);
    }

    private void DetectMissMidle()
    {
        DetectMissNote(ref actualViewMidlePadNotes, SpawnerNotas.instance.timeArriveMidleNotes);
    }

    private void DetectMissNote(ref int index, List<float> notesGroup)
    {
        if (index >= notesGroup.Count) return;
        if (notesGroup[index] + toleraciaError < TimeController.instance.ActualTime)
        {
            index++;
        }
    }
    private void DetectHitNote(ref int index, List<float> notesGroup, ParticleSystem particle)
    {
        if (index >= notesGroup.Count) return;

        bool MinRange = notesGroup[index] >= TimeController.instance.ActualTime - toleraciaError;
        bool MaxRange = notesGroup[index] <= TimeController.instance.ActualTime + toleraciaError;
        if (MinRange && MaxRange)
        {
            index++;
            particle.Play();
        }
    }

    public void OnLeftPad(InputAction.CallbackContext context)
    {
        if (context.performed) PadClick?.Invoke(CorrespondenciaTecla.Left);
    }

    public void OnRigthpad(InputAction.CallbackContext context)
    {
        if (context.performed) PadClick?.Invoke(CorrespondenciaTecla.Right);
    }

    public void OnMidlePad(InputAction.CallbackContext context)
    {
        if (context.performed) PadClick?.Invoke(CorrespondenciaTecla.Midle);
    }

    public void ButtonClicked(CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                DetectHitNote(ref actualViewLeftPadNotes, SpawnerNotas.instance.timeArriveLeftNotes, leftParticle);
                break;

            case CorrespondenciaTecla.Right:
                DetectHitNote(ref actualViewRigthPadNotes, SpawnerNotas.instance.timeArriveRigthNotes, rightParticle);
                break;

            case CorrespondenciaTecla.Midle:
                DetectHitNote(ref actualViewMidlePadNotes, SpawnerNotas.instance.timeArriveMidleNotes, midleParticle);
                break;
        }
    }
}
