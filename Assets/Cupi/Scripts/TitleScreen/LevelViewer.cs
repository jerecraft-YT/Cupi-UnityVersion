using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LevelViewer : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _nivelesDropdown;

    [SerializeField] private TMP_Dropdown _dificultadesDropDown;

    [SerializeField] private BpmController _bpmController;

    [SerializeField] private MusicLoader musicLoader;

    [SerializeField] private TitleScreenManager TitleScreenManager;

    [SerializeField] private TextMeshProUGUI detallesNivel;

    [SerializeField] private string plantillaInfo = "Nombre: {0} \nDescripcion: {1}\nArtista: {2}\nAutor:{3}\nBpm:{4}";

    private string mainDirectory;

    [Tooltip("coleccion de metadatas de los niveles")]
    public List<LevelInfo> levels = new();

    void Start()
    {
        //testGuardado();

        GetLevels();

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

        List<NotaInstance> notaInstances = new() {notaInstance};

        await DataLevelsLoader.SaveAll(notaInstances, "nivelPrueba", testDataLevel, "C:/Users/Alumno/Documents/GitHub/Cupi-UnityVersion/Assets/Cupi/Resources/audiosPrueba/DJ Quads - The Improv (SPOTISAVER).mp3");
    }

    private void GetLevels()
    {
        DataLevelsLoader.FindGameFolder();

        mainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        List<string> direccionesNiveles = new(Directory.GetDirectories(Path.Combine(mainDirectory, DataLevelsLoader.nombreCarpetaJuego)));

        foreach(string levelPath in direccionesNiveles)
        {
            string nombreCarpeta = Path.GetFileName(levelPath.TrimEnd(Path.DirectorySeparatorChar));

            if (DataLevelsLoader.MetadataExists(nombreCarpeta))
            {
                LevelInfo LevelInfo = new(nombreCarpeta, levelPath, DataLevelsLoader.LoadMetadata(nombreCarpeta));

                if (VerificarNiveles(nombreCarpeta, LevelInfo))
                {
                    levels.Add(LevelInfo);
                }
            }
        }
    }

    private bool VerificarNiveles(string nombreCarpeta,LevelInfo levelInfo)
    {
        int nivelesVerificar = levelInfo.levelData.levelsFiles.Count;

        if (nivelesVerificar == 0)
        {
            NivelCorruptoAviso(nombreCarpeta);
            return false;
        }

        foreach (LevelData levelData in levelInfo.levelData.levelsFiles)
        {
            if (!DataLevelsLoader.LevelExist(nombreCarpeta, levelData.levelFileName))
            {
                NivelCorruptoAviso(nombreCarpeta);
                return false;
            }
        }

        return true;
    }

    private void NivelCorruptoAviso(string carpetaCorrupta)
    {
        Debug.LogWarning("NIVEL |" + carpetaCorrupta +"| CORRUPTO");
    }

    public void ChangeOptionSelected(int option)
    {
        LevelMetadata nivel = levels[option].levelData;

        detallesNivel.text = string.Format(plantillaInfo, nivel.name, nivel.description, nivel.artist, nivel.autor, nivel.bpm);

        List<string> dificultades = nivel.levelsFiles.ConvertAll(x => x.nombreDificultad);

        TitleScreenManager.ultimoNivelSeleccionado = option;

        _dificultadesDropDown.ClearOptions();
        _dificultadesDropDown.AddOptions(dificultades);

        musicLoader.MusicChangeRequest(option);
    }
}
