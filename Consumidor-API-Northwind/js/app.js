const API_BASE_KEY = "northwind_api_base";
const TOKEN_KEY = "northwind_api_token";
const USER_KEY = "northwind_api_user";

const apiUrlInput = document.getElementById("apiUrl");
const btnGuardarUrl = document.getElementById("btnGuardarUrl");
const btnPing = document.getElementById("btnPing");
const linkSwagger = document.getElementById("linkSwagger");
const btnLoginGoogle = document.getElementById("btnLoginGoogle");
const btnLogout = document.getElementById("btnLogout");
const userInfo = document.getElementById("userInfo");
const btnCargar = document.getElementById("btnCargar");
const tablaBody = document.querySelector("#tablaProductos tbody");
const sinDatos = document.getElementById("sinDatos");
const googleConfigInfo = document.getElementById("googleConfigInfo");
const alertZone = document.getElementById("alertZone");

function getBaseUrl() {
    let url = localStorage.getItem(API_BASE_KEY) || "";
    if (!url) {
        url = prompt("Introduce la URL base de la API (ej: https://xxxx.ngrok-free.app o http://IP:5080)");
        if (url) {
            url = url.replace(/\/$/, "");
            localStorage.setItem(API_BASE_KEY, url);
        }
    }
    return url;
}

function setBaseUrl(url) {
    if (!url) return;
    url = url.replace(/\/$/, "");
    localStorage.setItem(API_BASE_KEY, url);
    apiUrlInput.value = url;
    actualizarEnlaces();
}

function getToken() { return localStorage.getItem(TOKEN_KEY); }
function getUser() {
    try { return JSON.parse(localStorage.getItem(USER_KEY) || "null"); } catch { return null; }
}

function mostrarAlerta(msg, tipo) {
    if (tipo == null) tipo = "warning";
    alertZone.innerHTML = '<div class="alert alert-' + tipo + ' alert-dismissible fade show" role="alert">' + msg + '<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>';
}

function actualizarEnlaces() {
    const base = getBaseUrl();
    if (!base) return;
    apiUrlInput.value = base;
    linkSwagger.href = base + "/swagger/index.html";
    linkSwagger.classList.remove("disabled");
    btnLoginGoogle.classList.remove("d-none");
    btnCargar.disabled = !getToken();
}

async function ping() {
    const base = getBaseUrl();
    if (!base) return;
    try {
        const res = await fetch(base + "/", { method: "GET" });
        const data = await res.json();
        mostrarAlerta("Conexion OK: " + data.name + " (" + data.status + ")", "success");
        cargarGoogleConfig();
    } catch (e) {
        mostrarAlerta("No se pudo conectar con la API.", "danger");
    }
}

async function cargarGoogleConfig() {
    const base = getBaseUrl();
    try {
        const res = await fetch(base + "/api/Auth/google-config");
        const data = await res.json();
        if (data.configured && data.clientId) {
            googleConfigInfo.innerHTML = "Google OAuth configurado en el servidor.";
            inicializarGoogle(data.clientId);
        } else {
            googleConfigInfo.innerHTML = "Google OAuth no configurado en el servidor.";
        }
    } catch (e) {
        googleConfigInfo.innerHTML = "No se pudo obtener configuracion.";
    }
}

function inicializarGoogle(clientId) {
    if (!window.google || !google.accounts) return;
    google.accounts.id.initialize({
        client_id: clientId,
        callback: handleGoogleResponse
    });
    const btn = document.getElementById("btnLoginGoogle");
    btn.innerHTML = '<i class="bi bi-google"></i> Login con Google';
    google.accounts.id.renderButton(btn, { theme: "outline", size: "medium", text: "signin_with" });
}

async function handleGoogleResponse(response) {
    const base = getBaseUrl();
    try {
        const res = await fetch(base + "/api/Auth/google", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ idToken: response.credential })
        });
        if (!res.ok) throw new Error("Error");
        const data = await res.json();
        localStorage.setItem(TOKEN_KEY, data.token);
        localStorage.setItem(USER_KEY, JSON.stringify(data.user));
        actualizarSesion();
        mostrarAlerta("Login correcto con Google", "success");
        btnCargar.disabled = false;
    } catch (e) {
        mostrarAlerta("Error en login con Google", "danger");
    }
}

function actualizarSesion() {
    const user = getUser();
    if (user) {
        userInfo.textContent = user.email || user.name || "Usuario";
        userInfo.classList.remove("d-none");
        btnLoginGoogle.classList.add("d-none");
        btnLogout.classList.remove("d-none");
        btnCargar.disabled = false;
    } else {
        userInfo.classList.add("d-none");
        btnLoginGoogle.classList.remove("d-none");
        btnLogout.classList.add("d-none");
        btnCargar.disabled = true;
    }
}

async function cargarProductos() {
    const base = getBaseUrl();
    const token = getToken();
    if (!token) { mostrarAlerta("Necesitas iniciar sesion con Google", "warning"); return; }
    try {
        const res = await fetch(base + "/api/Products", {
            headers: { "Authorization": "Bearer " + token }
        });
        if (res.status === 401) { mostrarAlerta("Token invalido o expirado", "warning"); localStorage.removeItem(TOKEN_KEY); localStorage.removeItem(USER_KEY); actualizarSesion(); return; }
        if (!res.ok) throw new Error("Error");
        const productos = await res.json();
        tablaBody.innerHTML = "";
        if (!productos.length) { sinDatos.classList.remove("d-none"); return; }
        sinDatos.classList.add("d-none");
        for (var i = 0; i < productos.length; i++) {
            var p = productos[i];
            var tr = document.createElement("tr");
            tr.innerHTML = "<td>" + p.productId + "</td><td>" + p.productName + "</td><td>" + (p.supplierId || "") + "</td><td>" + (p.categoryId || "") + "</td><td>$" + Number(p.unitPrice||0).toFixed(2) + "</td><td>" + (p.unitsInStock || "") + "</td>";
            tablaBody.appendChild(tr);
        }
    } catch (e) {
        mostrarAlerta("Error al cargar productos", "danger");
    }
}

btnGuardarUrl.addEventListener("click", function(){ setBaseUrl(apiUrlInput.value.trim()); });
btnPing.addEventListener("click", ping);
btnCargar.addEventListener("click", cargarProductos);
btnLogout.addEventListener("click", function(){ localStorage.removeItem(TOKEN_KEY); localStorage.removeItem(USER_KEY); actualizarSesion(); tablaBody.innerHTML=""; sinDatos.classList.remove("d-none"); });

window.addEventListener("load", function(){
    var base = getBaseUrl();
    if (base) { apiUrlInput.value = base; actualizarEnlaces(); ping(); }
    actualizarSesion();
});