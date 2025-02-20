﻿using UnityEngine;
using System.Collections;

public enum AutoDestructMode
{
    Destroy,
    Deactivate,
    Pool
}

public class AutoDestruct : MonoBehaviour {

    public AutoDestructMode mode = AutoDestructMode.Destroy;
    public float delay;
    public bool waitForParticles = true;
    public bool waitForAudio = true;
    private float tick;
    private GameObject prefab;

    private void OnEnable()
    {
        tick = 0;
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
        foreach (var audio in GetComponentsInChildren<AudioSource>())
        {
            audio.Stop();
            audio.Play();
        }
    }

    // Update is called once per frame
    void Update () 
    {
        if (tick > delay)
        {
            bool destruct = true;
            if (waitForParticles)
            {
                foreach (var ps in GetComponentsInChildren<ParticleSystem>())
                {
                    if (ps.IsAlive())
                    {
                        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        destruct = false;
                        break;
                    }
                }
            }
            if(destruct && waitForAudio)
            {
                foreach (var audio in GetComponentsInChildren<AudioSource>())
                {
                    audio.loop = false;
                    if(audio.isPlaying)
                    {
                        destruct = false;
                        break;
                    }
                }
            }
            if (destruct)
            {
                switch (mode)
                {
                    case AutoDestructMode.Destroy:
                        Destroy(gameObject);
                        break;
                    case AutoDestructMode.Deactivate:
                        gameObject.SetActive(false);
                        break;
                    case AutoDestructMode.Pool:
                        GameObjectPool.Instance.Pool(new GameObjectPool.PooledGameObject(prefab, this.gameObject));
                        break;
                }
            }
        }
        else
        {
            tick += Time.deltaTime;
        }
	}

    public void PoolWhenFinished(GameObject prefab)
    {
        if (prefab)
        {
            mode = AutoDestructMode.Pool;
            this.prefab = prefab;
        }
    }
}
