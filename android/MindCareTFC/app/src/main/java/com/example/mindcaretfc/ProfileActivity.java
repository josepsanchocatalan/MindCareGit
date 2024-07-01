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
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

public class ProfileActivity extends AppCompatActivity {

    private TextView textViewName, textViewEmail, textViewAge, textViewAddress,
            textViewCountry, textViewCity, textViewProvince, textViewProfession,
            textViewNIF, textViewPhone1, textViewPhone2, textViewDirConsu, textViewTarifa, textViewHorarios, textViewMoreInfo;

    private View separador;

    private DatabaseReference usuariosReference;

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
        textViewProfession = findViewById(R.id.textViewProfession);
        textViewPhone1 = findViewById(R.id.textViewPhone1);
        textViewPhone2 = findViewById(R.id.textViewPhone2);
        textViewDirConsu = findViewById(R.id.textViewDirCons);
        textViewTarifa = findViewById(R.id.textViewTarifa);
        textViewHorarios = findViewById(R.id.textViewHorarios);
        textViewMoreInfo = findViewById(R.id.textViewMoreInfoTitle);
        separador = findViewById(R.id.separator);

        String currentUserEmail = FirebaseAuth.getInstance().getCurrentUser().getEmail();

        usuariosReference = FirebaseDatabase.getInstance().getReference().child("usuarios");

