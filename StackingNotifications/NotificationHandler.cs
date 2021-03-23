using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace StackingNotifications
{
    public class NotificationHandler : MonoBehaviour
    {
        private static NotificationHandler instance;
        private static GameObject NotificationLayer;
        public List<GameObject> notifications = new List<GameObject>();
        private readonly Queue<GameObject> incomingNotificationQueue = new Queue<GameObject>();
        
        private GameObject currentNotification;

        static NotificationHandler()
        {
        }

        public static NotificationHandler Instance
        {
            get
            {
                if (!instance) instance = new NotificationHandler();
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
            StartCoroutine("ProcessQueue");
        }

        public void AddNotification(string text, float duration = 3f)
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
#if DEBUG
            if (Input.GetKeyDown(KeyCode.Keypad8)) NotificationHandler.Instance.AddNotification("TestNotification", 5f);
#endif
        }

        public IEnumerator ProcessQueue()
        {
            while (true)
            {
                if (incomingNotificationQueue.Count > 0)
                {
                    if (!currentNotification) currentNotification = incomingNotificationQueue.Peek();
                    //Main.log.LogDebug($"Moving?: {currentNotification.GetComponent<Notification>().isMoving()}");
                    var notComponent = currentNotification.GetComponent<Notification>();
                    if (!notComponent.IsMoving())
                    {
                        //Main.log.LogDebug($"Not moving.");
                        var incNot = incomingNotificationQueue.Dequeue();
                        
                        incNot.gameObject.transform.localPosition = new Vector3(-notComponent.Size.width, -notComponent.Size.height, 0); //Starting pos
                        incNot.SetActive(true);
                        notifications.Add(incNot);
                        //Main.log.LogDebug($"Move all up.");
                        foreach (var n in notifications)
                        {
                            if (n!= null && n.GetComponent<Notification>()) n.GetComponent<Notification>().MoveUp();
                        }
                    }
                    //else yield return new WaitForSeconds(1f);
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
