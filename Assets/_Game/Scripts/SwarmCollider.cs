using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmCollider : MonoBehaviour
{
    private SwarmControl swarmControl;
    private int index;

    public void SetControl(SwarmControl swarmControl, int index)
    {
        this.swarmControl = swarmControl;
        this.index = index;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // it will only ever collide with players
        swarmControl.SwarmHitPlayer(index);
    }
}
