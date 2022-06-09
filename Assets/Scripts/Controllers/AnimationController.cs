using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator Animator;

    [SerializeField] string DefaultAnimation;

    [SerializeField] string PlayingAnimation;

    void Start()
    {
        if(!Animator)
            Animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Changes the animator's state to match the given animation parameters.
    /// </summary>
    /// <param name="animationParams"></param>
    public virtual void PlayAnimation(AnimationParameters animationParams)
    {
        DefaultAnimation = animationParams.DefaultAnimation;
        PlayingAnimation = animationParams.Animation;

        Animator.Play(PlayingAnimation);

        if (!animationParams.Loop)
        {
            Invoke(nameof(StopAnimation), animationParams.Duration);
        }

        // Debug
        if (SimulationManager.Instance.Mode != SimMode.Debug)
            Debug.Log($"<color=blue>Playing {animationParams.Animation} animation</color>.");
    }

    /// <summary>
    /// Stops current animation and returns animator to default state. Default state is given in the PlayAnimation() call.
    /// </summary>
    public virtual void StopAnimation() 
    {
        CancelInvoke();

        if (!string.IsNullOrEmpty(DefaultAnimation))
        {
            PlayingAnimation = DefaultAnimation;
            Animator.Play(DefaultAnimation);
        }

        // Debug
        if (SimulationManager.Instance.Mode != SimMode.Debug)
            Debug.Log($"<color=blue>Playing default animation</color>.");
    }
}
