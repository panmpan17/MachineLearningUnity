using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : AbstractCharacterInput
{
    public override bool Left => m_leftPressed;
    public override bool Right => m_rightPressed;
    public override bool Jump => m_spacePressed;

    private bool m_leftPressed;
    private bool m_rightPressed;
    private bool m_spacePressed;

    private void Update() {
        m_leftPressed = Input.GetKey(KeyCode.A);
        m_rightPressed = Input.GetKey(KeyCode.D);
        m_spacePressed = Input.GetKey(KeyCode.Space);
    }
}
