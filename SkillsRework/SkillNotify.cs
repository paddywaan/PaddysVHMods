using System;
using UnityEngine;
namespace SkillsRework
{
    internal class SkillNotify : MonoBehaviour
    {
        private const float NotificationTime = 3f;
        private float time;
        private GameObject parent;
        private RectTransform rect;
        public void Start()
        {
            Main.log.LogDebug($"Start called.");
            rect = this.GetComponent<RectTransform>();
            if (rect == null) Main.log.LogDebug($"{rect.root.localPosition}");
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y - 80, rect.localPosition.z);
            time = Time.time;
        }
        public void Update()
        {
            if (Time.time > Time.time + NotificationTime*1000)
            {
                rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + 1, rect.localPosition.z);
            }
        }
    }
}