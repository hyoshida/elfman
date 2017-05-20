using System;

[Serializable]
public class NotificationObject<T> {
    public delegate void NotificationAction(T t);

    public UnityEngine.Events.UnityAction<T, T> watcher;

    T data;

    public NotificationObject(T t) {
        Value = t;
    }

    public NotificationObject() {
    }

    ~NotificationObject() {
        Dispose();
    }

    public T Value {
        get {
            return data;
        }
        set {
            Notifier(value, data);
            data = value;
        }
    }

    public void Dispose() {
        watcher = null;
    }

    void Notifier(T newValue, T oldValue) {
        if (watcher != null) {
            watcher(newValue, oldValue);
        }
    }
}