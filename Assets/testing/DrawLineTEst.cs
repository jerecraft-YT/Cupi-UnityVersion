using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class DrawLineTEst : MonoBehaviour
{
    public LineData line;
    public int numberPoints = 10;
    public int withLine = 100;

    public float maximoAnguloAProcesar = 360;
    public float maximoAnguloSimpleAProcesar = 360;
    public float amplitud = 5.0f;
    public float amplitudSimple = 5.0f;
    public float amplitudGeneral = 1f;
    public float multiplicadorTiempo = 1f;

    public bool desplazarConElTiempo = false;
    public bool animarAmplitud = true;
    public int myIndex;
    private float time;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = new LineData();

        MyLineRenderer.instance.AddLine(line,numberPoints + 1,transform, myIndex,0.25f);

        //line.positionCount = numberPoints + 1;

        float progresoWith = withLine / (float)numberPoints;

        for (int i = 0; i < numberPoints + 1; i++)
        {
            //line.SetPosition(i, new Vector2(progresoWith * i, 0.0f));
            line.points[i] = new Vector2(progresoWith * i, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime * multiplicadorTiempo;

        float progressAngle = maximoAnguloAProcesar / numberPoints;
        float finalAMP = amplitud;
        float moveTime = 0.0f;
        float progressAngleSimple = maximoAnguloSimpleAProcesar / numberPoints;

        if (desplazarConElTiempo)
        {
            moveTime = time;
        }
        if (animarAmplitud)
        {
            if (Mathf.Cos(time) > 0)
            {
                finalAMP = amplitud * Mathf.Cos(time);
            }
            else
            {
                finalAMP = amplitud * -(Easing.OutCubic(-Mathf.Cos(time)));
            }

        }


        for (int i = 0; i < numberPoints + 1; i++)
        {
            line.points[i] = new Vector2(line.points[i].x, ((Mathf.Cos(moveTime + ((progressAngle * i) * Mathf.Deg2Rad)) * finalAMP) +
                (Mathf.Cos(moveTime + ((progressAngleSimple * i) * Mathf.Deg2Rad)) * amplitudSimple)
                ) * amplitudGeneral);
            /*
            line.SetPosition(i, new Vector2(line.GetPosition(i).x, ((Mathf.Cos(moveTime + ((progressAngle * i) * Mathf.Deg2Rad)) * finalAMP) +
                (Mathf.Cos(moveTime + ((progressAngleSimple * i) * Mathf.Deg2Rad)) * amplitudSimple)
                ) * amplitudGeneral));
            */
        }

        line.updateLine = true;

    }
}
