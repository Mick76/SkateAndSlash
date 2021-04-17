using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMass : MonoBehaviour
{
    public Vector3 centerOfMass;
    public bool awake;
    protected Rigidbody body;

    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        body.centerOfMass = centerOfMass;
        body.WakeUp();
        awake = !body.IsSleeping();

        //transform.position += transform.forward * Time.deltaTime * 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.rotation * centerOfMass, 0.2f);
    }
}
