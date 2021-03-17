using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SkillsRework
{
    internal class NotificationHandler : MonoBehaviour
    {
        private static NotificationHandler instance;
        internal static GameObject NotificationLayer;
        public List<GameObject> notifications = new List<GameObject>();
        private Queue<GameObject> incomingNotificationQueue = new Queue<GameObject>();
        
        private GameObject currentNotification;

        static NotificationHandler()
        {
        }

        public static NotificationHandler Instance
        {
            get
            {
                if (instance == null) instance = new NotificationHandler();
                return instance;
            }
        }
        
        public void Remove(GameObject go)
        {
            notifications.Remove(go);
        }

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            Main.log.LogDebug($"Start called on NotificationHandler.");
            NotificationLayer = GameObject.Instantiate(ModAssets.Instance.NotificationLayer, this.transform);
            NotificationLayer.SetActive(true);
            //this


            //this.StartCoroutine("ProcessQueue");
            //AddNotification("test1111", 5f);
            //AddNotification("test2222", 5f);
            //AddNotification("test333", 5f);
            StartCoroutine("ProcessQueue");
        }

        public void AddNotification(string text, float duration)
        {
            var newNotification = GameObject.Instantiate(ModAssets.Instance.SkillUp, NotificationLayer.transform);
            var notifyComponent = newNotification.AddComponent<Notification>();

            newNotification.GetComponentInChildren<Text>().text = text;
            notifyComponent.Duration = duration;
            newNotification.SetActive(false);
            incomingNotificationQueue.Enqueue(newNotification);
            Main.log.LogDebug($"Queued message \"{text}\" for processing.");
        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad8)) NotificationHandler.Instance.AddNotification("TestNotification", 5f);
        }

        public IEnumerator ProcessQueue()
        {
            while (true)
            {
                //Main.log.LogDebug($"Tick");
                if (incomingNotificationQueue.Count > 1)
                {
                    //Main.log.LogDebug($"QueueCount: {incomingNotificationQueue.Count}");
                    //Main.log.LogDebug($"Moving?: {currentNotification?.GetComponent<Notification>().isMoving()}");
                    if (!currentNotification) currentNotification = incomingNotificationQueue.Peek();
                    //Main.log.LogDebug($"Moving?: {currentNotification.GetComponent<Notification>().isMoving()}");
                    if (!currentNotification.GetComponent<Notification>().isMoving())
                    {
                        //Main.log.LogDebug($"Not moving.");
                        var incNot = incomingNotificationQueue.Dequeue();
                        incNot.SetActive(true);
                        incNot.gameObject.transform.localPosition = new Vector3(-280, -30, 0); //Starting pos
                        notifications.Add(incNot);
                        //Main.log.LogDebug($"Move all up.");
                        foreach (var n in notifications)
                        {
                            if (n != null && n.GetComponent<Notification>()) n.GetComponent<Notification>().MoveUp();
                        }
                    }
                    //else yield return new WaitForSeconds(1f);
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        private bool areMoving()
        {
            if (notifications.Count == 0) return false;
            foreach(var go in notifications)
            {
                if (go != null)
                {
                    //Main.log.LogDebug($"{go.gameObject.transform.localPosition} vs {go.GetComponent<Notification>().nextPos}");
                    if (go.gameObject.transform.localPosition.Equals(go.GetComponent<Notification>().nextPos)) return true;
                }
            }
            return false;
        }
    }
}
