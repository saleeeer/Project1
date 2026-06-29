using UnityEngine;
using TMPro;

public class EventChoiceButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    EventChoiceData currentChoice;

    public void Setup(EventChoiceData choice)
    {
        currentChoice = choice;

        buttonText.text = choice.choiceText;
    }

    public void Click()
    {
        if (EventUIManager.Instance == null)
            return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);

        EventUIManager.Instance.ResolveChoice(
            currentChoice
        );
    }
}