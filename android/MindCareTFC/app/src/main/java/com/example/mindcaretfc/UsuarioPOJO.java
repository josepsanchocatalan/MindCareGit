package com.example.mindcaretfc;

import com.google.firebase.database.PropertyName;

public class UsuarioPOJO {

    @PropertyName("Nombre")
    private String Nombre;
    @PropertyName("Apellidos")
    private String Apellidos;
    @PropertyName("Correo")
    private String Correo;
    @PropertyName("Rol")
    private String Rol;

    public UsuarioPOJO() {

    }

    public UsuarioPOJO(String nombre, String apellidos, String correo, String rol) {
        Nombre = nombre;
        Apellidos = apellidos;
        Correo = correo;
        Rol = rol;
    }

    public String getNombre() {
        return Nombre;
    }

    public void setNombre(String nombre) {
        Nombre = nombre;
    }

    public String getApellidos() {
        return Apellidos;
    }

    public void setApellidos(String apellidos) {
        Apellidos = apellidos;
    }

    public String getCorreo() {
        return Correo;
    }

    public void setCorreo(String correo) {
        Correo = correo;
    }

    public String getRol() {
        return Rol;
    }

    public void setRol(String rol) {
        Rol = rol;
    }
}
