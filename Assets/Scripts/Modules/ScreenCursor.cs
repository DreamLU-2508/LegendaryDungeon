using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class ScreenCursor : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}
