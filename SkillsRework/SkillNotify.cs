using System;
using UnityEngine;
using UnityEngine.UI;
namespace SkillsRework
{
    internal class SkillNotify : MonoBehaviour
    {
        private const float NotificationTime = 3f;
        private float time;
        private RectTransform rect;
        //private string text = "";
        public string text;
        public float Duration;
        private float timeLeft;
        public Vector3 startPos;
        GameObject border;
        RectTransform borderRect;
        bool flag;

        public SkillNotify()
        {
            //text = this.gameObject.AddComponent<Text>();
        }
        public void Start()
        {
            timeLeft = Time.time + ((timeLeft == 0f) ? NotificationTime : timeLeft);
            this.gameObject.SetActive(true);
            Main.log.LogDebug($"Start called for {text}.");
            rect = this.gameObject.GetComponent<RectTransform>();
            
            Main.log.LogDebug($"objectRect: {rect}");

            flag = false;
            borderRect = this.transform.Find("borderOverlay").GetComponent<RectTransform>();
            //startPos = new Vector3(this.gameObject.tralocalPosition.x - borderRect.rect.width, rect.localPosition.y - borderRect.rect.height, rect.localPosition.z);
            startPos = new Vector3(this.transform.localPosition.x - borderRect.rect.width, this.transform.localPosition.y, this.transform.localPosition.z);
            rect.localPosition = new Vector3(this.transform.localPosition.x - borderRect.rect.width, -30, 0);
            Main.log.LogDebug($"borderRect: {borderRect}");
            //rect.localPosition = startPos; //new Vector3(rect.localPosition.x, rect.localPosition.y- borderRect.sizeDelta.y, rect.localPosition.z);
            time = Time.time;







            //border = this.gameObject.GetComponentInParent<Image>();


            /*var txt = base.transform.Find("Image (1)/Text").GetComponent<Text>();
            if (txt != null)
            {
                Main.log.LogDebug($"{txt}");
                text.text = "text";
                text.transform.SetParent(this.transform.parent);
                //txt.name = "Test message";
            }*/
        }
        public void Update()
        {
            if (flag) timeLeft -= Time.deltaTime;
            if (rect.localPosition.y < startPos.y + borderRect.rect.height && !flag)
            {
                
                rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + 1, rect.localPosition.z);
            }else flag = true;
            
            //if (Time.time < timeLeft)
            //{
            //    if (rect.localPosition.y < startPos.y + borderRect.sizeDelta.y) rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + 1, rect.localPosition.z);
            //}
            if (Time.time > timeLeft)
            {
                if (!isOffScreen()) rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y - 1, rect.localPosition.z);
            }
            if (isOffScreen()) Destroy(this.gameObject);
        }
        public bool isOffScreen()
        {
            //Main.log.LogDebug($"{rect.localPosition.y}");
            if (rect.localPosition.y <= -borderRect.rect.height*2) return true;
            return false;
        }

        //void IDisposable.Dispose()
        //{
        //    Destroy(this.gameObject);
        //}
    }
}