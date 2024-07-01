package com.example.mindcaretfc;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
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

public class ProfessionalInfoActivity extends AppCompatActivity {

    private TextView textViewName, textViewEmail, textViewAge, textViewAddress,
            textViewCountry, textViewCity, textViewProvince, textViewProfession,
            textViewNIF, textViewPhone1, textViewPhone2, textViewDirCons, textViewTarifa, textViewHorarios;

    private DatabaseReference pacientesReference, usuariosReference, psicologosReference, psiquiatrasReference;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_professional_info);

        initializeToolbar();



        textViewName = findViewById(R.id.textViewName);
        textViewEmail = findViewById(R.id.textViewEmail);
        textViewAge = findViewById(R.id.textViewAge);
        textViewAddress = findViewById(R.id.textViewAddress);
        textViewCountry = findViewById(R.id.textViewCountry);
        textViewCity = findViewById(R.id.textViewCity);
        textViewProvince = findViewById(R.id.textViewProvince);
        textViewProfession = findViewById(R.id.textViewProfession);
        textViewNIF = findViewById(R.id.textViewNIF);
        textViewPhone1 = findViewById(R.id.textViewPhone1);
        textViewPhone2 = findViewById(R.id.textViewPhone2);
        textViewDirCons = findViewById(R.id.textViewDirCons);
        textViewTarifa = findViewById(R.id.textViewTarifa);
        textViewHorarios = findViewById(R.id.textViewHorarios);

        String currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();

        pacientesReference = FirebaseDatabase.getInstance().getReference().child("pacientes");
        usuariosReference = FirebaseDatabase.getInstance().getReference().child("usuarios");
        psicologosReference = FirebaseDatabase.getInstance().getReference().child("psicologos");
        psiquiatrasReference = FirebaseDatabase.getInstance().getReference().child("psiquiatras");

        pacientesReference.orderByChild("Correo").equalTo(currentUserEmail).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                if (dataSnapshot.exists()) {
                    for (DataSnapshot pacienteSnapshot : dataSnapshot.getChildren()) {
                        String profesionalCorreo = pacienteSnapshot.child("ProfesionalActual").getValue(String.class);
                        if (profesionalCorreo != null) {
                            cargarDatosProfesional(profesionalCorreo);
                        } else {
                            Toast.makeText(ProfessionalInfoActivity.this, "No se encontró el profesional a cargo", Toast.LENGTH_SHORT).show();
                        }
                    }
                } else {
                    Toast.makeText(ProfessionalInfoActivity.this, "Datos de paciente no encontrados", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(ProfessionalInfoActivity.this, "Error al cargar datos de paciente: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }


        });
    }

    private void cargarDatosProfesional(String profesionalCorreo) {
        usuariosReference.orderByChild("Correo").equalTo(profesionalCorreo).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                if (dataSnapshot.exists()) {
                    for (DataSnapshot usuarioSnapshot : dataSnapshot.getChildren()) {
                        String rol = usuarioSnapshot.child("Rol").getValue(String.class);
                        if (rol.equals("Psicólogo")) {
                            cargarDatosPsicologo(profesionalCorreo);
                        } else if (rol.equals("Psiquiatra")) {
                            cargarDatosPsiquiatra(profesionalCorreo);
                        }
                    }
                } else {
                    Toast.makeText(ProfessionalInfoActivity.this, "Datos del profesional no encontrados", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(ProfessionalInfoActivity.this, "Error al cargar datos del profesional: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void cargarDatosPsicologo(String profesionalCorreo) {
        psicologosReference.orderByChild("Correo").equalTo(profesionalCorreo).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                if (dataSnapshot.exists()) {
                    for (DataSnapshot psicologoSnapshot : dataSnapshot.getChildren()) {
                        String nombre = psicologoSnapshot.child("Nombre").getValue(String.class);
                        String apellidos = psicologoSnapshot.child("Apellidos").getValue(String.class);
                        String email = psicologoSnapshot.child("Correo").getValue(String.class);
                        String edad = psicologoSnapshot.child("Edad").getValue(long.class).toString();
                        String direccion = psicologoSnapshot.child("Direccion").getValue(String.class);
                        String pais = psicologoSnapshot.child("Pais").getValue(String.class);
                        String poblacion = psicologoSnapshot.child("Poblacion").getValue(String.class);
                        String provincia = psicologoSnapshot.child("Provincia").getValue(String.class);
                        String telefono1 = psicologoSnapshot.child("Telefono1").getValue(String.class);
                        String telefono2 = psicologoSnapshot.child("Telefono2").getValue(String.class);
                        String dirCons = psicologoSnapshot.child("DireccionConsultorio").getValue(String.class);
                        String tarifa = psicologoSnapshot.child("Tarifas").getValue(long.class).toString();
                        String horarios = psicologoSnapshot.child("HorariosAtencion").getValue(String.class);

                        textViewName.setText(nombre + " " + apellidos);
                        textViewEmail.setText("Correo: " + email);
                        textViewAge.setText("Edad: " + edad);
                        textViewAddress.setText("Dirección: " + direccion);
                        textViewCountry.setText("País: " + pais);
                        textViewCity.setText("Población: " + poblacion);
                        textViewProvince.setText("Provincia: " + provincia);
                        textViewPhone1.setText("Teléfono 1: " + telefono1);
                        textViewPhone2.setText("Teléfono 2: " + telefono2);
                        textViewDirCons.setText("Dirección consultorio: " + dirCons);
                        textViewTarifa.setText("Tarifa: " + tarifa);
                        textViewHorarios.setText("Horarios: " + horarios);

                    }
                } else {
                    Toast.makeText(ProfessionalInfoActivity.this, "Datos del psicólogo no encontrados", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(ProfessionalInfoActivity.this, "Error al cargar datos del psicólogo: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }


    private void cargarDatosPsiquiatra(String profesionalCorreo) {
        psiquiatrasReference.orderByChild("Correo").equalTo(profesionalCorreo).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                if (dataSnapshot.exists()) {
                    for (DataSnapshot psiquiatrasSnapshot : dataSnapshot.getChildren()) {
                        String nombre = psiquiatrasSnapshot.child("Nombre").getValue(String.class);
                        String apellidos = psiquiatrasSnapshot.child("Apellidos").getValue(String.class);
                        String email = psiquiatrasSnapshot.child("Correo").getValue(String.class);
                        String edad = psiquiatrasSnapshot.child("Edad").getValue(long.class).toString();
                        String direccion = psiquiatrasSnapshot.child("Direccion").getValue(String.class);
                        String pais = psiquiatrasSnapshot.child("Pais").getValue(String.class);
                        String poblacion = psiquiatrasSnapshot.child("Poblacion").getValue(String.class);
                        String provincia = psiquiatrasSnapshot.child("Provincia").getValue(String.class);
                        String telefono1 = psiquiatrasSnapshot.child("Telefono1").getValue(String.class);
                        String telefono2 = psiquiatrasSnapshot.child("Telefono2").getValue(String.class);
                        String dirCons = psiquiatrasSnapshot.child("DireccionConsultorio").getValue(String.class);
                        String tarifa = psiquiatrasSnapshot.child("Tarifas").getValue(long.class).toString();
                        String horarios = psiquiatrasSnapshot.child("HorariosAtencion").getValue(String.class);

                        textViewName.setText(nombre + " " + apellidos);
                        textViewEmail.setText("Correo: " + email);
                        textViewAge.setText("Edad: " + edad);
                        textViewAddress.setText("Dirección: " + direccion);
                        textViewCountry.setText("País: " + pais);
                        textViewCity.setText("Población: " + poblacion);
                        textViewProvince.setText("Provincia: " + provincia);
                        textViewPhone1.setText("Teléfono 1: " + telefono1);
                        textViewPhone2.setText("Teléfono 2: " + telefono2);
                        textViewDirCons.setText("Dirección consultorio: " + dirCons);
                        textViewTarifa.setText("Tarifa: " + tarifa);
                        textViewHorarios.setText("Horarios: " + horarios);

                    }
                } else {
                    Toast.makeText(ProfessionalInfoActivity.this, "Datos del psiquiatra no encontrados", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(ProfessionalInfoActivity.this, "Error al cargar datos del psiquiatra: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void initializeToolbar() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setTitle("Profesional actual");
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
            startActivity(new Intent(ProfessionalInfoActivity.this, MainActivity.class));
            finish();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

}