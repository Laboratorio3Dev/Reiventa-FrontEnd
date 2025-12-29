// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


    document.addEventListener('show.bs.modal', function (e) {
    const btn = e.relatedTarget;
    const id = btn.getAttribute('data-id');

    const input = e.target.querySelector('input[type="hidden"]');
    if (input) input.value = id;
});





       
        // Función que se llama cuando pulsas 'Guardar' dentro del modal
        function guardarRevisionGO() {
            var idPlanAccion = $('#modalIdPlanAccionGO').val();
            var estadoSeleccionado = $('#selectEstadoGO').val();
            var comentario = $('#comentarioGO').val();

            // Aquí debes usar AJAX/Fetch API para enviar estos datos al servidor
            console.log("ID:", idPlanAccion, "Estado:", estadoSeleccionado, "Comentario:", comentario);

            // Cierra el modal (requiere Bootstrap JS completo)
            var modalInstance = bootstrap.Modal.getInstance(document.getElementById('modalGO'));
            modalInstance.hide();
        }
