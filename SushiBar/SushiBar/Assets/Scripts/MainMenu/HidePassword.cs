using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidePassword : MonoBehaviour
{
    public Button toggleButton;
    public InputField passwordInputField;
    public Sprite showPasswordSprite; 
    public Sprite hidePasswordSprite; 
    private bool isPasswordVisible = false; 
    private Image buttonImage; 

    private void Start()
    {
        buttonImage = toggleButton.GetComponent<Image>(); 
        toggleButton.onClick.AddListener(TogglePasswordVisibility); 
    }

    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible; 

        if (isPasswordVisible)
        {
            passwordInputField.contentType = InputField.ContentType.Standard; 
            buttonImage.sprite = showPasswordSprite; 
        }
        else
        {
            passwordInputField.contentType = InputField.ContentType.Password; 
            buttonImage.sprite = hidePasswordSprite;
        }

        passwordInputField.ForceLabelUpdate(); 
        passwordInputField.ActivateInputField(); 
    }
}
