using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class metaballFollow : MonoBehaviour
{
    private SpriteShapeController controller;
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

        if (metaball.instance.MetaballController.spline.GetPointCount() != 0)
        {
            if (!m_Follow)
            {
                m_Follow = true;
                for (int i = 0; i < metaball.instance.NumberPoints; i++)
                {
                    controller.spline.InsertPointAt(i, metaball.instance.MetaballController.spline.GetPosition(i));
                    controller.spline.SetTangentMode(i, ShapeTangentMode.Continuous);

                    controller.spline.SetLeftTangent(i, metaball.instance.TangentPositions[i]);
                    controller.spline.SetRightTangent(i, -metaball.instance.TangentPositions[i]);
                }
                amplitudPoints = (float[])metaball.instance.AmplitudPoints.Clone();
            }

            for (int i = 0; i < metaball.instance.NumberPoints; i++)
            {   
                amplitudPoints[i] = amplitudPoints[i] < metaball.instance.AmplitudPoints[i] ? metaball.instance.AmplitudPoints[i] : amplitudPoints[i];

                amplitudPoints[i] = amplitudPoints[i] > metaball.instance.AmplitudPoints[i] ? Mathf.Lerp(amplitudPoints[i], metaball.instance.Amplitud, velocidadSeguimiento * Time.deltaTime) : amplitudPoints[i];
                
                controller.spline.SetPosition(i, metaball.instance.DirectionPoint[i] * amplitudPoints[i] +(metaball.instance.DirectionPoint[i] * metaball.instance.AmplitudNoise[i]));
            }

            if (refreshTime >= metaball.instance.RefreshEvery)
            {
                refreshTime -= metaball.instance.RefreshEvery;
                controller.RefreshSpriteShape();
            }
        }
    }
}
