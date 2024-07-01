package com.example.mindcaretfc;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import java.text.SimpleDateFormat;
import java.util.List;
import java.util.Locale;

public class MessageAdapter extends ArrayAdapter<Message> {

    public MessageAdapter(Context context, List<Message> messages) {
        super(context, 0, messages);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        Message message = getItem(position);

        if (convertView == null) {
            convertView = LayoutInflater.from(getContext()).inflate(R.layout.item_message, parent, false);
        }

        TextView textViewSender = convertView.findViewById(R.id.textViewSender);
        TextView textViewMessage = convertView.findViewById(R.id.textViewMessage);
        TextView textViewTimestamp = convertView.findViewById(R.id.textViewTimestamp);

        textViewSender.setText(message.getSender());
        textViewMessage.setText(message.getMessage());
        textViewTimestamp.setText(message.getTimestamp());

        return convertView;
    }
}