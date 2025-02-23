using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource), typeof(SpriteRenderer))]
public class SupplyArea : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float refillRate = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip refillLoopSound;
    [SerializeField] private AudioClip refillEndSound;
    [SerializeField] private AudioClip refillBeginSound;

    private static readonly int IsActive = Shader.PropertyToID("_Active");

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Material material;

    private readonly Dictionary<ProjectileTurretController, Coroutine> activeRefills = new();
    private readonly Dictionary<ProjectileTurretController, Action<IDamagable>> onDestroyedHandlers = new();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = refillLoopSound;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other) return;

        var turretController = other.GetComponent<ProjectileTurretController>();
        if (!turretController) return;

        var tankController = other.GetComponent<TankController>();
        if (tankController)
        {
            Action<IDamagable> handler = _ => StopRefilling(turretController);
            onDestroyedHandlers[turretController] = handler;
            tankController.OnDestroyed += handler;
        }

        if (activeRefills.Count == 0)
        {
            PlayOneShot(refillBeginSound);
            audioSource.Play();
            SetActive(true);
        }

        StartRefilling(turretController);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other) return;

        var turretController = other.GetComponent<ProjectileTurretController>();
        if (!turretController) return;

        var tankController = turretController.tankHullController;
        if (tankController && onDestroyedHandlers.TryGetValue(turretController, out var handler))
        {
            tankController.OnDestroyed -= handler;
            onDestroyedHandlers.Remove(turretController);
        }

        StopRefilling(turretController);

        if (activeRefills.Count == 0)
        {
            PlayOneShot(refillEndSound);
            SetActive(false);
        }
    }

    private void StartRefilling(ProjectileTurretController turretController)
    {
        if (activeRefills.ContainsKey(turretController)) return;
        if (!audioSource.isPlaying) audioSource.Play();

        var coroutine = StartCoroutine(RefillProjectiles(turretController));
        activeRefills.Add(turretController, coroutine);
    }

    private void StopRefilling(ProjectileTurretController turretController)
    {
        if (!activeRefills.TryGetValue(turretController, out var coroutine)) return;

        if (coroutine != null) StopCoroutine(coroutine);
        activeRefills.Remove(turretController);

        if (activeRefills.Count == 0) audioSource.Stop();
    }

    private void SetActive(bool active)
    {
        material.SetFloat(IsActive, active ? 1f : 0f);
    }

    private IEnumerator RefillProjectiles(ProjectileTurretController turretController)
    {
        if (!turretController) yield break;
        var waitTime = new WaitForSeconds(refillRate);

        while (turretController.enabled)
        {
            yield return waitTime;
            turretController.RefillProjectile();
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (!clip || !audioSource) return;
        audioSource.PlayOneShot(clip);
    }
}