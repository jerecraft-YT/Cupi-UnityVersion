using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class metaball : MonoBehaviour
{
    #region variables
    #region variablesExternas
    private SpriteShapeController metaballController;
    private Vector2[] directionPoint;
    private float[] amplitudPoints;
    private float[] amplitudNoise;
    private Vector2[] tangentPositions;
    #endregion

    private float[] objetivoPoints;
    private float[] spectrumData = new float[256];
    private float musicTime;
    private float baseAngle;
    [Header("Config")]

    [Header("Metaball Config")]
    [SerializeField] private int numberPoints = 16;
    [SerializeField] private float velocidadRotacion = 0.1f;
    [SerializeField] private float velocidadAmplitud = 20.0f;
    [SerializeField] private float velocidadAmplitudObjetivo = 1.0f;
    [SerializeField] private float amplitud = 6.0f;
    [SerializeField] private float tangentAmplitud = 1.0f;
    [SerializeField] private float metaballScale = 0.8f;
    [SerializeField] private float fuerzaMusica = 40.0f;
    [SerializeField] private float fuerzaMaxima = 10.0f;

    [SerializeField] private int maxFollow = 4;

    [ColorUsage(showAlpha:true,hdr:true)]
    [SerializeField] private Color[] followColors;
    [SerializeField] private GameObject followAsset;

    [SerializeField] private float refreshEvery = 0.016f;
    [SerializeField] private float noiseInfluence = 0.2f;
    [SerializeField] private float noiseReadingFrequency = 0.18f;
    private float refreshTime;

    [Range(0f, 10.0f)]
    [SerializeField] private float[] FuerzaPorPunto = new float[16] {
        0.15f,0.65f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f
    };

    [Header("Music Config")]

    [Range(0f, 1f)]
    [SerializeField] private float SpectrumSize = 0.185f;

    [SerializeField] private float GetMusicEvery = 0.033f;
    #endregion

    #region core
    private void Awake()
    {
        metaballController = GetComponent<SpriteShapeController>();

        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }

        CreateCirclePoints();
        CreateMetaballFollowInstances();
    }

    private bool ValidateComponents()
    {
        if (followAsset == null && maxFollow != 0)
        {
            Debug.LogError("follow asset no establecido");
            return false;
        }

        if (metaballController == null)
        {
            Debug.LogError("metaballController no establecido");
            return false;
        }
        return true;
    }

    private void CreateMetaballFollowInstances()
    {
        for (int i = 0; i < maxFollow; i++)
        {
            GameObject metaballFollowInstance = Instantiate(followAsset,transform,false);

            metaballFollow followScript = metaballFollowInstance.GetComponent<metaballFollow>();
            followScript.mainMetaball = this;
            SpriteShapeRenderer followRenderer = metaballFollowInstance.GetComponent<SpriteShapeRenderer>();

            followScript.velocidadSeguimiento = maxFollow - i;
            followRenderer.sortingOrder = -(i + 1);
            if (followColors.Length != 0)
            {
                followRenderer.color = followColors[Mathf.Min(i, followColors.Length - 1)];
            }
            else
            {
                followRenderer.color = Color.blue;
            }
            
        }
    }

    private void Update()
    {
        RotateCircle();
        AddLocalTime();
        MoveCircle();
    }
    #endregion

    private void AddLocalTime()
    {
        musicTime += Time.deltaTime;
        refreshTime += Time.deltaTime;
    }

    //anima la metaball siguiendo el espectro de audio
    private void MoveCircle()
    {
        if (SpectrumSize > 1 || SpectrumSize < 0.01)
        {
            Debug.LogError("spectrum size no debe ser mayor a 1 o menor a 0.01f");
            SpectrumSize = Mathf.Clamp(SpectrumSize, 0.01f, 1);
        }

        int rangoSpectro = Mathf.Max(1,(int)((spectrumData.Length * SpectrumSize)) / numberPoints);

        if (musicTime >= GetMusicEvery)
        {
            spectrumData = SpectrumAnalizer.Instance.SpectrumData;
        }

        for (int i = 0; i < numberPoints; i++)
        {
            if (musicTime >= GetMusicEvery)
            {
                float avg;
                float sum = 0;

                for (int j = 0; j < rangoSpectro; j++)
                {
                    int index = rangoSpectro * i + j;

                    if (index >= spectrumData.Length) break;

                    sum += spectrumData[index];
                }

                avg = sum / rangoSpectro;
                float fuerzaPunto = FuerzaPorPunto.Length > 0 ? FuerzaPorPunto[Mathf.Min(i, FuerzaPorPunto.Length - 1)] : 1f;
                objetivoPoints[i] = Mathf.Min(amplitud + avg * fuerzaMusica * fuerzaPunto, fuerzaMaxima);
            }
            else
            {
                objetivoPoints[i] = Mathf.Lerp(objetivoPoints[i], amplitud, velocidadAmplitudObjetivo * Time.deltaTime);
            }

            amplitudNoise[i] = Mathf.PerlinNoise(i * noiseReadingFrequency, Time.time) * noiseInfluence;

            amplitudPoints[i] = Mathf.Max(Mathf.Lerp(amplitudPoints[i], objetivoPoints[i], velocidadAmplitud * Time.deltaTime), amplitud);

            metaballController.spline.SetPosition(i, metaballScale * amplitudPoints[i] * directionPoint[i] + (directionPoint[i] * amplitudNoise[i]));

            float tangentScale = amplitudPoints[i] / amplitud;
            metaballController.spline.SetLeftTangent(i, tangentPositions[i] * tangentScale);
            metaballController.spline.SetRightTangent(i, -tangentPositions[i] * tangentScale);
        }

        if (refreshTime >= refreshEvery)
        {
            metaballController.RefreshSpriteShape();
            refreshTime -= refreshEvery;
        }
        
        if (musicTime >= GetMusicEvery)
        {
            musicTime -= GetMusicEvery;
        }
    }

    //rota el circulo para darle mas variedad
    private void RotateCircle()
    {
        baseAngle += velocidadRotacion * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, baseAngle);
        
    }

    /// <summary>
    /// sirve para recalcular los datos del circulo para datos que solo se calculan una vez
    /// </summary>
    public void CreateCirclePoints()
    {
        Spline spline = metaballController.spline;

        //segun investigue esto deja basura y no es eficiente
        //aunque no me queda de otra para evitar que crashee
        metaballController.spline.Clear();

        //vaya resulta que esto que es la soluciona mas eficiente causa crasheos en esta version, increible
        /*
        while (spline.GetPointCount() > 0)
        {
            spline.RemovePointAt(spline.GetPointCount() - 1);
        }
        */

        float progresoPorIteracion = 360.0f / numberPoints;

        tangentPositions = new Vector2[numberPoints];
        directionPoint = new Vector2[numberPoints];     
        amplitudPoints = new float[numberPoints];
        objetivoPoints = new float[numberPoints];
        amplitudNoise = new float[numberPoints];

        for (int i = 0; i < numberPoints; i++)
        {
            float angle = progresoPorIteracion * i;
            float radAngle = angle * Mathf.Deg2Rad;
            float tangentAnglerad = (angle - 90.0f) * Mathf.Deg2Rad;

            Vector2 pointAngle = new Vector2(MathF.Cos(radAngle) , -MathF.Sin(radAngle));
            Vector3 positionTangent = new Vector3(MathF.Cos(tangentAnglerad) * tangentAmplitud, -MathF.Sin(tangentAnglerad) * tangentAmplitud, 0.0f);

            spline.InsertPointAt(i , new Vector3(pointAngle.x * amplitud, pointAngle.y * amplitud, 0.0f) );
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);

            spline.SetLeftTangent(i, positionTangent);
            spline.SetRightTangent(i, -positionTangent);

            directionPoint[i] = pointAngle;
            amplitudPoints[i] = amplitud;
            objetivoPoints[i] = amplitud;
            tangentPositions[i] = positionTangent;
        }

        spline.isOpenEnded = false;
        metaballController.RefreshSpriteShape();
    }


    public float[] AmplitudPoints => amplitudPoints;

    public float[] AmplitudNoise => amplitudNoise;

    public float Amplitud => amplitud;

    public Vector2[] TangentPositions => tangentPositions;

    public Vector2[] DirectionPoint => directionPoint;

    public SpriteShapeController MetaballController => metaballController;

    public float RefreshEvery => refreshEvery;

    public int NumberPoints
    {
        get { return numberPoints; }
        set {
            //limita el numero de puntos a algo que no haga chillar al especialito de sprite shape
            value = Mathf.Clamp(value, 3, 64);

            if (value != numberPoints)
            {
                numberPoints = value;
                CreateCirclePoints();
            }
        }
    }

}