        usuariosReference.orderByChild("Correo").equalTo(currentUserEmail).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                if (dataSnapshot.exists()) {
                    for (DataSnapshot snapshot : dataSnapshot.getChildren()) {
                        String rol = snapshot.child("Rol").getValue(String.class);

                        switch (rol) {
                            case "Paciente":
                                DatabaseReference pacientesRef = FirebaseDatabase.getInstance().getReference().child("pacientes");
                                pacientesRef.orderByChild("Correo").equalTo(currentUserEmail).addListenerForSingleValueEvent(new ValueEventListener() {
                                    @Override
                                    public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                                        if (dataSnapshot.exists()) {
                                            for (DataSnapshot pacienteSnapshot : dataSnapshot.getChildren()) {
                                                String nombre = pacienteSnapshot.child("Nombre").getValue(String.class);
                                                String apellidos = pacienteSnapshot.child("Apellidos").getValue(String.class);
                                                String direccion = pacienteSnapshot.child("Direccion").getValue(String.class);
                                                String edad = pacienteSnapshot.child("Edad").getValue(long.class).toString();
                                                String pais = pacienteSnapshot.child("Pais").getValue(String.class);
                                                String poblacion = pacienteSnapshot.child("Poblacion").getValue(String.class);
                                                String provincia = pacienteSnapshot.child("Provincia").getValue(String.class);
                                                String telefono1 = pacienteSnapshot.child("Telefono").getValue(String.class);
                                                String telefono2 = pacienteSnapshot.child("Telefono2").getValue(String.class);
                                                String profesional = pacienteSnapshot.child("ProfesionalActual").getValue(String.class);

                                                textViewName.setText(nombre + " " + apellidos);
                                                textViewEmail.setText("Email: " + currentUserEmail);
                                                textViewAge.setText("Edad: " + edad);
                                                textViewAddress.setText("Dirección: " + direccion);
                                                textViewCountry.setText("Pais: " + pais);
                                                textViewCity.setText("Población: " + poblacion);
                                                textViewProvince.setText("Provincia: " + provincia);
                                                textViewProfession.setText("Profesional: " + profesional);
                                                textViewPhone1.setText("Teléfono1: " + telefono1);
                                                textViewPhone2.setText(TextUtils.isEmpty(telefono2) ? "Teléfono2: ." : "Teléfono2: " + telefono2);
                                                textViewMoreInfo.setText("");
                                                textViewTarifa.setText("");
                                                textViewHorarios.setText("");
                                                textViewDirConsu.setText("");
                                                separador.setEnabled(false);
                                            }
                                        } else {
                                            Toast.makeText(ProfileActivity.this, "Datos de paciente no encontrados", Toast.LENGTH_SHORT).show();
                                        }
                                    }

                                    @Override
                                    public void onCancelled(@NonNull DatabaseError databaseError) {
                                        Toast.makeText(ProfileActivity.this, "Error al cargar datos de paciente: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
                                    }
                                });
                                break;

                            case "Psicólogo":
                                DatabaseReference psicologosRef = FirebaseDatabase.getInstance().getReference().child("psicologos");
                                psicologosRef.orderByChild("Correo").equalTo(currentUserEmail).addListenerForSingleValueEvent(new ValueEventListener() {
                                    @Override
                                    public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                                        try {
                                            if (dataSnapshot.exists()) {
                                                for (DataSnapshot psicologoSnapshot : dataSnapshot.getChildren()) {
                                                    String nombre = psicologoSnapshot.child("Nombre").getValue(String.class).toString();
                                                    String apellidos = psicologoSnapshot.child("Apellidos").getValue(String.class).toString();
                                                    String direccion = psicologoSnapshot.child("Direccion").getValue(String.class).toString();
                                                    String direccionConsultorio = psicologoSnapshot.child("DireccionConsultorio").getValue(String.class).toString();
                                                    String edad = psicologoSnapshot.child("Edad").getValue(long.class).toString();
                                                    String especialidad = psicologoSnapshot.child("Especialidad").getValue(String.class).toString();
                                                    String horariosAtencion = psicologoSnapshot.child("HorariosAtencion").getValue(String.class).toString();
                                                    String nif = psicologoSnapshot.child("NIF").getValue(String.class).toString();
                                                    String numeroLicencia = psicologoSnapshot.child("NumeroLicencia").getValue(String.class).toString();
                                                    String pais = psicologoSnapshot.child("Pais").getValue(String.class).toString();
                                                    String poblacion = psicologoSnapshot.child("Poblacion").getValue(String.class).toString();
                                                    String provincia = psicologoSnapshot.child("Provincia").getValue(String.class).toString();
                                                    String tarifas = psicologoSnapshot.child("Tarifas").getValue(double.class).toString();
                                                    String telefono1 = psicologoSnapshot.child("Telefono1").getValue(String.class).toString();
                                                    String telefono2 = psicologoSnapshot.child("Telefono2").getValue(String.class).toString();

                                                    if (nombre != null && apellidos != null && direccion != null && edad != null &&
                                                            especialidad != null && horariosAtencion != null && nif != null &&
                                                            numeroLicencia != null && pais != null && poblacion != null &&
                                                            provincia != null && tarifas != null && telefono1 != null) {
                                                        textViewName.setText(nombre + " " + apellidos);
                                                        textViewEmail.setText("Email: " + currentUserEmail);
                                                        textViewAge.setText("Edad: " + edad);
                                                        textViewAddress.setText("Dirección: " + direccion);
                                                        textViewCountry.setText("Pais: " + pais);
                                                        textViewCity.setText("Población: " + poblacion);
                                                        textViewProvince.setText("Provincia: " + provincia);
                                                        textViewProfession.setText("Especialidad: " + especialidad);
                                                        textViewPhone1.setText("Teléfono1: " + telefono1);
                                                        textViewPhone2.setText(TextUtils.isEmpty(telefono2) ? "Teléfono2: ." : "Teléfono2: " + telefono2);
                                                        textViewDirConsu.setText("Dirección consultorio: " + direccionConsultorio);
                                                        textViewHorarios.setText("Horarios: " + horariosAtencion);
                                                        textViewTarifa.setText("Tarifa: " + tarifas);
                                                    } else {
                                                        Toast.makeText(ProfileActivity.this, "Datos de psicólogo incompletos", Toast.LENGTH_SHORT).show();
                                                    }
                                                }
                                            } else {
                                                Toast.makeText(ProfileActivity.this, "Datos de psicólogo no encontrados", Toast.LENGTH_SHORT).show();
                                            }
                                        } catch (Exception ex) {
                                            Toast.makeText(ProfileActivity.this, "Datos no encontrados " + ex.getMessage(), Toast.LENGTH_LONG).show();
                                        }
                                    }

                                    @Override
                                    public void onCancelled(@NonNull DatabaseError databaseError) {
                                        Toast.makeText(ProfileActivity.this, "Error al cargar datos de psicólogo: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
                                    }
                                });
                                break;

                            case "Psiquiatra":
                                DatabaseReference psiquiatrasRef = FirebaseDatabase.getInstance().getReference().child("psiquiatras");
                                psiquiatrasRef.orderByChild("Correo").equalTo(currentUserEmail).addListenerForSingleValueEvent(new ValueEventListener() {
                                    @Override
                                    public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                                        try {
                                            if (dataSnapshot.exists()) {
                                                for (DataSnapshot psiquiatraSnapshot : dataSnapshot.getChildren()) {
                                                    String nombre = psiquiatraSnapshot.child("Nombre").getValue(String.class).toString();
                                                    String apellidos = psiquiatraSnapshot.child("Apellidos").getValue(String.class).toString();
                                                    String direccion = psiquiatraSnapshot.child("Direccion").getValue(String.class).toString();
                                                    String direccionConsultorio = psiquiatraSnapshot.child("DireccionConsultorio").getValue(String.class).toString();
                                                    String edad = psiquiatraSnapshot.child("Edad").getValue(long.class).toString();
                                                    String especialidad = psiquiatraSnapshot.child("Especialidad").getValue(String.class).toString();
                                                    String horariosAtencion = psiquiatraSnapshot.child("HorariosAtencion").getValue(String.class).toString();
                                                    String nif = psiquiatraSnapshot.child("NIF").getValue(String.class).toString();
                                                    String numeroLicencia = psiquiatraSnapshot.child("NumeroLicencia").getValue(String.class).toString();
                                                    String pais = psiquiatraSnapshot.child("Pais").getValue(String.class).toString();
                                                    String poblacion = psiquiatraSnapshot.child("Poblacion").getValue(String.class).toString();
                                                    String provincia = psiquiatraSnapshot.child("Provincia").getValue(String.class).toString();
                                                    String tarifas = psiquiatraSnapshot.child("Tarifas").getValue(double.class).toString();
                                                    String telefono1 = psiquiatraSnapshot.child("Telefono1").getValue(String.class).toString();
                                                    String telefono2 = psiquiatraSnapshot.child("Telefono2").getValue(String.class).toString();

                                                    if (nombre != null && apellidos != null && direccion != null && edad != null &&
                                                            especialidad != null && horariosAtencion != null && nif != null &&
                                                            numeroLicencia != null && pais != null && poblacion != null &&
                                                            provincia != null && tarifas != null && telefono1 != null) {
                                                        textViewName.setText(nombre + " " + apellidos);
                                                        textViewEmail.setText("Email: " + currentUserEmail);
                                                        textViewAge.setText("Edad: " + edad);
                                                        textViewAddress.setText("Dirección: " + direccion);
                                                        textViewCountry.setText("Pais: " + pais);
                                                        textViewCity.setText("Población: " + poblacion);
                                                        textViewProvince.setText("Provincia: " + provincia);
                                                        textViewProfession.setText("Especialidad: " + especialidad);
                                                        textViewPhone1.setText("Teléfono1: " + telefono1);
                                                        textViewPhone2.setText(TextUtils.isEmpty(telefono2) ? "Teléfono2: ." : "Teléfono2: " + telefono2);
                                                        textViewDirConsu.setText("Dirección consultorio: " + direccionConsultorio);
                                                        textViewHorarios.setText("Observaciones: ");


                                                    } else {
                                                        Toast.makeText(ProfileActivity.this, "Datos de psicólogo incompletos", Toast.LENGTH_SHORT).show();
                                                    }
                                                }
                                            } else {
                                                Toast.makeText(ProfileActivity.this, "Datos de psicólogo no encontrados", Toast.LENGTH_SHORT).show();
                                            }
                                        } catch (Exception ex) {
                                            Toast.makeText(ProfileActivity.this, "Datos no encontrados " + ex.getMessage(), Toast.LENGTH_LONG).show();
                                        }
                                    }

                                    @Override
                                    public void onCancelled(@NonNull DatabaseError databaseError) {
                                        Toast.makeText(ProfileActivity.this, "Error al cargar datos de psicólogo: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
                                    }
                                });
                                break;
                        }
                    }
                }
            }




            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {
                Toast.makeText(ProfileActivity.this, "Error al cargar datos: " + databaseError.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });


        getSupportActionBar().setTitle("Perfil");

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
            startActivity(new Intent(ProfileActivity.this, MainActivity.class));
            finish();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

}
