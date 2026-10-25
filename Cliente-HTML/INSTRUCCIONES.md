# Cliente HTML para consumir Northwind OAuth API

Este cliente HTML está listo para conectarse a la API.

## Cómo usarlo

1. Lanza la API Northwind (http://localhost:5080 o con ngrok/IP)
2. Abre el archivo js/config.js (o el archivo JS principal donde se defina la URL)
3. **Modifica ÚNICAMENTE la línea donde se indique la URL base** por la de tu API

   Busca algo como:
   `js
   const API_BASE_URL = "http://localhost:5080";
   `
   o
   `js
   const BASE_URL = "http://localhost:5080";
   `

   Cámbiala por:
   - http://localhost:5080 (local)
   - http://IP_DEL_SERVIDOR:5080 (otra máquina en red)
   - https://TU-URL-NGROK.ngrok-free.app (ngrok)

4. Abre index.html con un servidor estático (Live Server, http-server)

Con CORS abierto en la API no habrá problemas.