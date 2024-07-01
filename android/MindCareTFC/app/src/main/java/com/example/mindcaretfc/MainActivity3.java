package com.example.mindcaretfc;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;

public class MainActivity3 extends AppCompatActivity {

    private ListView listViewChats;
    private DatabaseReference chatsReference;
    private List<String> chatIds;
    private ArrayAdapter<String> chatAdapter;
    private String currentUserEmail;
    private Toolbar toolbarChatOptions;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main3);

        toolbarChatOptions = findViewById(R.id.toolbar_chatoptions);
        setSupportActionBar(toolbarChatOptions);

        listViewChats = findViewById(R.id.listViewChats);
        chatIds = new ArrayList<>();
        chatAdapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, chatIds);
        listViewChats.setAdapter(chatAdapter);

        currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();
        chatsReference = FirebaseDatabase.getInstance().getReference().child("chats");

        loadUserChats();

        listViewChats.setOnItemClickListener((parent, view, position, id) -> {
            String chatId = chatIds.get(position);
            Intent intent = new Intent(MainActivity3.this, Chat.class);
            intent.putExtra("chatId", chatId);
            startActivity(intent);
        });

        getSupportActionBar().setTitle("Chat");
    }

    private void loadUserChats() {
        chatsReference.addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                chatIds.clear();
                for (DataSnapshot chatSnapshot : dataSnapshot.getChildren()) {
                    DataSnapshot participantsSnapshot = chatSnapshot.child("participantes");
                    boolean isUserParticipant = false;
                    for (DataSnapshot participantSnapshot : participantsSnapshot.getChildren()) {
                        String participantEmail = participantSnapshot.getValue(String.class);
                        if (participantEmail.equals(currentUserEmail)) {
                            isUserParticipant = true;
                            break;
                        }
                    }
                    if (isUserParticipant) {
                        chatIds.add(chatSnapshot.getKey());
                    }
                }
                chatAdapter.notifyDataSetChanged();
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(MainActivity3.this, "Error: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.toolbar_chatoptions, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == R.id.action_create_chat) {
            createChat();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    private void createChat() {
        Intent intent = new Intent(MainActivity3.this, CrearChatActivity.class);
        startActivity(intent);
    }
}