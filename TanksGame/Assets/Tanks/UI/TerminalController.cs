using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TerminalController : UIBehaviour 
{
    [SerializeField]
    private TMP_InputField consoleInputField;
    
    private void Awake()
    {
        Cursor.visible = false;
    }
    
    protected override void OnEnable() {
        base.OnEnable();
        consoleInputField.Select();
    }
    
    private void Update() {
        if (!consoleInputField.isFocused) {
            consoleInputField.Select();
        }
    }
}

