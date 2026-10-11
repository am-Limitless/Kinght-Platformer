using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiCelebration : MonoBehaviour
{
    [Header("Effects (from the Procedural Confetti pack)")]
    [SerializeField] private ParticleSystem burstPrefab;
    [SerializeField] private ParticleSystem rainPrefab;

    [Header("Look")]
    [Tooltip("Shrinks the pack's effects to fit this game's camera size.")]
    [SerializeField, Min(0.05f)] private float effectScale = 0.4f;
    [Tooltip("Must be higher than the level sprites so confetti is drawn in front of them.")]
    [SerializeField] private int sortingOrder = 500;

    [Header("Bursts (fired from the bottom corners)")]
    [SerializeField, Range(0f, 40f)] private float burstTiltDegrees = 15f;
    [Tooltip("Seconds after the first burst when the corners fire again. 0 = no second burst.")]
    [SerializeField, Min(0f)] private float secondBurstDelaySeconds = 0.8f;

    [Header("Rain (falls from the top edge)")]
    [SerializeField, Range(1, 6)] private int rainEmitterCount = 3;
    [SerializeField, Min(0f)] private float rainDelaySeconds = 0.3f;
    [SerializeField, Min(0.5f)] private float rainDurationSeconds = 4f;

    private const float CleanupSecondsAfterLastEmission = 6f;

    private readonly List<ParticleSystem> _bursts = new List<ParticleSystem>();
    private readonly List<ParticleSystem> _rainEmitters = new List<ParticleSystem>();

    public void Play(Camera targetCamera)
    {
        if (targetCamera == null || burstPrefab == null || rainPrefab == null)
        {
            Destroy(gameObject);
            return;
        }

        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;
        Vector3 center = new Vector3(targetCamera.transform.position.x, targetCamera.transform.position.y, 0f);

        SpawnBursts(center, halfWidth, halfHeight);
        SpawnRain(center, halfWidth, halfHeight);
        StartCoroutine(RunTimeline());
    }

    private void SpawnBursts(Vector3 center, float halfWidth, float halfHeight)
    {
        Vector3 left = center + new Vector3(-halfWidth * 0.85f, -halfHeight * 0.95f, 0f);
        Vector3 right = center + new Vector3(halfWidth * 0.85f, -halfHeight * 0.95f, 0f);
        _bursts.Add(CreateEmitter(burstPrefab, left, -burstTiltDegrees));
        _bursts.Add(CreateEmitter(burstPrefab, right, burstTiltDegrees));
    }

    private void SpawnRain(Vector3 center, float halfWidth, float halfHeight)
    {
        float span = halfWidth * 1.4f;
        for (int i = 0; i < rainEmitterCount; i++)
        {
            float t = rainEmitterCount == 1 ? 0.5f : i / (float)(rainEmitterCount - 1);
            Vector3 position = center + new Vector3(-span / 2f + span * t, halfHeight * 1.05f, 0f);
            _rainEmitters.Add(CreateEmitter(rainPrefab, position, 0f));
        }
    }

    private ParticleSystem CreateEmitter(ParticleSystem prefab, Vector3 position, float tiltDegrees)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, tiltDegrees) * prefab.transform.rotation;
        ParticleSystem emitter = Instantiate(prefab, position, rotation, transform);
        emitter.transform.localScale = Vector3.one * effectScale;

        foreach (ParticleSystemRenderer particleRenderer in emitter.GetComponentsInChildren<ParticleSystemRenderer>(true))
        {
            particleRenderer.sortingOrder = sortingOrder;
        }

        emitter.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        return emitter;
    }

    private IEnumerator RunTimeline()
    {
        PlayAll(_bursts);

        float elapsed = 0f;
        bool secondBurstFired = secondBurstDelaySeconds <= 0f;
        bool rainStarted = false;

        while (elapsed < rainDelaySeconds + rainDurationSeconds)
        {
            if (!secondBurstFired && elapsed >= secondBurstDelaySeconds)
            {
                PlayAll(_bursts);
                secondBurstFired = true;
            }

            if (!rainStarted && elapsed >= rainDelaySeconds)
            {
                PlayAll(_rainEmitters);
                rainStarted = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        StopEmitting(_rainEmitters);
        StopEmitting(_bursts);
        Destroy(gameObject, CleanupSecondsAfterLastEmission);
    }

    private static void PlayAll(List<ParticleSystem> emitters)
    {
        foreach (ParticleSystem emitter in emitters)
        {
            emitter.Play(true);
        }
    }

    private static void StopEmitting(List<ParticleSystem> emitters)
    {
        foreach (ParticleSystem emitter in emitters)
        {
            emitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
