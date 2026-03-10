using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioManager : Singleton<AudioManager>
{
    [Header("Pre-Runtime Config")]
    [SerializeField] private EventReference musicOnStart;
    private Bank master;
    private Dictionary<string, EventReference> eventReferences;
    private EventInstance currMusicInstance;
    [SerializeField, Range(0, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0, 1f)] private float musicVolume = 1f;
    [SerializeField, Range(0, 1f)] private float sfxVolume = 1f;
    private void Awake()
    {
        InitializeSingleton();
        RuntimeManager.LoadBank("Master");
        RuntimeManager.StudioSystem.getBank("bank:/Master", out Bank masterBank);
        master = masterBank;
        eventReferences = new Dictionary<string, EventReference>();
    }
    private void Start()
    { 
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        OnStartMusic();
    }
    public void PlayOneShotSound(string eventPath)
    {
        if (!eventReferences.ContainsKey(eventPath))
        {
            eventReferences[eventPath] = RuntimeManager.PathToEventReference(eventPath);
        }
        RuntimeManager.PlayOneShot(eventReferences[eventPath]);
    }
    public void PlayOneShotSound(EventReference sound, Vector2 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    public void SetMasterVolume(float volume)
    { 
        RuntimeManager.GetBus("bus:/").setVolume(volume);
    }
    public void SetSFXVolume(float volume)
    {
        RuntimeManager.GetBus("bus:/SFX").setVolume(volume);
    }
    #region MUSIC
    // Public Accessibles
    public void SetSceneMusic(string eventPath)
    {
        if (!eventReferences.ContainsKey(eventPath))
        {
            eventReferences[eventPath] = RuntimeManager.PathToEventReference(eventPath);
        }
        SetSceneMusic(eventReferences[eventPath]); // Compilers usually unroll this right?
    }
    public void SetSceneMusic(EventReference music)
    {
        currMusicInstance = RuntimeManager.CreateInstance(music);
        currMusicInstance.start();
    }
    public void PauseMusic()
    {
        if (currMusicInstance.isValid())
        {
            currMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    public void StopMusic()
    {
        PauseMusic();
        currMusicInstance.release();
    }
    public void ResumeMusic()
    {
        if (currMusicInstance.isValid())
        {
            currMusicInstance.start();
        }
    }
    public void SetMusicVolume(float volume)
    {
        RuntimeManager.GetBus("bus:/Music").setVolume(volume);
    }
    // Privates
    private void OnStartMusic()
    {
        if (musicOnStart.IsNull) return;
        SetSceneMusic(musicOnStart);
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioManager))]
public class AudioManager_Editor : Editor
{
    SerializedProperty masterVolume;
    SerializedProperty musicVolume;
    SerializedProperty sfxVolume;
    private void OnEnable()
    {
        masterVolume = serializedObject.FindProperty("masterVolume");
        musicVolume = serializedObject.FindProperty("musicVolume");
        sfxVolume = serializedObject.FindProperty("sfxVolume");
    }
    public override void OnInspectorGUI()
    {
        AudioManager manager = (AudioManager)target;

        // Since we end up using serialized properties, we need to update the serialized object to get the latest values
        // Also because I didn't want to make some properities publics
        serializedObject.Update(); 

        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
            {
                // Just to not bug it out if in edit mode
                manager.SetMasterVolume(masterVolume.floatValue);
                manager.SetMusicVolume(musicVolume.floatValue);
                manager.SetSFXVolume(sfxVolume.floatValue);
            }  
        }
    }
}
#endif
