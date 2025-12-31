// ===============================
// BOOTSTRAP MODAL (GO)
// ===============================
document.addEventListener('show.bs.modal', function (e) {
    const btn = e.relatedTarget;
    const id = btn.getAttribute('data-id');

    const input = e.target.querySelector('input[type="hidden"]');
    if (input) input.value = id;
});


// ===============================
// DOM READY
// ===============================
document.addEventListener("DOMContentLoaded", function () {

    // ===============================
    // SUBMENÚS SIDEBAR
    // ===============================
    document.querySelectorAll(".menu-title").forEach(title => {
        title.addEventListener("click", function () {
            this.parentElement.classList.toggle("open");
        });
    });

    // ===============================
    // SIDEBAR RESPONSIVE
    // ===============================
    const toggle = document.querySelector(".menu-toggle");
    const sidebar = document.querySelector(".sidebar");
    const overlay = document.querySelector(".sidebar-overlay");

    if (toggle && sidebar && overlay) {

        toggle.addEventListener("click", function () {
            sidebar.classList.toggle("open");
            overlay.classList.toggle("show");
        });

        overlay.addEventListener("click", function () {
            sidebar.classList.remove("open");
            overlay.classList.remove("show");
        });
    }

    // ===============================
    // VALIDACIÓN DNI (8 DÍGITOS)
    // ===============================
    const dniInput = document.getElementById("dniInput");
    const btnBuscar = document.getElementById("btnBuscar");

    if (dniInput && btnBuscar) {

        dniInput.addEventListener("input", function () {

            // Solo números
            this.value = this.value.replace(/\D/g, "");

            // Botón activo solo con 8 dígitos
            btnBuscar.disabled = this.value.length !== 8;
        });
    }
});


// ===============================
// GUARDAR REVISIÓN GO
// ===============================
function guardarRevisionGO() {

    var idPlanAccion = $('#modalIdPlanAccionGO').val();
    var estadoSeleccionado = $('#selectEstadoGO').val();
    var comentario = $('#comentarioGO').val();

    console.log("ID:", idPlanAccion, "Estado:", estadoSeleccionado, "Comentario:", comentario);

    var modalInstance = bootstrap.Modal.getInstance(
        document.getElementById('modalGO')
    );

    modalInstance.hide();
}


document.querySelectorAll(".toggle").forEach(btn => {
        btn.addEventListener("click", () => {
            const input = btn.previousElementSibling;
            input.type = input.type === "password" ? "text" : "password";
            btn.firstElementChild.classList.toggle("bi-eye");
            btn.firstElementChild.classList.toggle("bi-eye-slash");
        });
});

document.getElementById("passwordNueva").addEventListener("input", e => {
    const v = e.target.value;
    set("min", v.length >= 8);
    set("mayus", /[A-Z]/.test(v));
    set("minus", /[a-z]/.test(v));
    set("num", /\d/.test(v));
    set("esp", /[!@#$%^&*()_+=\-{ };:,.<>?]/.test(v));
});

        function set(rule, ok){
    const li = document.querySelector(`[data-rule='${rule}']`);
        li.style.color = ok ? "green" : "red";
        li.textContent = (ok ? "✔ " : "✘ ") + li.textContent.substring(2);
}

