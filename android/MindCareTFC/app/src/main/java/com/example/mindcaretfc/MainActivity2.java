package com.example.mindcaretfc;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.util.Log;
import android.view.View;
import android.widget.Button;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.Query;
import com.google.firebase.database.ValueEventListener;

public class MainActivity2 extends AppCompatActivity {

    String currentUserEmail;
    DatabaseReference usuariosRef;
    String rol = "";
    Button btnRoles;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main2);

        usuariosRef = FirebaseDatabase.getInstance().getReference().child("usuarios");
        currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();
        btnRoles = findViewById(R.id.btnPacientesActuales);

        initializeToolbar();

        if (currentUserEmail != null) {
            Query query = usuariosRef.orderByChild("Correo").equalTo(currentUserEmail);

            query.addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    if (dataSnapshot.exists()) {
                        DataSnapshot usuarioSnapshot = dataSnapshot.getChildren().iterator().next();
                        rol = usuarioSnapshot.child("Rol").getValue(String.class);
                        updateButtonLabel();
                    }
                }

                @Override
                public void onCancelled(DatabaseError databaseError) {
                    Log.e("DATABASE_ERROR", "Error al leer la base de datos: " + databaseError.getMessage());
                }
            });
        }

        getSupportActionBar().setTitle("MindCare");

    }

    private void initializeToolbar() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.toolbar_menu, menu);
        return true;
    }


    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item) {
        if (item.getTitle().equals("Cerrar Sesión")) {
            FirebaseAuth.getInstance().signOut();
            startActivity(new Intent(MainActivity2.this, MainActivity.class));
            finish();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    private void updateButtonLabel() {
        if (rol.equals("Psicólogo") || rol.equals("Psiquiatra")) {
            btnRoles.setText("Tus pacientes");
        } else {
            btnRoles.setText("Profesional actual");
        }
    }

    public void onChatButtonClick(View view) {
        startActivity(new Intent(MainActivity2.this, MainActivity3.class));
    }

    public void onPerfilButtonClick(View view) {
        startActivity(new Intent(MainActivity2.this, ProfileActivity.class));
    }

    public void onButtonPacientesActuales(View view) {
        if (rol.equals("Psicólogo") || rol.equals("Psiquiatra")) {
            startActivity(new Intent(MainActivity2.this, PacientesListActivity.class));
        } else if (rol.equals("Paciente")) {
            startActivity(new Intent(MainActivity2.this, ProfessionalInfoActivity.class));
        }
    }
}
