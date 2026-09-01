using System;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cupi.DebugTools
{
    public class DebugController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textoDebug;
        [SerializeField] private float _timeUpdateDebugInfo;

        private AudioSource _audioSource;

        private bool _updateDebugInfo = true;

        private const string TEXT_DEBUG_INFO = "UnscaledCustomTime: {0:N2}\nCustomTime: {1:N2} \nMusicTime: {2:N2}\nTimeScale: {3:N2}";

        [HideInInspector] public AudioClip clip;

        private void Start()
        {
            MusicController.OnMusicChanged += setMainMusic;

            InputController.instance.gameInputs.UI.Enable();

            InputController.instance.gameInputs.UI.ScrollWheel.performed += ScrollMouse;
        }

        private void OnDisable()
        {
            MusicController.OnMusicChanged -= setMainMusic;
        }

        private void setMainMusic()
        {
            _audioSource = MusicController.mainMusic.clip != null ? MusicController.mainMusic : null;
        }

        private async void Update()
        {
            if (_updateDebugInfo)
            {
                _updateDebugInfo = false;
                await UpdateDebug();
            }
        }

        private void ScrollMouse(InputAction.CallbackContext ctx)
        {
            float timeScale = TimeController.instance.GetCurrentTimeScale();

            //TimeController.instance.SetTimeScale(timeScale + ctx.ReadValue<Vector2>().y * 0.1f);

            ChangeTimeScale(timeScale + ctx.ReadValue<Vector2>().y * 0.1f);
        }

        private async Task UpdateDebug()
        {
            _textoDebug.text = string.Format(
                TEXT_DEBUG_INFO,
                TimeController.instance.ActualTime,
                TimeController.instance.AdditiveTime,
                _audioSource != null ? _audioSource.time : "no hay musica XD",
                TimeController.instance.TimeScale
                );

            await Awaitable.WaitForSecondsAsync(_timeUpdateDebugInfo);

            _updateDebugInfo = true;
        }
        public void ChangeTimeScale(float valor)
        {
            //redondea el valor a solo 2 digitos
            float value = (float)Math.Round(valor, 2);

            TimeController.instance.TimeScale = value;

            if (_audioSource == null) return;
            _audioSource.pitch = value;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DebugController))]
    public class DebugControllerCustomEditor : Editor
    {
        private SerializedProperty newAudioClip;

        private void OnEnable()
        {
            newAudioClip = serializedObject.FindProperty("clip");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //if (!Application.isPlaying) return;

            serializedObject.Update();

            AudioClip newClip = newAudioClip.objectReferenceValue as AudioClip;

            EditorGUILayout.Space(15f);

            EditorGUILayout.LabelField("Debug Tools");

            EditorGUILayout.Space(15f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Audio Clip", GUILayout.Width(70f));

            EditorGUILayout.PropertyField(newAudioClip, GUIContent.none);

            //MusicController music = (MusicController)target;

            if (GUILayout.Button("Play Clip", GUILayout.Width(90f)))
            {
                if (!Application.isPlaying)
                {
                    Debug.LogWarning("ejecuta el juego para usar esta funcion");
                    return;
                }

                if (newClip != null)
                    MusicController.PlayMusic(newClip);
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}