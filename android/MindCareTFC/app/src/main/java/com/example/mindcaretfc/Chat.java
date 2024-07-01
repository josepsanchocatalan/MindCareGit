package com.example.mindcaretfc;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class Chat extends AppCompatActivity {

    private ListView listViewMessages;
    private EditText editTextMessage;
    private Button buttonSend;
    private DatabaseReference messagesReference;
    private DatabaseReference chatReference;
    private String chatId;
    private String currentUserEmail;
    private MessageAdapter messageAdapter;
    private List<Message> messageList;
    private Toolbar toolbarChat;
    private String otherUserName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_chat);

        toolbarChat = findViewById(R.id.toolbar_chat);
        setSupportActionBar(toolbarChat);

        listViewMessages = findViewById(R.id.listViewMessages);
        editTextMessage = findViewById(R.id.editTextMessage);
        buttonSend = findViewById(R.id.buttonSend);

        chatId = getIntent().getStringExtra("chatId");
        currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();

        messagesReference = FirebaseDatabase.getInstance().getReference()
                .child("chats").child(chatId).child("messages");
        chatReference = FirebaseDatabase.getInstance().getReference()
                .child("chats").child(chatId);

        messageList = new ArrayList<>();
        messageAdapter = new MessageAdapter(this, messageList);
        listViewMessages.setAdapter(messageAdapter);

        buttonSend.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                sendMessage();
            }
        });

        loadMessages();

        getOtherUserName();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.toolbar_chat, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == R.id.action_delete_chat) {
            deleteChat();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    private void deleteChat() {
        chatReference.removeValue().addOnCompleteListener(new OnCompleteListener<Void>() {
            @Override
            public void onComplete(@NonNull Task<Void> task) {
                if (task.isSuccessful()) {
                    Toast.makeText(Chat.this, "Chat eliminado correctamente", Toast.LENGTH_SHORT).show();
                    finish();
                } else {
                    Toast.makeText(Chat.this, "Error al eliminar el chat", Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    private void getOtherUserName() {
        chatReference.child("participantes").addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                for (DataSnapshot participantSnapshot : dataSnapshot.getChildren()) {
                    String participantEmail = participantSnapshot.getValue(String.class);
                    if (!participantEmail.equals(currentUserEmail)) {
                        otherUserName = participantEmail;
                        getSupportActionBar().setTitle(otherUserName);
                        break;
                    }
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(Chat.this, "Error: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void sendMessage() {
        String messageText = editTextMessage.getText().toString().trim();
        if (!TextUtils.isEmpty(messageText)) {
            String timestamp = getTimestamp();
            Message message = new Message(currentUserEmail, messageText, timestamp);

            String messageId = messagesReference.push().getKey();
            if (messageId != null) {
                messagesReference.child(messageId).setValue(message)
                        .addOnCompleteListener(new OnCompleteListener<Void>() {
                            @Override
                            public void onComplete(@NonNull Task<Void> task) {
                                if (task.isSuccessful()) {
                                    editTextMessage.setText("");
                                } else {
                                    Toast.makeText(Chat.this, "Error al enviar el mensaje", Toast.LENGTH_SHORT).show();
                                }
                            }
                        });
            }
        }
    }

    private String getTimestamp() {
        ZonedDateTime now = ZonedDateTime.now();
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSSSSSX", Locale.getDefault());
        String formattedDateTime = now.format(formatter);

        if (formattedDateTime.endsWith("Z")) {
            String offset = now.getOffset().getId();
            formattedDateTime = formattedDateTime.replace("Z", offset);
        }

        return formattedDateTime;
    }

    private void loadMessages() {
        messagesReference.addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                messageList.clear();
                for (DataSnapshot snapshot : dataSnapshot.getChildren()) {
                    Message message = snapshot.getValue(Message.class);
                    if (message != null) {
                        messageList.add(message);
                    }
                }
                messageAdapter.notifyDataSetChanged();
                listViewMessages.setSelection(messageAdapter.getCount() - 1);
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(Chat.this, "Error al cargar los mensajes: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}