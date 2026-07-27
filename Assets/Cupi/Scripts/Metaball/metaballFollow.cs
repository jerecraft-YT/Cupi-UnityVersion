using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class metaballFollow : MonoBehaviour
{
    private SpriteShapeController controller;
    public metaball mainMetaball;
    private bool m_Follow = false;
    private float[] amplitudPoints;
    public float velocidadSeguimiento = 5.0f;
    private float refreshTime;

    void Start()
    {
        controller = GetComponent<SpriteShapeController>();
        controller.spline.Clear();
    }
    void Update()
    {
        refreshTime += Time.deltaTime;

        if (mainMetaball.MetaballController.spline.GetPointCount() != 0)
        {
            if (!m_Follow)
            {
                m_Follow = true;
                for (int i = 0; i < mainMetaball.NumberPoints; i++)
                {
                    controller.spline.InsertPointAt(i, mainMetaball.MetaballController.spline.GetPosition(i));
                    controller.spline.SetTangentMode(i, ShapeTangentMode.Continuous);

                    controller.spline.SetLeftTangent(i, mainMetaball.TangentPositions[i]);
                    controller.spline.SetRightTangent(i, -mainMetaball.TangentPositions[i]);
                }
                amplitudPoints = (float[])mainMetaball.AmplitudPoints.Clone();
            }

            for (int i = 0; i < mainMetaball.NumberPoints; i++)
            {   
                amplitudPoints[i] = amplitudPoints[i] < mainMetaball.AmplitudPoints[i] ? mainMetaball.AmplitudPoints[i] : amplitudPoints[i];

                amplitudPoints[i] = amplitudPoints[i] > mainMetaball.AmplitudPoints[i] ? Mathf.Lerp(amplitudPoints[i], mainMetaball.Amplitud, velocidadSeguimiento * Time.deltaTime) : amplitudPoints[i];
                
                controller.spline.SetPosition(i, mainMetaball.DirectionPoint[i] * amplitudPoints[i] +(mainMetaball.DirectionPoint[i] * mainMetaball.AmplitudNoise[i]));
            }

            if (refreshTime >= mainMetaball.RefreshEvery)
            {
                refreshTime -= mainMetaball.RefreshEvery;
                controller.RefreshSpriteShape();
            }
        }
    }
}
