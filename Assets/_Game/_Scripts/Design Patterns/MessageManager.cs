using System.Collections.Generic;
using UnityEngine;

public enum GameMessageType
{
    EnterCutMode,
    ExitCutMode
}

public class Message
{
    public GameMessageType type;
    public object[] data;
    public Message(GameMessageType type)
    {
        this.type = type;
    }
    public Message(GameMessageType type, object[] data)
    {
        this.type = type;
        this.data = data;
    }
}

public interface IMessageHandle
{
    void Handle(Message message);
}

public static class MessageManager
{
    private static List<GameMessageType> _keys = new List<GameMessageType>();
    private static List<List<IMessageHandle>> _values = new List<List<IMessageHandle>>();

    private static Dictionary<GameMessageType, List<IMessageHandle>> subscribers = new Dictionary<GameMessageType, List<IMessageHandle>>();

    public static void AddSubscriber(GameMessageType type, IMessageHandle handle)
    {
        if (!subscribers.ContainsKey(type))
            subscribers[type] = new List<IMessageHandle>();
        if (!subscribers[type].Contains(handle))
            subscribers[type].Add(handle);
    }

    public static void RemoveSubscriber(GameMessageType type, IMessageHandle handle)
    {
        if (subscribers.ContainsKey(type))
            if (subscribers[type].Contains(handle))
                subscribers[type].Remove(handle);
    }

    public static void SendMessage(Message message)
    {
        if (subscribers.ContainsKey(message.type))
            for (int i = subscribers[message.type].Count - 1; i > -1; i--)
                subscribers[message.type][i].Handle(message);
    }

    public static void SendMessageWithDelay(Message message, float delayDuration)
    {
        float timeElapsed = 0;
        while (timeElapsed < delayDuration)
        {
            timeElapsed += Time.deltaTime;
        }
        SendMessage(message);
    }

    public static void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var element in subscribers)
        {
            _keys.Add(element.Key);
            _values.Add(element.Value);
        }
    }

    public static void OnAfterDeserialize()
    {
        subscribers = new Dictionary<GameMessageType, List<IMessageHandle>>();
        for (int i = 0; i < _keys.Count; i++)
        {
            subscribers.Add(_keys[i], _values[i]);
        }
    }
}