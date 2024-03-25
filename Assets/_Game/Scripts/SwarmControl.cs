using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class SwarmControl : MonoBehaviour
{
    [Serializable]
    private struct Swarm
    {
        [SerializeField] internal Transform Transform;
        [SerializeField] internal GameObject GameObject;
        [SerializeField] internal int Health;

        public Swarm(Transform transform, GameObject gameObject, int health)
        {
            Transform = transform;
            GameObject = gameObject;
            Health = health;
        }

        public void SetGameObject(GameObject gameObject)
        {
            if(GameObject != null)
                Destroy(GameObject);

            if (gameObject == null)
                return;
            
            GameObject = gameObject;
            gameObject.transform.SetParent(Transform);
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }

        public void SetHealth(int health)
        {
            Health = health;
        }
    }

    [SerializeField] private GameObject emptySwarm;
    [SerializeField] private GameObject circleSwarm;
    
    [SerializeField] private List<Swarm> swarm;

    [SerializeField] private int maxSwarm = 10;
    
    private void Awake()
    {
        for (int i = 0; i < maxSwarm; ++i)
        {
            var go = Instantiate(emptySwarm, Vector3.zero, Quaternion.identity);
            var swarmChild = Instantiate(circleSwarm, go.transform);
            var swarmCollider = swarmChild.GetComponent<SwarmCollider>();
            swarmCollider.SetControl(this, i);
            swarm.Add(new Swarm(go.transform, swarmChild, 1));
            var distance = Random.Range(3, 15);
            var loc = Random.insideUnitCircle * distance;
            var wPos = Vector3.zero;
            wPos.x = loc.x;
            wPos.z = loc.y;
            go.transform.position = wPos;
            //go.SetActive(false);
        }
    }

    public void SwarmHitPlayer(int index)
    {
        // change this to recycle later...
        Destroy(swarm[index].GameObject);
        swarm[index].SetGameObject(null);
        swarm[index].SetHealth(0);
        
        swarm[index].Transform.gameObject.SetActive(false);
    }
}
