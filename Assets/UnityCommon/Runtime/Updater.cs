using System;
using UnityEngine;

namespace UnityCommon
{
    public class Updater : MonoBehaviour
    {
        public float UpdateDelay { get => updateDelay; set => updateDelay = value; }

        [SerializeField] private float updateDelay;

        private Action[] actions = Array.Empty<Action>();
        private float lastUpdateTime;

        private void Update ()
        {
            var timeSinceLastUpdate = Time.time - lastUpdateTime;
            if (timeSinceLastUpdate < UpdateDelay) return;

            var length = actions.Length;
            for (int i = 0; i < length; i++)
                actions[i].Invoke();

            lastUpdateTime = Time.time;
        }

        private void OnDestroy ()
        {
            actions = Array.Empty<Action>();
        }

        public void AddAction (Action action)
        {
            ArrayUtils.Add(ref actions, action);
        }

        public void RemoveAction (Action action)
        {
            ArrayUtils.Remove(ref actions, action);
        }
    }
}
