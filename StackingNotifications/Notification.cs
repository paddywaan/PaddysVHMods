using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace StackingNotifications
{
    public class Notification : MonoBehaviour
    {
        private const float NotificationTime = 3f;
        private RectTransform rect;
        public float timeLeft;
        public Vector3 nextPos;
        RectTransform borderRect;
        bool flag;
        public Vector2 size;

        public void Awake()
        {
            rect = this.gameObject.GetComponent<RectTransform>();
            borderRect = this.transform.Find("borderOverlay").GetComponent<RectTransform>();
            size = rect.sizeDelta;
        }

        public void Start()
        {
            timeLeft = (timeLeft == 0f) ? NotificationTime : timeLeft;

            //Main.log.LogDebug($"{rect}{borderRect}");
            if (!rect || !borderRect) Main.log.LogDebug($"Failed to load assets for Notification: {rect}{borderRect}");
            flag = false;
            //Main.log.LogDebug($"rect.rect.size.x:{rect.rect.size.x}, rect.sizeDelta.x:{rect.sizeDelta.x}, rect.rect.width:{rect.rect.width}, {Size.x}");
            //Size = borderRect.rect;
            this.gameObject.transform.localPosition = new Vector3(-size.x, -size.y, 0); //Starting pos
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
                        NotificationHandler.Instance.Remove(this.gameObject);
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        public void MoveUp()
        {
            //if(!rect || !borderRect) nextPos = new Vector3(rect.localPosition.x, rect.localPosition.y + Size.height, rect.localPosition.z);
            //if (rect.localPosition.y % borderRect.rect.height == 0)
            //{
            var pos = this.gameObject.transform.localPosition;
            nextPos = new Vector3(pos.x, pos.y + size.y, pos.z);
            //Main.log.LogDebug($"MovedUp from {this.gameObject.transform.localPosition} to {nextPos}");
            //    return true;
            //}
            //return false;

        }

        public bool IsOffScreen()
        {
            if (rect.localPosition.y <= -borderRect.rect.height || rect.localPosition.y >= size.y*7) return true;
            return false;
        }

        public bool IsMoving()
        {
            if (this.timeLeft <= 0) return false;
            //Main.log.LogDebug($"comparing {this.gameObject.transform.localPosition} to {nextPos}");
            if (this.gameObject.transform.localPosition.Equals(nextPos) || nextPos.Equals(Vector3.zero))
            {
                return false;
            }
            return true;
        }
    }
}