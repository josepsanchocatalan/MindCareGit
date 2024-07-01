package com.example.mindcaretfc;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.List;

public class UsuariosAdapter extends RecyclerView.Adapter<UsuariosAdapter.UsuarioViewHolder> {

    private Context context;
    private List<UsuarioPOJO> usuariosList;
    private OnItemClickListener onItemClickListener;

    public interface OnItemClickListener {
        void onItemClick(int position);
    }

    public void setOnItemClickListener(OnItemClickListener onItemClickListener) {
        this.onItemClickListener = onItemClickListener;
    }

    public UsuariosAdapter(Context context, List<UsuarioPOJO> usuariosList) {
        this.context = context;
        this.usuariosList = usuariosList;
    }

    @NonNull
    @Override
    public UsuarioViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_usuario, parent, false);
        return new UsuarioViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull UsuarioViewHolder holder, int position) {
        UsuarioPOJO usuario = usuariosList.get(position);
        holder.textViewNombre.setText(usuario.getNombre());
        holder.textViewApellidos.setText(usuario.getApellidos());
        holder.textViewCorreo.setText(usuario.getCorreo());
    }

    @Override
    public int getItemCount() {
        return usuariosList.size();
    }

    class UsuarioViewHolder extends RecyclerView.ViewHolder {

        TextView textViewNombre;
        TextView textViewApellidos;
        TextView textViewCorreo;

        public UsuarioViewHolder(@NonNull View itemView) {
            super(itemView);
            textViewNombre = itemView.findViewById(R.id.textViewNombre);
            textViewApellidos = itemView.findViewById(R.id.textViewApellidos);
            textViewCorreo = itemView.findViewById(R.id.textViewCorreo);

            itemView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    if (onItemClickListener != null) {
                        int position = getAdapterPosition();
                        if (position != RecyclerView.NO_POSITION) {
                            onItemClickListener.onItemClick(position);
                        }
                    }
                }
            });
        }
    }
}
