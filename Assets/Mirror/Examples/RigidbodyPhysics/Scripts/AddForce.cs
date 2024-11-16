using UnityEngine;

namespace Mirror.Examples.RigidbodyPhysics
{
    public class AddForce : NetworkBehaviour
    {
        public Rigidbody rigidbody3d;
        public float force = 500f;

<<<<<<< Updated upstream
        void Start()
        {
            rigidbody3d.isKinematic = !isServer;
=======
        void OnValidate()
        {
            rigidbody3d = GetComponent<Rigidbody>();
            rigidbody3d.isKinematic = true;
>>>>>>> Stashed changes
        }

        public override void OnStartServer()
        {
            rigidbody3d.isKinematic = false;
        }

        [ServerCallback]
        void Update()
        {
<<<<<<< Updated upstream
            if (isServer && Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody3d.AddForce(Vector3.up * force);
            }
=======
            if (Input.GetKeyDown(KeyCode.Space))
                rigidbody3d.AddForce(Vector3.up * force);
>>>>>>> Stashed changes
        }
    }
}
