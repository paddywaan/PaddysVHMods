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
        private GameObject NotificationLayer;
        public List<GameObject> notifications;
        private Queue<GameObject> incomingNotificationQueue;
        private GameObject currentNotification;

        NotificationHandler()
        {
            if (notifications == null) notifications = new List<GameObject>();
            if (incomingNotificationQueue == null) incomingNotificationQueue = new Queue<GameObject>();
        }

        public static NotificationHandler Instance
        {
            get
            {
                if (instance == null) instance = new NotificationHandler();
                return instance;
            }
        }
        

        public void Start()
        {
            NotificationLayer = GameObject.Instantiate(ModAssets.Instance.NotificationLayer, this.transform);
            NotificationLayer.SetActive(true);
            //this


            //this.StartCoroutine("ProcessQueue");
            //AddNotification("test1111", 5f);
            //AddNotification("test2222", 5f);
            //AddNotification("test333", 5f);
            this.StartCoroutine("ProcessQueue");
        }

        public void AddNotification(string text, float duration)
        {
            var newNotification = GameObject.Instantiate(ModAssets.Instance.SkillUp, NotificationLayer.transform);
            var notifyComponent = newNotification.AddComponent<Notification>();
            
            newNotification.GetComponentInChildren<Text>().text = text;
            notifyComponent.Duration = duration;
            newNotification.SetActive(false);
            incomingNotificationQueue.Enqueue(newNotification);

            //notifications.Add(newNotification);
            //foreach (var n in notifications)
            //{
            //    if(n.GetComponent<Notification>().MoveUp());
            //}
        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad8)) AddNotification("TestNotification", 5f);
        }

        public IEnumerator ProcessQueue()
        {
            while (true)
            {
                if (incomingNotificationQueue.Count > 1)
                {
                    //Main.log.LogDebug($"Moving?: {currentNotification?.GetComponent<Notification>().isMoving()}");
                    if (!currentNotification) currentNotification = incomingNotificationQueue.Peek();
                    Main.log.LogDebug($"Moving?: {currentNotification.GetComponent<Notification>().isMoving()}");
                    if (!currentNotification.GetComponent<Notification>().isMoving())
                    {
                        
                        var incNot = incomingNotificationQueue.Dequeue();
                        incNot.SetActive(true);
                        incNot.gameObject.transform.localPosition = new Vector3(-280, -30, 0); //Starting pos
                        notifications.Add(incNot);
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

        //private bool isMoving()
        //{
        //    if(currentNotification)
        //    {
        //        if(currentNotification.GetComponent<Notification>().)
        //    }
        //}

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
        //public void AddNotification(string text, float duration)
        //{
        //    //var go = ModAssets.SkillUp.clo
        //    var go = GameObject.Instantiate(ModAssets.Instance.SkillUp, NotificationLayer.transform);
            
        //    var notify = go.AddComponent<SkillNotify>();
        //    //go.AddComponent<Text>();

        //    notify.Duration = duration;
        //    //notify.startPos = NotificationLayer.transform.localPosition;
        //    notify.startPos= new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + (notifications.Count * 30)-30, this.transform.localPosition.z);
        //    go.GetComponent<RectTransform>().localPosition = notify.startPos;
        //    //notify.text.text = text;

        //    var notifText = go.GetComponentInChildren<Text>();
        //    //notifText.name = "TestNotif";
        //    notifText.text = text;
        //    notify.Duration = duration;
        //    notifications.Add(go);
        //    //go.SetActive(true);

        //}


        //go.transform.SetParent(instance.transform.parent);

        //notify.text.text = text;
        /*
        go.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
        go.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
        go.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        go.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        */
        //go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        //go.GetComponent<RectTransform>().
        ///go.transform.localPosition = new Vector3(0, 0, 0);
    }
}
