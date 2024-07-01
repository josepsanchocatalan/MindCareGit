package com.example.mindcaretfc;

public class PacientePOJO {

    private String id;
    private String Nombre;
    private String Apellidos;
    private String Correo;

    public PacientePOJO() {

    }

    public PacientePOJO(String Nombre, String Apellidos, String Correo) {
        this.Nombre = Nombre;
        this.Apellidos = Apellidos;
        this.Correo = Correo;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
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
}
