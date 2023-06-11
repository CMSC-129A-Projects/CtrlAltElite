using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem runParticle;
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private ParticleSystem dashParticle;
    [SerializeField] private ParticleSystem flipParticle;
    private ParticleSystem.VelocityOverLifetimeModule flipVelocityModule;

    private void Start()
    {
        flipVelocityModule = flipParticle.velocityOverLifetime;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayJumpParticle();
        }
    }
    public void PlayRunParticle()
    {
        // runParticle.Play();
    }

    public void PlayJumpParticle()
    {
        jumpParticle.Play();
    }

    public void PlayDashPartcile()
    {
        dashParticle.Play();
    }

    public void PlayFlipParticle(bool isFacingRight)
    {
        // get the direction player is facing
        float direction = isFacingRight ? 1f : -1f;
        // flip direction for particle effect
        direction *= -1;
        // flip the velocity over time x according to the direction against the player
        flipVelocityModule.x = new ParticleSystem.MinMaxCurve(direction);

        flipParticle.Stop();
        flipParticle.Play();

    }
}
