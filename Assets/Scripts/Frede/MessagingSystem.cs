using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UserRole { Player, Admin }
public enum MsgType { Private, Public, Group }

public class MessagingSystem : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Dropdown msgTypeDropdown;
    public TMP_Text msgText;

    private RabbitMqManager rabbitMqManager;

    private UserRole userRole;

    public void SendButtonClicked()
    {
        string msgContent = inputField.text;
        MsgType msgType = (MsgType)msgTypeDropdown.value;

        if (CanSendMsg(msgType))
        {
            string msgLabel = GetMsgTypeLabel(msgType);
            string fullMsg = msgLabel + ": " + msgContent;

            // Send full message to RabbitMQ
            rabbitMqManager.SendToRabbitMQ(fullMsg);

            // Append full message to the TMPro text field
            msgText.text += fullMsg + "\n";

            // Clear input field
            inputField.text = "";
        }
        else
        {
            Debug.LogWarning("You don't have permission to send this type of message.");
        }
    }

    private bool CanSendMsg(MsgType msgType)
    {
        // Implement logic to check if the user can send a message of the specified type.
        // For example, allow players to send private messages and admins to send public/group messages.
        if (userRole == UserRole.Player && msgType == MsgType.Private)
        {
            return true;
        }
        else if (userRole == UserRole.Admin && (msgType == MsgType.Public || msgType == MsgType.Group))
        {
            return true;
        }
        return false;
    }

    private string GetMsgTypeLabel(MsgType msgType)
    {
        // Define labels for message types
        switch (msgType)
        {
            case MsgType.Private:
                return "Private";
            case MsgType.Public:
                return "Public";
            case MsgType.Group:
                return "Group";
            default:
                return "Unknown";
        }
    }
}
