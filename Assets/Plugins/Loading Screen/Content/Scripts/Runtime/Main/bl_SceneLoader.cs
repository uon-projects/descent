﻿using System;
using System.Collections;
using Lovatto.SceneLoader;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class bl_SceneLoader : MonoBehaviour
{
    /// <summary>
    /// </summary>
    public SceneSkipType GetSkipType
    {
        get
        {
            if (CurrentLoadLevel != null)
                if (CurrentLoadLevel.SkipType != SceneSkipType.None)
                    return CurrentLoadLevel.SkipType;
            if (SkipType != SceneSkipType.None)
                return SkipType;
            return SceneSkipType.AnyKey;
        }
    }

    /// <summary>
    /// </summary>
    public float DeltaTime => useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;

    /// <summary>
    /// </summary>
    private void Awake()
    {
        StartCoroutine(AsynLoadData());
    }

    /// <summary>
    /// </summary>
    private void Update()
    {
        if (!isOperationStarted)
            return;
        if (sceneAsyncOp == null)
            return;

        UpdateProgress();
        ScreenUI.OnUpdate();
        SkipWithKey();
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private IEnumerator AsynLoadData()
    {
        yield return bl_SceneLoaderManager.AsyncLoadData();
        yield return new WaitForEndOfFrame();
        Manager = bl_SceneLoaderManager.Instance;
        Init();
    }

    /// <summary>
    /// </summary>
    private void Init()
    {
        ScreenUI.Init(this);
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// </summary>
    private void UpdateProgress()
    {
        //asynchronous loading
        if (CurrentLoadLevel.LoadingType == LoadingType.Async)
        {
            //Get progress of load level
            var offset = GetSkipType == SceneSkipType.InstantComplete ? 0 : 0.1f;
            var completeProgress = sceneAsyncOp.progress + offset;
            smoothValue = Mathf.Lerp(smoothValue, completeProgress, DeltaTime * SceneSmoothLoad);
            if (sceneAsyncOp.isDone || smoothValue > 0.99f)
                //Called one time what is inside in this function.
                if (!FinishLoad)
                    OnFinish();
        }
        else //Fake load time
        {
            if (smoothValue >= 1)
                //Called one time what is inside in this function.
                if (!FinishLoad)
                    OnFinish();
        }

        //round the progress value if's necessary
        var barValue = RoundBarProgress > 0
            ? Mathf.Round(smoothValue / RoundBarProgress) * RoundBarProgress
            : smoothValue;
        ScreenUI.UpdateLoadProgress(barValue, smoothValue);
    }

    /// <summary>
    ///     Called when the scene load finish
    /// </summary>
    private void OnFinish()
    {
        FinishLoad = true;
        onLoaded?.Invoke();

        switch (GetSkipType)
        {
            case SceneSkipType.Button:
                break;
            case SceneSkipType.Instant:
            case SceneSkipType.InstantComplete:
                TransitionToScene();
                break;
            case SceneSkipType.AnyKey:
                canSkipWithKey = true;
                break;
        }

        if (FinishSound != null)
            AudioSource.PlayClipAtPoint(FinishSound, transform.position, PlayerPrefs.GetFloat(Constants.Options.Sound, 1.0f));
        ScreenUI.OnFinish();
    }

    /// <summary>
    ///     Start to loading a scene
    /// </summary>
    /// <param name="level">The scene name</param>
    public void LoadLevel(string level)
    {
        //get the scene info from the SceneLoaderManager
        CurrentLoadLevel = Manager.GetSceneInfo(level);
        if (CurrentLoadLevel == null)
            return;

        //Setup the UI with the scene info
        ScreenUI.SetupUIForScene(CurrentLoadLevel);
        //start load the scene asynchronously doesn't matter if use fake time
        StartCoroutine(DoAsyncOperation(CurrentLoadLevel.SceneName));
        //if fake time is used, don't load the scene until the fake time passed
        if (CurrentLoadLevel.LoadingType == LoadingType.Fake) StartCoroutine(StartFakeLoading());
    }

    /// <summary>
    /// </summary>
    public void TransitionToScene()
    {
        ScreenUI.TransitionToScene();
    }


    /// <summary>
    /// </summary>
    private void SkipWithKey()
    {
        if (!canSkipWithKey)
            return;

        if (Input.anyKeyDown) TransitionToScene();
    }

    /// <summary>
    /// </summary>
    private IEnumerator DoAsyncOperation(string level)
    {
        float t = 0;
        float d = 0;
        while (d < 1)
        {
            d += DeltaTime * FadeInSpeed;
            t = StartFadeInCurve.Evaluate(d);
            ScreenUI.RootAlpha.alpha = t;
            yield return null;
        }

        sceneAsyncOp = bl_SceneLoaderUtils.LoadLevelAsync(level);
        if (GetSkipType != SceneSkipType.InstantComplete || CurrentLoadLevel.LoadingType == LoadingType.Fake)
            sceneAsyncOp.allowSceneActivation = false;
        else
            sceneAsyncOp.allowSceneActivation = true;
        isOperationStarted = true;
        yield return sceneAsyncOp;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartFakeLoading()
    {
        smoothValue = 0;
        while (smoothValue < 1)
        {
            smoothValue += Time.deltaTime / CurrentLoadLevel.FakeLoadingTime;
            yield return new WaitForEndOfFrame();
        }
    }

    #region Public members

    [Header("Settings")] public SceneSkipType SkipType = SceneSkipType.Button;

    [Range(0.5f, 7)] public float SceneSmoothLoad = 3;
    [Range(0.5f, 7)] public float FadeInSpeed = 2;
    [Range(0.5f, 7)] public float FadeOutSpeed = 2;
    public bool useTimeScale;

    [Header("Background")] public bool useBackgrounds = true;

    public bool ShowDescription = true;
    [Range(1, 60)] public float TimePerBackground = 5;
    [Range(0.5f, 7)] public float FadeBackgroundSpeed = 2;
    [Range(0.5f, 5)] public float TimeBetweenBackground = 0.5f;
    public AnimationCurve StartFadeInCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    [Header("Tips")] public bool RandomTips;

    [Range(1, 60)] public float TimePerTip = 5;
    [Range(0.5f, 5)] public float FadeTipsSpeed = 2;

    [Header("Loading")] public bool FadeLoadingBarOnFinish;

    public float RoundBarProgress;
    [Range(50, 1000)] public float LoadingCircleSpeed = 300;
    [TextArea(2, 2)] public string LoadingTextFormat = "{0}";

    [Header("Audio")] [Range(0.1f, 1)] public float AudioVolume = 1f;

    [Range(0.5f, 5)] public float FadeAudioSpeed = 0.5f;
    [Range(0.1f, 1)] public float FinishSoundVolume = 0.5f;
    public AudioClip FinishSound;
    public AudioClip BackgroundAudio;

    [Serializable]
    public class OnLoaded : UnityEvent
    {
    }

    [SerializeField] public OnLoaded onLoaded;

    [Header("References")] public bl_LoadingScreenUI ScreenUI;

    public AsyncOperation sceneAsyncOp { get; set; }

    #endregion

    #region Private members

    private bl_SceneLoaderManager Manager;
    private bool isOperationStarted;
    private bool FinishLoad;
    private float smoothValue;
    private bool canSkipWithKey;
    private bl_SceneLoaderInfo CurrentLoadLevel;

    #endregion
}