package com.example.mindcaretfc;

import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.List;

public class PacientesAdapter extends RecyclerView.Adapter<PacientesAdapter.PacienteViewHolder> {

    private Context context;
    private List<PacientePOJO> pacientesList;

    public PacientesAdapter(Context context, List<PacientePOJO> pacientesList) {
        this.context = context;
        this.pacientesList = pacientesList;
    }

    @NonNull
    @Override
    public PacienteViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_paciente, parent, false);
        return new PacienteViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull PacienteViewHolder holder, int position) {
        PacientePOJO paciente = pacientesList.get(position);
        String nombreCompleto = paciente.getNombre() + " " + paciente.getApellidos();
        holder.textViewPacienteName.setText(nombreCompleto);
        holder.textViewPacienteEmail.setText(paciente.getCorreo());

        holder.itemView.setOnClickListener(v -> {
            Intent intent = new Intent(context, PacienteProfileActivity.class);
            intent.putExtra("pacienteId", paciente.getId());
            context.startActivity(intent);
        });
    }

    @Override
    public int getItemCount() {
        return pacientesList.size();
    }

    public static class PacienteViewHolder extends RecyclerView.ViewHolder {

        TextView textViewPacienteName;
        TextView textViewPacienteEmail;

        public PacienteViewHolder(@NonNull View itemView) {
            super(itemView);
            textViewPacienteName = itemView.findViewById(R.id.textViewPacienteName);
            textViewPacienteEmail = itemView.findViewById(R.id.textViewPacienteEmail);
        }
    }
}
