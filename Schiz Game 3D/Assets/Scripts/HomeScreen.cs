using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeScreen : MonoBehaviour
{
    public InputField nameInputField;
    public Toggle consentToggle;

    public void OnPlayButtonClicked()
    {
        // Check if the input name is filled and consent checkbox is checked
        if (IsInputNameFilled() && IsConsentChecked())
        {
            // Load the instructions scene
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("Please fill the input name and check the consent checkbox.");
        }
    }

    private bool IsInputNameFilled()
    {
        return !string.IsNullOrEmpty(nameInputField.text);
    }

    private bool IsConsentChecked()
    {
        return consentToggle.isOn;
    }
}
