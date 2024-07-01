package com.example.mindcaretfc;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.Firebase;
import com.google.firebase.FirebaseApp;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.auth.AuthResult;

public class MainActivity extends AppCompatActivity {

    private FirebaseAuth mAuth;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        FirebaseApp.initializeApp(this);

        mAuth = FirebaseAuth.getInstance();



    }

    private void signIn(String email, String password) {
        mAuth.signInWithEmailAndPassword(email, password)
                .addOnCompleteListener(this, new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            Toast.makeText(MainActivity.this, "Autenticacion correcta", Toast.LENGTH_SHORT).show();
                            startActivity((new Intent(MainActivity.this, MainActivity2.class)));                            finish();
                        } else {
                            // Fallo de inicio de sesión, mostrar mensaje de error al usuario
                            Toast.makeText(MainActivity.this, "Autenticacion erronea", Toast.LENGTH_SHORT).show();
                        }
                    }
                });
    }

    public void onIngresarBtn(View view) {
        EditText emailText = findViewById(R.id.editTextCorreo);
        EditText passText = findViewById(R.id.editTextContrasena);

        String email = emailText.getText().toString();
        String pass = passText.getText().toString();

        signIn(email, pass);
    }
}