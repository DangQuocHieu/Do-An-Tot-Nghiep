using UnityEngine;

public class PlayerController : MonoBehaviour, IMessageHandle
{
    [SerializeField] private StarterAssets.FirstPersonController _firstPersonController;

    private void OnEnable()
    {
        MessageManager.AddSubscriber(GameMessageType.EnterCutMode, this);
        MessageManager.AddSubscriber(GameMessageType.ExitCutMode, this);
    }

    private void OnDisable()
    {
        MessageManager.RemoveSubscriber(GameMessageType.EnterCutMode, this);
        MessageManager.RemoveSubscriber(GameMessageType.ExitCutMode, this);
    }

    private void EnableMovement()
    {
        _firstPersonController.enabled = true;
    }

    private void DisableMovement()
    {
        _firstPersonController.enabled = false;
    }

    public void Handle(Message message)
    {
        switch(message.type)
                    {
            case GameMessageType.EnterCutMode:
                DisableMovement();
                break;
            case GameMessageType.ExitCutMode:
                EnableMovement();
                break;
        }
    }
}
