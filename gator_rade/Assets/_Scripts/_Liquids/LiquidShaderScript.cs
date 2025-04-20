using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidShaderScript : MonoBehaviour
{
        public Material material; 
        private Rigidbody rb;      
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            // Get the object's velocity from the Rigidbody
            Vector3 velocity = rb.velocity;

            // Convert the velocity to magnitude (speed)
            float velocityMagnitude = (velocity.magnitude * .1f);

            // Set the velocity magnitude and direction to the shader material property
            material.SetFloat("_VelocityMagnitude", velocityMagnitude);
            material.SetVector("_DisplacementDirection", velocity.normalized);
        }
}
