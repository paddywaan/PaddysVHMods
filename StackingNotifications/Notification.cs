using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace StackingNotifications
{
    public class Notification : MonoBehaviour
    {
        private const float NotificationTime = 5f;
        private RectTransform rect;
        public float Duration;
        private float timeLeft;
        public Vector3 nextPos;
        RectTransform borderRect;
        public Rect Size;
        bool flag;


        public void Start()
        {
            rect = this.gameObject.GetComponent<RectTransform>();
            borderRect = this.transform.Find("borderOverlay").GetComponent<RectTransform>();
            timeLeft = (timeLeft == 0f) ? NotificationTime : timeLeft;
            Size = rect.rect;
            //Main.log.LogDebug($"{rect}{borderRect}");
            if (!rect || !borderRect) Main.log.LogDebug($"Failed to load assets for Notification: {rect}{borderRect}");
            flag = false;
            this.gameObject.transform.localPosition = new Vector3(-Size.width, -Size.height, 0); //Starting pos
            //this.gameObject.SetActive(true);
        }

        public void Update()
        {
            if (rect && borderRect)
            {
                
                var goPos = this.gameObject.transform.localPosition;
                //Main.log.LogDebug($"{goPos}");
                if (goPos.y < nextPos.y) this.gameObject.transform.localPosition = new Vector3(goPos.x, goPos.y + 1, goPos.z);
                else flag = true;
                if (timeLeft > 0 && flag) timeLeft -= Time.deltaTime;
                if (timeLeft < 0)
                {
                    var cg = this.gameObject.GetComponent<CanvasGroup>();
                    //this.gameObject.GetComponent<Renderer>().
                    //if (!isOffScreen())
                    //{
                    this.gameObject.transform.localPosition = new Vector3(goPos.x, goPos.y + 2, goPos.z);
                        
                    if (cg) cg.alpha -= 0.01f;
                    //}
                    if (cg.alpha <= 0)
                    {
                        NotificationHandler.Instance.Remove(rect.gameObject);
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        public void MoveUp()
        {
            //if(!rect || !borderRect) nextPos = new Vector3(rect.localPosition.x, rect.localPosition.y + 30, rect.localPosition.z);
            //if (rect.localPosition.y % borderRect.rect.height == 0)
            //{
            nextPos = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y + 30, this.gameObject.transform.localPosition.z);
            //Main.log.LogDebug($"MovedUp from {this.gameObject.transform.localPosition} to {nextPos}");
            //    return true;
            //}
            //return false;

        }

        public bool IsOffScreen()
        {
            if (rect.localPosition.y <= -borderRect.rect.height || rect.localPosition.y >= Size.height*7) return true;
            return false;
        }

        public bool IsMoving()
        {
            if (this.timeLeft < 0) return false;
            //Main.log.LogDebug($"comparing {this.gameObject.transform.localPosition} to {nextPos}");
            if (this.gameObject.transform.localPosition.Equals(nextPos) || nextPos.Equals(Vector3.zero))
            {
                return false;
            }
            return true;
        }
    }
}