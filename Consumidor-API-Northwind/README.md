# Consumidor Northwind API

Mini proyecto para consumir la Northwind OAuth API desde otro PC.

## Uso rapido

1. Lanza la API (con iniciar-api.cmd o ngrok)
2. Abre este proyecto con un servidor estatico (http-server, Live Server)
3. Pega la URL base: `https://xxxx.ngrok-free.app` o `http://IP:5080`
4. Pulsa Comprobar conexion
5. Login con Google
6. Cargar productos

## Servir local

Con Node.js:
```bash
npx http-server . -p 8080
```

Con Python:
```bash
python -m http.server 8080
```

Abrir: http://localhost:8080