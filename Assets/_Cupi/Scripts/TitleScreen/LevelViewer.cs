using System.Collections.Generic;
using TMPro;
using UnityEngine;

using CupiEngine.ResourceLoader.Levels;
using CupiEngine.ResourceLoader.Audio;

public class LevelViewer : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _nivelesDropdown;
    [SerializeField] private TMP_Dropdown _dificultadesDropDown;
    [SerializeField] private BpmController _bpmController;
    //[SerializeField] private MusicLoader musicLoader;
    [SerializeField] private TitleScreenManager TitleScreenManager;
    [SerializeField] private TextMeshProUGUI detallesNivel;
    [SerializeField] private string plantillaInfo = "Nombre: {0} \nDescripcion: {1}\nArtista: {2}\nAutor:{3}\nBpm:{4}";

    [SerializeField] private LevelDataSO levelData;

    [Tooltip("coleccion de metadatas de los niveles")]
    public List<LevelInfo> levels = new();

   

    void Start()
    {
        //testGuardado();

        levels = CupiLevelsLoader.LoadListLevels(levelData);
        
        //GetLevels();

        if (levels.Count != 0)
        {
            ShowLevels();
            ChangeOptionSelected(0);
        }
    }

    private void ShowLevels()
    {

        List<string> levelsName = levels.ConvertAll(x => x.levelData.name);

        _nivelesDropdown.AddOptions(levelsName);
    }

    private async void TestGuardado()
    {
        LevelData levelData = new("dificultad 1",100,"miNivel.json");

        LevelMetadata testDataLevel = new LevelMetadata();

        testDataLevel.name = "NombreNivel";
        testDataLevel.artist = "ArtistaNivel";
        testDataLevel.autor = "AutorNivel";
        testDataLevel.bpm = 123;
        testDataLevel.previewTimeMusic = 0;
        //testDataLevel.MusicFileName = "DireccionNivel(local y con extension)";
        testDataLevel.description = "DescripcionNivel";
        testDataLevel.tags = "TagsSeparadosPor(|)";
        testDataLevel.levelsFiles = new(){levelData};

        //DataLevelsLoader.SaveMetadata(testDataLevel, "nivel");

        NotaInstance notaInstance = new();
        notaInstance.tipoNota = TipoNota.Normal;
        notaInstance.duracion = 0;
        notaInstance.correspondenciaTecla = CorrespondenciaTecla.One;
        notaInstance.timeToArrive = 1;
        notaInstance.direccionMovimiento = DireccionesMovimientoNotas.Up;
        notaInstance.localSpeed = 1;

        List<NotaInstance> notaInstances = new() { notaInstance };

        await CupiLevelsLoader.SaveAll(notaInstances, "nivelPrueba", testDataLevel, "C:/Users/Alumno/Documents/GitHub/Cupi-UnityVersion/Assets/Cupi/Resources/audiosPrueba/DJ Quads - The Improv (SPOTISAVER).mp3");
    }

    public void ChangeOptionSelected(int option)
    {
        LevelMetadata nivel = levels[option].levelData;

        detallesNivel.text = string.Format(plantillaInfo, nivel.name, nivel.description, nivel.artist, nivel.autor, nivel.bpm);

        List<string> dificultades = nivel.levelsFiles.ConvertAll(x => x.nombreDificultad);

        TitleScreenManager.ultimoNivelSeleccionado = option;

        _dificultadesDropDown.ClearOptions();
        _dificultadesDropDown.AddOptions(dificultades);

        CupiMusicLoader.MusicChangeRequest(levels[option],_bpmController);
    }
}
