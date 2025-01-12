using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour
{
    public float SeeDistance;
    public float AttackDistance;
    public float Speed;
    private Transform Target;
    // Use this for initialization
    void Start()
    {
        Target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, Target.transform.position) < SeeDistance)
        {
            if (Vector3.Distance(transform.position, Target.transform.position) > SeeDistance)
            {
                transform.LookAt(Target.transform);
                transform.Translate(new Vector3(0, 0, Speed = Time.deltaTime));
            }
        }
    }
}

