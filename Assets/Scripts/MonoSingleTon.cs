using UnityEngine;

// Singleton ������ Generic����
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance = null;

    public static T Instacne
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}