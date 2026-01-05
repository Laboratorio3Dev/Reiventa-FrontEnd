window.encuestaInicio = {
    setDotNetHelper: function (dotNetHelper) {
        window._dotNetHelper = dotNetHelper;
    },

    init: function () {

        // validar dni
        function validardni() {
            var dniInput = document.getElementById("dni");
            if (!dniInput) return;

            var dni = dniInput.value;
            var expregdni = "^[0-9]{8}$";
            var errorDNI = document.getElementById("errorDNI");

            if (dni.match(expregdni)) {
                errorDNI?.classList.add("banbif-none");
                dniInput.classList.remove("banbif-input-error");
            } else {
                errorDNI?.classList.remove("banbif-none");
                dniInput.classList.add("banbif-input-error");
            }
        }

        var dniEl = document.getElementById("dni");
        var btnNext = document.getElementById("btnNext");
        var condicion1 = document.getElementById("condicion1");

        if (!dniEl || !btnNext || !condicion1) return;

        // keyup / focusout
        var focus = 0;
        dniEl.addEventListener("focus", function () {
            focus++;
            if (focus > 1) {
                dniEl.addEventListener("keyup", validardni);
            }
        });

        dniEl.addEventListener("focusout", validardni);

        // checkbox condicion1 message
        condicion1.addEventListener("change", function () {
            var messageCheck = document.querySelector(".message-check");
            if (condicion1.checked) messageCheck?.classList.add("banbif-none");
            else messageCheck?.classList.remove("banbif-none");
        });

        // bg height
        window.addEventListener("resize", setBgImageHeight);
        window.addEventListener("load", setBgImageHeight);

        // click del botón (NO submit)
        btnNext.addEventListener("click", function (event) {
            event.preventDefault();

            validardni();
            dniEl.addEventListener("keyup", validardni);

            var messageCheck = document.querySelector(".message-check");
            if (condicion1.checked) messageCheck?.classList.add("banbif-none");
            else messageCheck?.classList.remove("banbif-none");

            var dni = dniEl.value;

            if (dni.length === 0 || !condicion1.checked) {
                return false;
            }

            // ✅ solo si valida, llama a .NET
            if (window._dotNetHelper) {
                window._dotNetHelper.invokeMethodAsync('EnviarDniDesdeJs', dni);
            } else {
                console.warn("DotNetHelper no está seteado.");
            }

            return true;
        });
    }
};


// tus funciones globales tal cual las tenías
function validaNumericos(event) {
    if (event.charCode >= 48 && event.charCode <= 57) return true;
    return false;
}

function validaCaracteres(event) {
    var charCode = event.charCode;
    if (
        (charCode >= 65 && charCode <= 90) ||
        (charCode >= 97 && charCode <= 122) ||
        charCode === 32 ||
        (charCode >= 192 && charCode <= 255)
    ) return true;

    return false;
}

function setBgImageHeight() {
    const headerText = document.getElementById('header-text');
    const bgImageForm = document.querySelector('.banbif-bg-image-form');
    if (window.innerWidth < 768 && headerText && bgImageForm) {
        bgImageForm.style.height = headerText.offsetHeight + 'px';
    } else if (bgImageForm) {
        bgImageForm.style.height = '';
    }
}