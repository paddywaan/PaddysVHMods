using System;
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

        NotificationHandler()
        {
            NotificationLayer = GameObject.Instantiate(ModAssets.Instance.NotificationLayer, this.transform);
            NotificationLayer.SetActive(true);
        }

        public static NotificationHandler Instance
        {
            get
            {
                if (instance == null) instance = new NotificationHandler();
                return instance;
            }
        }
        public static List<GameObject> notificationQueue;

        public void Start()
        {
            //this
            if (notificationQueue == null) notificationQueue = new List<GameObject>();
            AddNotification("test1111", 5f);
            Thread.Sleep(150);
            AddNotification("test2222", 5f);
            Thread.Sleep(150);
            AddNotification("test3333", 5f);
            Thread.Sleep(150);
            //AddNotification("test4444", 3f);
            //.Sleep(150);

        }
        public void AddNotification(string text, float duration)
        {
            //var go = ModAssets.SkillUp.clo
            var go = GameObject.Instantiate(ModAssets.Instance.SkillUp, NotificationLayer.transform);
            
            var notify = go.AddComponent<SkillNotify>();
            //go.AddComponent<Text>();

            notify.Duration = duration;
            //notify.startPos = NotificationLayer.transform.localPosition;
            notify.startPos= new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + (notificationQueue.Count * 30)-30, this.transform.localPosition.z);
            go.GetComponent<RectTransform>().localPosition = notify.startPos;
            //notify.text.text = text;

            var notifText = go.GetComponentInChildren<Text>();
            //notifText.name = "TestNotif";
            notifText.text = text;
            notify.Duration = duration;
            notificationQueue.Add(go);
            //go.SetActive(true);

        }


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
