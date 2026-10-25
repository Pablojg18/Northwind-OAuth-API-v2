# Cliente para consumir Northwind OAuth API

Este proyecto está preparado para trabajar con la API Northwind.

## Instrucciones rápidas

1. Lanza la API Northwind en el equipo que la aloje.
2. Abre src/api/client.js con un editor.
3. **Modifica ÚNICAMENTE la línea 1** para poner la URL de la API:

   `js
   const BASE = import.meta.env.VITE_API_URL || 'http://localhost:5080';
   `

   Cambia http://localhost:5080 por:
   - http://IP_DEL_SERVIDOR:5080 si está en otra máquina de tu red
   - https://TU-URL-NGROK.ngrok-free.app si lo expones con ngrok

4. Instala y ejecuta:
   `ash
   npm install
   npm run dev
   `

5. Abre http://localhost:5173

No es necesario tocar nada más. Con eso cualquier persona puede lanzar este cliente y conectarse a la API.