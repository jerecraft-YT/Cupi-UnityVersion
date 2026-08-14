using UnityEngine;

public class SpectrumAnalizer : MonoBehaviour
{
    public static SpectrumAnalizer Instance { get; private set;}

    private float[] _spectrumData = new float[256];
    private float musicTime;

    [SerializeField] private AudioSource _musica;

    const float GetMusicEvery = 0.033f;
    const FFTWindow fftwindow = FFTWindow.Rectangular;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //musica = MusicController.mainMusic;
        
    }

    // Update is called once per frame
    private void Update()
    {
        musicTime += Time.deltaTime;

        if (musicTime >= GetMusicEvery)
        {
            _musica.GetSpectrumData(_spectrumData, 0, fftwindow);
            //AudioListener.GetSpectrumData(spectrumData,0, fftwindow);
            musicTime -= GetMusicEvery;
        }
    }

    public float[] SpectrumData => _spectrumData;
}
