using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This handles visual indicators of player dashing, wing animations, and misc. particle effects
/// </summary>
public class Wings : MonoBehaviour
{
    Animation anims;

    [SerializeField] string suffix;

    [SerializeField] Material mat;

    [SerializeField] ParticleSystem jumpWoosh;
    [SerializeField] ParticleSystem dashParticles;

    private Coroutine emissiveLerpCo;
    [SerializeField] float emissiveLerpLength;
    float startingEmissivePower;

    //_Emissive_Power

    private void Start()
    {
        anims = GetComponent<Animation>();
        startingEmissivePower = 11;
        mat.SetFloat("_Emissive_Power", 11);
    }

    /// <summary>
    /// plays the jump animation
    /// </summary>
    public void Jump()
    {
        anims.Play(suffix + "Jump");

        // play air whoosh?
        // jumpWoosh.Play();
    }

    /// <summary>
    /// plays the dash animation
    /// </summary>
    public void Dash()
    {
        anims.Play(suffix + "Dash");
        EmissiveLerp(false);
        // sparkle
        // dashParticles.Play();
    }


    /// <summary>
    /// initiates the coroutine of the emissiveness of the wing's material
    /// </summary>
    /// <param name="active">if true turn emissive on. else turn emissive off</param>
    public void EmissiveLerp(bool active)
    {
        emissiveLerpCo = StartCoroutine(EmissiveLerpCoroutine(active));
    }

    /// <summary>
    /// toggles the emissiveness of the wing's material
    /// </summary>
    /// <param name="active">if true turn emissive on. else turn emissive off</param>
    /// <returns></returns>
    private IEnumerator EmissiveLerpCoroutine(bool active)
    {
        float currentEmissivePower = mat.GetFloat("_Emissive_Power");
        float totalTime = 0;

        while (totalTime <= emissiveLerpLength)
        {
            totalTime += Time.deltaTime;

            if (active)
            {
                // buffer for constant calls from player JumpHandler()
                if (currentEmissivePower != startingEmissivePower) currentEmissivePower = Mathf.Lerp(0, startingEmissivePower, totalTime / emissiveLerpLength);
            }
            else currentEmissivePower = Mathf.Lerp(startingEmissivePower, 0, totalTime / emissiveLerpLength);

            mat.SetFloat("_Emissive_Power", currentEmissivePower);
            yield return null;
        }

        emissiveLerpCo = null;
        yield return null;
    }
}
