package com.example.mindcaretfc;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;

public class PacientesListActivity extends AppCompatActivity {

    private RecyclerView recyclerViewPacientes;
    private PacientesAdapter pacientesAdapter;
    private List<PacientePOJO> pacientesList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pacientes_list);

        initializeToolbar();

        recyclerViewPacientes = findViewById(R.id.recyclerViewPacientes);
        recyclerViewPacientes.setLayoutManager(new LinearLayoutManager(this));
        pacientesList = new ArrayList<>();
        pacientesAdapter = new PacientesAdapter(this, pacientesList);
        recyclerViewPacientes.setAdapter(pacientesAdapter);

        String currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();
        DatabaseReference pacientesRef = FirebaseDatabase.getInstance().getReference().child("pacientes");
        pacientesRef.orderByChild("ProfesionalActual").equalTo(currentUserEmail).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                if (dataSnapshot.exists()) {
                    for (DataSnapshot pacienteSnapshot : dataSnapshot.getChildren()) {
                        PacientePOJO paciente = pacienteSnapshot.getValue(PacientePOJO.class);
                        paciente.setId(pacienteSnapshot.getKey());
                        pacientesList.add(paciente);
                    }
                    pacientesAdapter.notifyDataSetChanged();
                } else {
                    Toast.makeText(PacientesListActivity.this, "No hay pacientes asignados", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(PacientesListActivity.this, "Error al cargar pacientes: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });

        getSupportActionBar().setTitle("Pacientes activos");

    }

    private void initializeToolbar() {
        Toolbar toolbar = findViewById(R.id.toolbar_pacienteoptions);
        setSupportActionBar(toolbar);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.toolbar_pacienteoptions, menu);
        return true;
    }


    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item) {
        if (item.getTitle().equals("Cerrar Sesión")) {
            FirebaseAuth.getInstance().signOut();
            startActivity(new Intent(PacientesListActivity.this, MainActivity.class));
            finish();
            return true;
        } else if (item.getTitle().equals("Crear Paciente")) {
            startActivity(new Intent(PacientesListActivity.this, CrearPaciente.class));
        }
        return super.onOptionsItemSelected(item);
    }

}
