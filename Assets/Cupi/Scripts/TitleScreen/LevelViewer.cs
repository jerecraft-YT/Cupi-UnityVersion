using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LevelViewer : MonoBehaviour
{
    public TMP_Dropdown nivelesDropdown;

    public TMP_Dropdown dificultadesDropDown;

    public BpmController bpmController;

    private string mainDirectory;

    [SerializeField] private MusicLoader musicLoader;
    [SerializeField] private TitleScreenManager TitleScreenManager;

    public TextMeshProUGUI detallesNivel;

    [Tooltip("coleccion de metadatas de los niveles")]
    public List<LevelInfo> levels = new();

    public string plantillaInfo = "Nombre: {0} \nDescripcion: {1}\nArtista: {2}\nAutor:{3}\nBpm:{4}";

    void Start()
    {
        testGuardado();

        GetLevels();

        if (levels.Count != 0)
        {
            showLevels();
            ChangeOptionSelected(0);
        }
    }

    private void showLevels()
    {

        List<string> levelsName = levels.ConvertAll(x => x.levelData.Name);

        nivelesDropdown.AddOptions(levelsName);
    }

    private void testGuardado()
    {
        LevelData levelData = new("dificultad 1",100,"miNivel.json");

        List<LevelData> niveles = new(){levelData};

        LevelMetadata testDataLevel = new LevelMetadata();

        testDataLevel.Name = "NombreNivel";
        testDataLevel.Artist = "ArtistaNivel";
        testDataLevel.Autor = "AutorNivel";
        testDataLevel.Bpm = 123;
        testDataLevel.PreviewTimeMusic = 0;
        testDataLevel.MusicFileName = "DireccionNivel(local y con extension)";
        testDataLevel.Description = "DescripcionNivel";
        testDataLevel.Tags = "TagsSeparadosPor(|)";
        testDataLevel.LevelsFiles = new(){levelData};

        //DataLevelsLoader.SaveMetadata(testDataLevel, "nivel");

        NotaInstance notaInstance = new NotaInstance();
        notaInstance.tipoNota = TipoNota.Normal;
        notaInstance.duracion = 0;
        notaInstance.CorrespondenciaTecla = CorrespondenciaTecla.One;
        notaInstance.timeToArrive = 1;
        notaInstance.DireccionMovimiento = DireccionesMovimientoNotas.Up;

        List<NotaInstance> notaInstances = new() { notaInstance};

        DataLevelsLoader.SaveAll(notaInstances, "nivelPrueba", testDataLevel,"test");
    }

    private void GetLevels()
    {
        DataLevelsLoader.FindGameFolder();

        mainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        List<string> direccionesNiveles = new List<string>(Directory.GetDirectories(Path.Combine(mainDirectory, DataLevelsLoader.nombreCarpetaJuego)));

        foreach(string levelPath in direccionesNiveles)
        {
            string nombreCarpeta = Path.GetFileName(levelPath.TrimEnd(Path.DirectorySeparatorChar));

            if (DataLevelsLoader.MetadataExists(nombreCarpeta))
            {
                LevelInfo LevelInfo = new LevelInfo(nombreCarpeta, levelPath, DataLevelsLoader.LoadMetadata(nombreCarpeta));

                if (VerificarNiveles(nombreCarpeta, LevelInfo))
                {
                    levels.Add(LevelInfo);
                }

            }
        }
    }

    private bool VerificarNiveles(string nombreCarpeta,LevelInfo levelInfo)
    {
        int nivelesVerificar = levelInfo.levelData.LevelsFiles.Count;

        if (nivelesVerificar == 0)
        {
            NivelCorruptoAviso(nombreCarpeta);
            return false;
        }

        foreach (LevelData _levelData in levelInfo.levelData.LevelsFiles)
        {
            if (!DataLevelsLoader.LevelExist(nombreCarpeta, _levelData.levelFileName))
            {
                NivelCorruptoAviso(nombreCarpeta);
                return false;
            }
        }

        return true;
    }

    private void NivelCorruptoAviso(string carpetaCorrupta)
    {
        Debug.LogWarning("nivel en la carpeta |" + carpetaCorrupta +"| corrupto");
    }


    public void ChangeOptionSelected(int option)
    {
        LevelMetadata nivel = levels[option].levelData;

        detallesNivel.text = string.Format(plantillaInfo, nivel.Name, nivel.Description, nivel.Artist, nivel.Autor, nivel.Bpm);

        List<string> dificultades = nivel.LevelsFiles.ConvertAll(x => x.nombreDificultad);

        TitleScreenManager.ultimoNivelSeleccionado = option;

        dificultadesDropDown.ClearOptions();
        dificultadesDropDown.AddOptions(dificultades);

        musicLoader.MusicChangeRequest(option);
    }
}
