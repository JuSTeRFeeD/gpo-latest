using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// TODO: [!!!] занести все имена эффектов в Enum

public class ParticlesController : MonoBehaviour
{
    [SerializeField] private int prepearedCount = 5;
    [Serializable] private class Particle
    {
        public string name;
        public GameObject prefab;

        public ParticleSystem particle { get; private set; }
        public void SetParticle(ParticleSystem particleSystem)
        {
            this.particle = particleSystem;
        }

        public void PlayEffect(Vector2 position)
        {
            this.prefab.SetActive(true);
            this.prefab.transform.position = position;
            this.particle.Play();
        }
    }
    [SerializeField] private List<Particle> preparedParticles = new List<Particle>();
    private readonly List<Particle> _particlesList = new List<Particle>();

    private void Start()
    {
        foreach (var item in preparedParticles)
        {
            for (var j = 0; j < prepearedCount; j++)
            {
                _particlesList.Add(CreateParticle(item.name, item.prefab));
            }
        }
    }

    private Particle CreateParticle(string particleName, GameObject particlePrefab)
    {
        var obj = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity, transform);
        var newParticle = new Particle()
        {
            name = particleName,
            prefab = obj,
        };
        newParticle.SetParticle(obj.GetComponent<ParticleSystem>());
        obj.SetActive(false);
        return newParticle;
    }

    public void PlayParticle(string particleName, Vector2 position)
    {
        foreach (var item in _particlesList.Where(item => !item.prefab.activeSelf && item.name == particleName))
        {
            item.PlayEffect(position);
            return;
        }

        foreach (var item in preparedParticles)
        {
            if (item.name == particleName)
            {
                var particle = CreateParticle(particleName, item.prefab);
                _particlesList.Add(particle);
                particle.PlayEffect(position);
                return;
            }
        }
        Debug.LogError("Cannot found particle with name: " + particleName);
    }
}
