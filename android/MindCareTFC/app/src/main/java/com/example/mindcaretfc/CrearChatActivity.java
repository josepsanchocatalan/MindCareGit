package com.example.mindcaretfc;

import android.content.Intent;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.Query;
import com.google.firebase.database.ValueEventListener;

import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

public class CrearChatActivity extends AppCompatActivity {

    private EditText editTextSearch;
    private RecyclerView recyclerViewUsers;
    private DatabaseReference usuariosReference;
    private List<UsuarioPOJO> usuariosList;
    private UsuariosAdapter adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_crear_chat);

        editTextSearch = findViewById(R.id.editTextSearch);
        recyclerViewUsers = findViewById(R.id.recyclerViewUsers);
        recyclerViewUsers.setLayoutManager(new LinearLayoutManager(this));

        usuariosList = new ArrayList<>();
        adapter = new UsuariosAdapter(this, usuariosList);
        recyclerViewUsers.setAdapter(adapter);

        usuariosReference = FirebaseDatabase.getInstance().getReference().child("usuarios");

        usuariosReference.addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                usuariosList.clear();
                for (DataSnapshot snapshot : dataSnapshot.getChildren()) {
                    UsuarioPOJO usuario = snapshot.getValue(UsuarioPOJO.class);
                    if (usuario != null) {
                        usuariosList.add(usuario);
                    }
                }
                adapter.notifyDataSetChanged();
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(CrearChatActivity.this, "Error al cargar los usuarios: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });

        editTextSearch.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                searchUsers(s.toString());
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });

        adapter.setOnItemClickListener(new UsuariosAdapter.OnItemClickListener() {
            @Override
            public void onItemClick(int position) {
                UsuarioPOJO selectedUser = usuariosList.get(position);
                createChatAndOpen(selectedUser);
            }
        });
    }

    private void searchUsers(String searchText) {
        Query query = usuariosReference.orderByChild("nombre")
                .startAt(searchText)
                .endAt(searchText + "\uf8ff");

        query.addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                usuariosList.clear();
                for (DataSnapshot snapshot : dataSnapshot.getChildren()) {
                    UsuarioPOJO usuario = snapshot.getValue(UsuarioPOJO.class);
                    if (usuario != null) {
                        usuariosList.add(usuario);
                    }
                }
                adapter.notifyDataSetChanged();
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(CrearChatActivity.this, "Error al buscar usuarios: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void createChatAndOpen(UsuarioPOJO selectedUser) {
        DatabaseReference chatsReference = FirebaseDatabase.getInstance().getReference().child("chats");
        String currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();
        String selectedUserEmail = selectedUser.getCorreo();

        String chatId = chatsReference.push().getKey();

        Map<String, Object> chatData = new HashMap<>();
        List<String> participantes = new ArrayList<>();
        participantes.add(currentUserEmail);
        participantes.add(selectedUserEmail);
        chatData.put("participantes", participantes);

        assert chatId != null;
        chatsReference.child(chatId).setValue(chatData).addOnCompleteListener(task -> {
            if (task.isSuccessful()) {
                DatabaseReference messagesReference = chatsReference.child(chatId).child("messages");
                String messageId = messagesReference.push().getKey();
                Message initialMessage = new Message("Sistema", "Chat Iniciado", getTimestamp());

                if (messageId != null) {
                    messagesReference.child(messageId).setValue(initialMessage);
                }

                Intent intent = new Intent(CrearChatActivity.this, Chat.class);
                intent.putExtra("chatId", chatId);
                startActivity(intent);
            } else {
                Toast.makeText(CrearChatActivity.this, "Error al crear el chat", Toast.LENGTH_SHORT).show();
            }
        });
    }


    private String getTimestamp() {
        ZonedDateTime now = ZonedDateTime.now();
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSSSSSSXXX", Locale.getDefault());
        String formattedDateTime = now.format(formatter);

        if (formattedDateTime.endsWith("Z")) {
            String offset = now.getOffset().getId();
            formattedDateTime = formattedDateTime.replace("Z", offset);
        }

        return formattedDateTime;
    }
}