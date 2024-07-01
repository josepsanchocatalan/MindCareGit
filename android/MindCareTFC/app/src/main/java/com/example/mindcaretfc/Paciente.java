package com.example.mindcaretfc;

import com.google.firebase.database.Exclude;
import com.google.firebase.database.PropertyName;

public class Paciente {

    @PropertyName("Nombre")
    private String nombre;
    @PropertyName("Apellidos")
    private String apellidos;
    @PropertyName("Correo")
    private String correo;
    @PropertyName("Edad")
    private int edad;
    @PropertyName("NIF")
    private String nif;
    @PropertyName("Direccion")
    private String direccion;
    @PropertyName("Pais")
    private String pais;
    @PropertyName("Poblacion")
    private String poblacion;
    @PropertyName("Provincia")
    private String provincia;
    @PropertyName("Telefono")
    private String telefono;
    @PropertyName("Telefono2")
    private String telefono2;
    @PropertyName("ProfesionalActual")
    private String profesionalActual;

    public Paciente() {

    }

    public Paciente(String nombre, String apellidos, String correo, int edad, String nif, String direccion, String pais, String poblacion, String provincia, String telefono, String telefono2, String profesionalActual) {
        this.nombre = nombre;
        this.apellidos = apellidos;
        this.correo = correo;
        this.edad = edad;
        this.nif = nif;
        this.direccion = direccion;
        this.pais = pais;
        this.poblacion = poblacion;
        this.provincia = provincia;
        this.telefono = telefono;
        this.telefono2 = telefono2;
        this.profesionalActual = profesionalActual;
    }

    @PropertyName("Nombre")
    public String getNombre() {
        return nombre;
    }

    @PropertyName("Nombre")
    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    @PropertyName("Apellidos")
    public String getApellidos() {
        return apellidos;
    }

    @PropertyName("Apellidos")
    public void setApellidos(String apellidos) {
        this.apellidos = apellidos;
    }

    @PropertyName("Correo")
    public String getCorreo() {
        return correo;
    }

    @PropertyName("Correo")
    public void setCorreo(String correo) {
        this.correo = correo;
    }

    @PropertyName("Edad")
    public int getEdad() {
        return edad;
    }

    @PropertyName("Edad")
    public void setEdad(int edad) {
        this.edad = edad;
    }

    @PropertyName("NIF")
    public String getNif() {
        return nif;
    }

    @PropertyName("NIF")
    public void setNif(String nif) {
        this.nif = nif;
    }

    @PropertyName("Direccion")
    public String getDireccion() {
        return direccion;
    }

    @PropertyName("Direccion")
    public void setDireccion(String direccion) {
        this.direccion = direccion;
    }

    @PropertyName("Pais")
    public String getPais() {
        return pais;
    }

    @PropertyName("Pais")
    public void setPais(String pais) {
        this.pais = pais;
    }

    @PropertyName("Poblacion")
    public String getPoblacion() {
        return poblacion;
    }

    @PropertyName("Poblacion")
    public void setPoblacion(String poblacion) {
        this.poblacion = poblacion;
    }

    @PropertyName("Provincia")
    public String getProvincia() {
        return provincia;
    }

    @PropertyName("Provincia")
    public void setProvincia(String provincia) {
        this.provincia = provincia;
    }

    @PropertyName("Telefono")
    public String getTelefono() {
        return telefono;
    }

    @PropertyName("Telefono")
    public void setTelefono(String telefono) {
        this.telefono = telefono;
    }

    @PropertyName("Telefono2")
    public String getTelefono2() {
        return telefono2;
    }

    @PropertyName("Telefono2")
    public void setTelefono2(String telefono2) {
        this.telefono2 = telefono2;
    }

    @PropertyName("ProfesionalActual")
    public String getProfesionalActual() {
        return profesionalActual;
    }

    @PropertyName("ProfesionalActual")
    public void setProfesionalActual(String profesionalActual) {
        this.profesionalActual = profesionalActual;
    }

}
