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


// Cierra el modal (requiere Bootstrap JS completo)
var modalInstance = bootstrap.Modal.getInstance(document.getElementById('modalGO'));
modalInstance.hide();
        


document.addEventListener("DOMContentLoaded", () => {
    const btn = document.getElementById("toggleSidebar");
    const sidebar = document.querySelector(".sidebar");

    if (!btn || !sidebar) return;

    btn.addEventListener("click", () => {
        sidebar.classList.toggle("collapsed");
    });
});


function ValidarFormulario(form) {

    var MsjObligatorio = "<small class='form-text text-muted field-validation-error'>Campo {0} obligatorio</small>";
    var validacion = true;
    $(".form-text").remove();
    $("#" + form + " .form-control").removeClass("hasError");
    $("#" + form + " .custom-file").removeClass("hasError");
    $("#" + form + " div").removeClass("hasError");
    $("#" + form + " .select2-selection").removeClass("hasError");
    $("#" + form + " .custom-file-label").removeClass("hasError");
    $("#" + form + " .cam-peoplepicker-userlookup").removeClass("hasError");
    $("#" + form + " input").removeClass("hasErrorCheckBox");
    $("#" + form + " div").removeClass("is-invalid");
    $("#" + form + " .required").each(function () {
        if ($(this).is(":visible")) {
            if (this.type == "checkbox") {
                if (!$(this).prop('checked')) {
                    $(this).addClass("hasErrorCheckBox");
                    validacion = false;
                }
            } else if (this.localName == "input" || this.localName == "select" || this.localName == "textarea") {
                if ($(this).val() == "" || $(this).val() == "0" || $(this).val() == "-1" || $(this).val() == "[]" || $(this).val() == null) {
                    if ($(this).hasClass("reference")) {
                        var reference = this.className.split("ref_")[1];
                        var objreference = $("#" + reference);
                        if (objreference[0].type == "checkbox") {
                            if ($(this).hasClass("controlSiNo")) {
                                if (objreference.prop('checked')) {
                                    $(this.parentElement).addClass("hasError");
                                    validacion = false;
                                }
                            } else {
                                if (!objreference.prop('checked')) {
                                    $(this).addClass("hasError");
                                    validacion = false;
                                }
                            }
                        } else {
                            objreference.addClass("hasError");
                            validacion = false;
                        }
                    } else {
                        if ($(this).hasClass("selectpicker")) {
                            $(this).parent().addClass("is-invalid");
                            var Label = $("label[for='" + $(this).attr("id") + "']").text();
                            $(this).parent().parent().append(MsjObligatorio.replace("{0}", Label.replace(":", "")));
                            validacion = false;
                        } else if ($(this).hasClass("custom-file-input")) {
                            $(this).parent().find(".custom-file-label").addClass("hasError");
                            var Label = $("label[for='" + $(this).attr("id") + "']").text();
                            $(this).parent().parent().append(MsjObligatorio.replace("{0}", Label.replace(":", "")));
                            validacion = false;
                        } else if ($(this).parent().hasClass("date")) {
                            $(this).addClass("hasError");
                            var Label = $("label[for='" + $(this).attr("id") + "']").text();
                            $(this).parent().parent().append(MsjObligatorio.replace("{0}", Label.replace(":", "")));
                            validacion = false;
                        } else if ($(this).hasClass("selectMultiple")) {
                            var id = $(this).attr("id");
                            if ($("#" + id + ">option").length <= 0) {
                                $(this).addClass("hasError");
                                var Label = $("label[for='" + $(this).attr("id") + "']").text();
                                $(this).parent().append(MsjObligatorio.replace("{0}", Label.replace(":", "")));
                                validacion = false;
                            }
                        } else {
                            $(this).addClass("hasError");
                            var Label = $("label[for='" + $(this).attr("id") + "']").text();
                            $(this).parent().append(MsjObligatorio.replace("{0}", Label.replace(":", "")));
                            validacion = false;
                        }
                    }
                } else {
                    if ($(this).attr("minlength") != undefined && $(this).attr("minlength") != null) {
                        if ($(this).val().length < $(this).attr("minlength")) {
                            $(this).addClass("hasError");
                            validacion = false;
                        }
                    }
                }
            } else if (this.localName == "p") {
                if ($(this).html() == "") {
                    $(this).addClass("hasError");
                    validacion = false;
                }
            } else if (this.localName == "table") {
                var tableid = $(this).attr("id");
                if ($("#" + tableid + " tbody").html().indexOf("No se encontraron resultados") != -1) {
                    $(this).parent().parent().addClass("hasError");
                    validacion = false;
                }
            }
        } else {
            if (this.type == "hidden") {
                if ($(this).val() == "" || $(this).val() == "0" || $(this).val() == "-1" || $(this).val() == "[]") {
                    if ($(this).hasClass("reference")) {
                        var reference = this.className.split("ref_")[1];
                        var objreference = $("#" + reference);
                        if (objreference[0].type == "checkbox") {
                            if ($(this).hasClass("controlSiNo")) {
                                if (objreference.prop('checked')) {
                                    $(this.parentElement).addClass("hasError");
                                    validacion = false;
                                }
                            } else {
                                if (!objreference.prop('checked')) {
                                    $(this).addClass("hasError");
                                    validacion = false;
                                }
                            }
                        } else {
                            objreference.parent().addClass("hasError");
                            var Label = $("label[for='" + $(this).attr("id") + "']").text();
                            objreference.parent().parent().append(MsjObligatorio.replace("{0}", Label.replace(":", "")));
                            validacion = false;
                        }
                    } else if ($(this).hasClass("cam-peoplepicker-inputvalue")) {
                        $(this).parent().find(".cam-peoplepicker-userlookup").addClass("hasError");
                        var Label = "";
                        $(this).parent().append(MsjObligatorio.replace("{0}", Label.replace(":", "")));
                        validacion = false;
                    }
                }
            }
        }
    });

    return validacion;
}