using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = true;
        _agent.updatePosition = true;
    }

    // Update is called once per frame
    void Update()
    {
        _agent.Move
    }
}
