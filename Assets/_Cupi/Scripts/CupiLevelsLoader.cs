using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

namespace CupiEngine.ResourceLoader.Levels
{
    public static class CupiLevelsLoader
    {
        public static string nombreCarpetaJuego = "CUPI";
        public static int errorMusicLoad = -1;
        //public static LevelDataSO levelData;

        #region PrivateCommon

        /// <summary>
        /// verifica si levelData esta establecido antes de iniciar
        /// </summary>
        /*
        private static void CheckLevelData()
        {
            if (levelData != null) return;

            levelData = Resources.Load<LevelDataSO>("ScriptableObjects/LevelData");
        }
        */

        /// <summary>
        /// verifica de antemano si la carpeta del juego ya existe 
        /// para hacer las verificaciones necesarias sin problema
        /// </summary>
        private static void CheckGameFolder()
        {
            string dir = Path.Combine(MainPath, nombreCarpetaJuego);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        private static void FindLevelFolder(string folderName)
        {
            string dir = Path.Combine(MainPath, nombreCarpetaJuego, folderName);

            if (!Directory.Exists(dir))
            {
                Debug.Log("crear carpeta");
                Directory.CreateDirectory(dir);
            }
        }
        private static void NivelCorruptoAviso(string carpetaCorrupta)
        {
            Debug.LogWarning("NIVEL |" + carpetaCorrupta + "| CORRUPTO");
        }

        private static bool LevelExist(string folderName, string levelName)
        {
            string dir = Path.Combine(MainPath, nombreCarpetaJuego, folderName, levelName);
            return File.Exists(dir);
        }
        private static bool MetadataExists(string levelName)
        {
            string dataName = levelName + ".meta";
            string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, dataName);
            return File.Exists(dir);
        }

        #endregion

        #region Common
        public static void SetAllLevelData(LevelDataSO levelData, string levelName, LevelMetadata levelMetadata, string levelFolder)
        {
            //CheckLevelData();
            SetLevelFolder(levelData,levelFolder);
            SetLevelName(levelData,levelName);
            SetLevelMetadata(levelData,levelMetadata);
        }
        public static void SetLevelMetadata(LevelDataSO levelData, LevelMetadata levelMetadata)
        {
            //CheckLevelData();
            levelData.levelMetadata = levelMetadata;
        }
        public static void SetLevelName(LevelDataSO levelData, string levelName)
        {
            levelData.levelName = levelName;
        }

        public static void SetLevelFolder(LevelDataSO levelData, string levelFolder)
        {
            levelData.levelFolder = levelFolder;
        }

        /// <summary>
        /// Define el chart del nivel de acuerdo a los datos de nombre,carpeta y metadata
        /// </summary>
        public static void SetLevelChart(LevelDataSO levelData)
        {
            levelData.levelChart = LoadDataLevel(levelData);
        }

        #endregion

        #region Save 

        //si es una task es mas facil de saber cuando acabo en vez de ponerle variables de end
        public static async Task SaveAll(List<NotaInstance> notasToSave, string levelName, LevelMetadata metadata, string musicOriginalPath)
        {
            Debug.Log("-----GUARDANDO NIVEL COMPLETO-----");

            foreach (LevelData levelData in metadata.levelsFiles)
            {
                Debug.Log("guardando nivel");
                await SaveLevel(notasToSave, levelName, levelData.levelFileName);
            }

            Debug.Log("guardando musica");
            await SaveMusic(musicOriginalPath, levelName);

            if (errorMusicLoad != -1)
            {
                errorMusicLoad = -1;
            }
            else
            {
                Debug.Log("se sobreescribio la direccion de la musica en metadata");
                string fileName = Path.GetFileName(musicOriginalPath);
                metadata.musicFileName = fileName;
            }

            Debug.Log("guardando metadata");
            await SaveMetadata(metadata, levelName);

            Debug.Log("-----TERMINO DE GUARDAR NIVEL COMPLETO-----");
        }

        public static async Task SaveLevel(List<NotaInstance> notasToSave, string levelName, string fileName)
        {
            CheckGameFolder();
            FindLevelFolder(levelName);

            MainLevel LevelToSave = new(notasToSave);

            string JsonString = JsonUtility.ToJson(LevelToSave, true);

            string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, fileName);

            await Task.Run(() =>
            {
                File.WriteAllText(dir, JsonString);
            });
            Debug.Log("se guardo el nivel");
        }

