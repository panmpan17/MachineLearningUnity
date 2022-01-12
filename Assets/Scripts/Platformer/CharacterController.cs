using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace Platformer
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField]
        private float m_WalkAccelerate, m_WalkDrag, m_WalkMaxSpeed;
        [SerializeField]
        private float m_JumpForce, m_JumpSec, m_WaitJumpSec;
        private float m_JumpTimer;

        private bool m_Jumping, m_LeaveGround;

        private SmartBoxCollider m_BoxCollider;
        private Rigidbody2D m_Rigidbody2D;
        private AbstractCharacterInput input;


        private void Awake()
        {
            m_BoxCollider = GetComponent<SmartBoxCollider>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            input = GetComponent<AbstractCharacterInput>();
        }

        private void Update()
        {
            Vector2 velocity = m_Rigidbody2D.velocity;

            #region Walking
            bool walking = false;
            if (input.Right)
            {
                walking = true;
                velocity.x = Mathf.MoveTowards(velocity.x, m_WalkMaxSpeed, m_WalkAccelerate * TimeControl.deltaTime);
            }
            if (input.Left)
            {
                walking = true;
                velocity.x = Mathf.MoveTowards(velocity.x, -m_WalkMaxSpeed, m_WalkAccelerate * TimeControl.deltaTime);
            }

            if (!walking)
                velocity.x = Mathf.MoveTowards(velocity.x, 0, m_WalkDrag * TimeControl.deltaTime);
            #endregion

            #region Jumping
            if (!m_Jumping)
            {
                if (m_BoxCollider.DownTouched && input.Jump)
                {
                    m_Jumping = true;
                    velocity.y = m_JumpForce;
                }
            }
            else
            {
                if (m_LeaveGround)
                {
                    if (m_BoxCollider.DownTouched)
                    {
                        m_LeaveGround = false;
                        m_Jumping = false;
                        m_JumpTimer = 0;
                        return;
                    }
                }
                else if (!m_BoxCollider.DownTouched)
                    m_LeaveGround = true;

                if (m_JumpTimer < m_JumpSec)
                {
                    bool keepJumping = Input.GetKey(KeyCode.Space);
                    if (keepJumping)
                    {
                        m_JumpTimer += TimeControl.deltaTime;
                        velocity.y = m_JumpForce;
                    }
                    else
                    {
                        m_JumpTimer = m_JumpSec + 1;
                    }
                }
            }
            #endregion

            m_Rigidbody2D.velocity = velocity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("End"))
            {
                AbstractMachineLearningGameController gameController = FindObjectOfType<AbstractMachineLearningGameController>();
                Debug.Log(gameController);
                if (gameController != null)
                {
                    gameController.CharacterReachEnd(this);
                }
            }
        }
    }
}