using UnityEngine;

public class SpectrumAnalizer : MonoBehaviour
{
    public static SpectrumAnalizer instance;

    public float[] spectrumData = new float[256];
    private float musicTime;

    public AudioSource musica;

    const float GetMusicEvery = 0.033f;
    const FFTWindow fftwindow = FFTWindow.Rectangular;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        musica = MusicController.mainMusic;
    }

    // Update is called once per frame
    private void Update()
    {
        musicTime += Time.deltaTime;

        if (musicTime >= GetMusicEvery)
        {
            musica.GetSpectrumData(spectrumData, 0, fftwindow);
            musicTime -= GetMusicEvery;
        }
    }

    public float[] SpectrumData => spectrumData;
}
