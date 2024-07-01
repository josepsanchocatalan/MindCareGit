package com.example.mindcaretfc;

import com.google.firebase.database.PropertyName;

public class Message {

    @PropertyName("Id")
    private String id;

    @PropertyName("Sender")
    private String sender;

    @PropertyName("Message")
    private String message;

    @PropertyName("Timestamp")
    private String timestamp;


    public Message() {
    }

    public Message(String sender, String message, String timestamp) {
        this.sender = sender;
        this.message = message;
        this.timestamp = timestamp;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getSender() {
        return sender;
    }

    public void setSender(String sender) {
        this.sender = sender;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(String timestamp) {
        this.timestamp = timestamp;
    }
}