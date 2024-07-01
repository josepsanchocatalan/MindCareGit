package com.example.mindcaretfc;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

public class PacienteProfileActivity extends AppCompatActivity {

    private TextView textViewName, textViewEmail, textViewAge, textViewAddress,
            textViewCountry, textViewCity, textViewProvince, textViewNIF, textViewPhone1, textViewPhone2, textViewDirConsu, textViewProfAct, textViewHorario, textViewTarifa, textViewInfo;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_profile);

        initializeToolbar();

        textViewName = findViewById(R.id.textViewName);
        textViewEmail = findViewById(R.id.textViewEmail);
        textViewAge = findViewById(R.id.textViewAge);
        textViewAddress = findViewById(R.id.textViewAddress);
        textViewCountry = findViewById(R.id.textViewCountry);
        textViewCity = findViewById(R.id.textViewCity);
        textViewProvince = findViewById(R.id.textViewProvince);
        textViewPhone1 = findViewById(R.id.textViewPhone1);
        textViewPhone2 = findViewById(R.id.textViewPhone2);
        textViewDirConsu = findViewById(R.id.textViewDirCons);
        textViewProfAct = findViewById(R.id.textViewProfession);
        textViewHorario = findViewById(R.id.textViewHorarios);
        textViewTarifa = findViewById(R.id.textViewTarifa);
        textViewInfo = findViewById(R.id.textViewMoreInfoTitle);

        String pacienteId = getIntent().getStringExtra("pacienteId");

        DatabaseReference pacientesRef = FirebaseDatabase.getInstance().getReference().child("pacientes").child(pacienteId);
        pacientesRef.addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                if (dataSnapshot.exists()) {
                    String nombre = dataSnapshot.child("Nombre").getValue(String.class);
                    String apellidos = dataSnapshot.child("Apellidos").getValue(String.class);
                    String correo = dataSnapshot.child("Correo").getValue(String.class);
                    String direccion = dataSnapshot.child("Direccion").getValue(String.class);
                    String edad = dataSnapshot.child("Edad").getValue(long.class).toString();
                    String nif = dataSnapshot.child("NIF").getValue(String.class);
                    String pais = dataSnapshot.child("Pais").getValue(String.class);
                    String poblacion = dataSnapshot.child("Poblacion").getValue(String.class);
                    String provincia = dataSnapshot.child("Provincia").getValue(String.class);
                    String telefono1 = dataSnapshot.child("Telefono").getValue(String.class);
                    String telefono2 = dataSnapshot.child("Telefono2").getValue(String.class);

                    String currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();

                    textViewName.setText(nombre + " " + apellidos);
                    textViewEmail.setText("Correo: " + correo);
                    textViewAge.setText("Edad: " + edad);
                    textViewAddress.setText("Dirección: " + direccion);
                    textViewCountry.setText("Pais: " + pais);
                    textViewCity.setText("Población: " + poblacion);
                    textViewProvince.setText("Provincia: " + provincia);
                    textViewPhone1.setText("Teléfono1: " + telefono1);
                    textViewPhone2.setText(telefono2 != null ? "Teléfono2: " + telefono2 : "Teléfono2: .");
                    textViewDirConsu.setText("");
                    textViewProfAct.setText("Profesional actual: " + currentUserEmail);
                    textViewTarifa.setText("");
                    textViewHorario.setText("");
                    textViewInfo.setText("");

                } else {
                    Toast.makeText(PacienteProfileActivity.this, "Datos del paciente no encontrados", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(PacienteProfileActivity.this, "Error al cargar datos del paciente: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
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
            startActivity(new Intent(PacienteProfileActivity.this, MainActivity.class));
            finish();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

}
