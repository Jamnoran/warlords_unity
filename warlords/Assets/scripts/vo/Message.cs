using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.vo {
    public class Message {

        public int recipient;
        public string message;

        public Message() {
        }

        public Message(int rec, string mess) {
            recipient = rec;
            message = mess;
        }

        public int getRecipient() {
            return recipient;
        }

        public void setRecipient(int recipient) {
            this.recipient = recipient;
        }

        public string getMessage() {
            return message;
        }

        public void setMessage(string message) {
            this.message = message;
        }
    }
}
