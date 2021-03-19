using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlappyBird
{
    public class BirdContoller : MonoBehaviour
    {
        public float jumpVelocity = 3;

        public float upwardAngularSpeed = 360;
        public float upwardStopAngle;
        protected bool upward;
        public float downwardAngularSpeed = -180;

        protected new Rigidbody2D rigidbody2D;
        protected Vector3 initialPosition;


        protected virtual void Awake() {
            rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.simulated = false;

            initialPosition = transform.position;
        }

        protected virtual void OnEnable() {
            transform.position = initialPosition;
            rigidbody2D.simulated = true;
            rigidbody2D.velocity = new Vector2(0, jumpVelocity);
            rigidbody2D.angularVelocity = downwardAngularSpeed;
            transform.rotation = Quaternion.Euler(0, 0, upwardStopAngle);
        }

        void Update()
        {
            UpdateRotation();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (rigidbody2D.simulated) Jump();
                else ResetGame();
            }
        }

        protected void UpdateRotation()
        {
            if (upward)
            {
                if (transform.rotation.eulerAngles.z >= upwardStopAngle)
                {
                    upward = false;
                    rigidbody2D.angularVelocity = downwardAngularSpeed;
                }
            }
        }

        protected void Jump()
        {
            rigidbody2D.velocity = new Vector2(0, jumpVelocity);
            rigidbody2D.angularVelocity = upwardAngularSpeed;

            upward = true;
        }

        public void ResetGame()
        {
            transform.position = initialPosition;
            rigidbody2D.simulated = true;
            rigidbody2D.velocity = new Vector2(0, jumpVelocity);

            transform.rotation = Quaternion.Euler(0, 0, 0);
            rigidbody2D.angularVelocity = upwardAngularSpeed;
            upward = true;
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            rigidbody2D.simulated = false;
            FindObjectOfType<GameControl>().GameOver();
            enabled = false;
        }
    }
}