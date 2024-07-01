package com.example.mindcaretfc;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.auth.UserProfileChangeRequest;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
public class CrearPaciente extends AppCompatActivity {

    private EditText editTextNombre, editTextApellidos, editTextCorreo, editTextEdad,
            editTextNIF, editTextDireccion, editTextPais, editTextPoblacion,
            editTextProvincia, editTextTelefono, editTextTelefono2;

    private TextView textViewUserActual;

    private Button btnSavePaciente;

    private DatabaseReference databaseReference;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_crear_paciente);

        databaseReference = FirebaseDatabase.getInstance().getReference().child("pacientes");

        editTextNombre = findViewById(R.id.editTextNombre);
        editTextApellidos = findViewById(R.id.editTextApellidos);
        editTextCorreo = findViewById(R.id.editTextCorreo);
        editTextEdad = findViewById(R.id.editTextEdad);
        editTextNIF = findViewById(R.id.editTextNIF);
        editTextDireccion = findViewById(R.id.editTextDireccion);
        editTextPais = findViewById(R.id.editTextPais);
        editTextPoblacion = findViewById(R.id.editTextPoblacion);
        editTextProvincia = findViewById(R.id.editTextProvincia);
        editTextTelefono = findViewById(R.id.editTextTelefono);
        editTextTelefono2 = findViewById(R.id.editTextTelefono2);
        textViewUserActual = findViewById(R.id.textViewProfesional);
        textViewUserActual.setText("");

        textViewUserActual.setText(FirebaseAuth.getInstance().getCurrentUser().getEmail());

        btnSavePaciente = findViewById(R.id.btnGuardarPaciente);
        btnSavePaciente.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                guardarPaciente();
            }
        });
    }

    private void guardarPaciente() {
        String nombre = editTextNombre.getText().toString().trim();
        String apellidos = editTextApellidos.getText().toString().trim();
        String correo = editTextCorreo.getText().toString().trim();
        int edad = editTextEdad.getText().length();
        String nif = editTextNIF.getText().toString().trim();
        String direccion = editTextDireccion.getText().toString().trim();
        String pais = editTextPais.getText().toString().trim();
        String poblacion = editTextPoblacion.getText().toString().trim();
        String provincia = editTextProvincia.getText().toString().trim();
        String telefono = editTextTelefono.getText().toString().trim();
        String telefono2 = editTextTelefono2.getText().toString().trim();
        String profesionalActual = FirebaseAuth.getInstance().getCurrentUser().getEmail();


        if (nombre.isEmpty() || apellidos.isEmpty() || correo.isEmpty() || nif.isEmpty()) {
            Toast.makeText(this, "Por favor, completa todos los campos obligatorios", Toast.LENGTH_SHORT).show();
            return;
        }


        Paciente paciente = new Paciente(nombre, apellidos, correo, edad, nif, direccion,
                pais, poblacion, provincia, telefono, telefono2, profesionalActual);


        String pacienteId = databaseReference.push().getKey();
        databaseReference.child(pacienteId).setValue(paciente);

        guardarUsuario(nombre, apellidos, correo, "paciente", nif);

        Toast.makeText(this, "Paciente guardado correctamente", Toast.LENGTH_SHORT).show();

        limpiarCampos();
    }

    private void limpiarCampos() {
        editTextNombre.setText("");
        editTextApellidos.setText("");
        editTextCorreo.setText("");
        editTextEdad.setText("");
        editTextNIF.setText("");
        editTextDireccion.setText("");
        editTextPais.setText("");
        editTextPoblacion.setText("");
        editTextProvincia.setText("");
        editTextTelefono.setText("");
        editTextTelefono2.setText("");
    }

    private void guardarUsuario(String nombre, String apellidos, String correo, String rol, String NIF) {

        String contrasenya = generarContraseña(NIF);

        DatabaseReference usuariosRef = FirebaseDatabase.getInstance().getReference().child("usuarios");

        UsuarioPOJO usuario = new UsuarioPOJO(nombre, apellidos, correo, rol);

        String usuarioId = usuariosRef.push().getKey();
        usuariosRef.child(usuarioId).setValue(usuario);


        FirebaseAuth.getInstance().createUserWithEmailAndPassword(correo, contrasenya)
                .addOnCompleteListener(task -> {
                    if (task.isSuccessful()) {
                        FirebaseUser firebaseUser = task.getResult().getUser();
                        UserProfileChangeRequest profileUpdates = new UserProfileChangeRequest.Builder()
                                .setDisplayName(correo)
                                .build();
                        firebaseUser.updateProfile(profileUpdates);
                        Toast.makeText(CrearPaciente.this, "Usuario creado exitosamente", Toast.LENGTH_SHORT).show();
                        startActivity(new Intent(CrearPaciente.this, PacientesListActivity.class));
                        this.finish();
                    } else {
                        Toast.makeText(CrearPaciente.this, "Error al crear usuario: " + task.getException().getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });

    }

    public String generarContraseña(String nif) {

        String contraseña = "mc" + nif + "1";
        return contraseña;
    }


}