        public static async Task SaveMusic(string originalPath, string levelName)
        {
            CheckGameFolder();
            FindLevelFolder(levelName);

            string fileName = Path.GetFileName(originalPath);
            string finalDir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, fileName);

            if (!File.Exists(originalPath))
            {
                errorMusicLoad = 1;
                Debug.LogWarning("direccion de musica no valido");
                return;
            }

            if (File.Exists(finalDir))
            {
                Debug.LogWarning("el archivo ya existe en la carpeta de nivel");
                return;
            }

            await Task.Run(() =>
            {
                File.Copy(originalPath, finalDir, true);
            });
        }

        public static async Task SaveMetadata(LevelMetadata metadata, string levelName)
        {
            CheckGameFolder();
            FindLevelFolder(levelName);

            string dataName = levelName + ".meta";

            string JsonString = JsonUtility.ToJson(metadata, true);

            string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, dataName);

            await Task.Run(() =>
            {
                File.WriteAllText(dir, JsonString);
            });

            Debug.Log("se guardo la metadata");
        }

        #endregion

        #region Load

        /// <summary>
        /// carga el chart del nivel actual contenido en <see cref="LevelDataSO"/>
        /// </summary>
        private static MainLevel LoadDataLevel(LevelDataSO levelData)
        {
            string folderName = levelData.levelFolder;
            string levelName = levelData.levelName;

            string dir = Path.Combine(MainPath, nombreCarpetaJuego, folderName, levelName);

            if (!File.Exists(dir))
            {
                return new(new());
            }

            MainLevel notas;

            string JsonString = File.ReadAllText(dir);

            //hacer esto con task.run es pegriloso :c
            notas = JsonUtility.FromJson<MainLevel>(JsonString);

            return notas;
        }

        /// <summary>
        /// carga y retorna la lista de niveles en la carpeta Documentos/Cupi
        /// </summary>
        public static List<LevelInfo> LoadListLevels(LevelDataSO levelData)
        {
            CheckGameFolder();

            List<LevelInfo> levels = new();

            List<string> direccionesNiveles = new(Directory.GetDirectories(Path.Combine(MainPath, nombreCarpetaJuego)));

            foreach (string levelPath in direccionesNiveles)
            {
                string nombreCarpeta = Path.GetFileName(levelPath.TrimEnd(Path.DirectorySeparatorChar));

                if (MetadataExists(nombreCarpeta))
                {
                    SetLevelFolder(levelData, nombreCarpeta);
                    SetLevelName(levelData, nombreCarpeta);

                    LevelInfo LevelInfo = new(nombreCarpeta, levelPath, LoadMetadata(levelData));

                    if (VerificarNivel(nombreCarpeta, LevelInfo))
                    {
                        levels.Add(LevelInfo);
                        continue;
                    }

                    //en caso no se pudo verificar reporta error
                    NivelCorruptoAviso(nombreCarpeta);
                    continue;
                }

                // en caso que no exista la metadata reporta error
                NivelCorruptoAviso(nombreCarpeta);
            }

            return levels;
        }

        /// <summary>
        /// Verifica si la carpeta de nivel obtiene datos validos
        /// </summary>
        private static bool VerificarNivel(string nombreCarpeta, LevelInfo levelInfo)
        {
            //CheckLevelData();

            int nivelesVerificar = levelInfo.levelData.levelsFiles.Count;

            if (nivelesVerificar == 0)
            {
                NivelCorruptoAviso(nombreCarpeta);
                return false;
            }

            foreach (LevelData levelData in levelInfo.levelData.levelsFiles)
            {
                if (!LevelExist(nombreCarpeta, levelData.levelFileName))
                {
                    NivelCorruptoAviso(nombreCarpeta);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// carga la metadata del nivel actual contenido en <see cref="LevelDataSO"/> 
        /// </summary>
        public static LevelMetadata LoadMetadata(LevelDataSO levelData)
        {
            //CheckLevelData();

            string levelName = levelData.levelName;
            string dataName = levelName + ".meta";

            string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, dataName);

            if (!File.Exists(dir))
            {
                Debug.LogWarning("el archivo no existe |" + dir);
                return new LevelMetadata();
            }

            string JsonString = File.ReadAllText(dir);

            return JsonUtility.FromJson<LevelMetadata>(JsonString);
        }

        #endregion

        /// <summary>
        /// direccion de la carpeta documentos (base para cargar y guardar)
        /// </summary>
        public static string MainPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
}
