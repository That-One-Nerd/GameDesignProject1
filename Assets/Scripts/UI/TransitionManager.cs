using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : Singleton<TransitionManager>
{
    public bool Transitioning { get; private set; }

    [Header("Properties")]
    public float FadeTimeTotal;

    private RawImage image;

    protected override void Awake()
    {
        base.Awake();
        image = GetComponentInChildren<RawImage>();
    }

    public void Transition(Action onBlackened)
    {
        if (Transitioning) return;
        DoTransition(onBlackened);
    }

    private async void DoTransition(Action callback)
    {
        Transitioning = true;
        float fadeTime = Mathf.Max(FadeTimeTotal * 0.5f, Time.deltaTime * 2);

        // Fade in over half a second.
        for (float t = 0; t <= fadeTime; t += Time.deltaTime)
        {
            image.color = new Color(0, 0, 0, t / fadeTime);
            await Task.Yield();
        }

        callback();

        // Fade out over half a second.
        for (float t = 0; t <= fadeTime; t += Time.deltaTime)
        {
            image.color = new Color(0, 0, 0, 1 - t / fadeTime);
            await Task.Yield();
        }

        Transitioning = false;
    }
}
