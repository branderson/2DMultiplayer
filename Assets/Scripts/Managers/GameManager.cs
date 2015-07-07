using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Managers
{
    // Don't ever delete this
    public class GameManager : MonoBehaviour
    {

        void Awake()
        {
            Object.DontDestroyOnLoad(this);
        }

        void Start()
        {

        }

        void Update()
        {

        }
    }
}