var tabItems;


function GenerarCorte() {
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Consulta/GenerarCorteInventario",
        beforeSend: function () {
            run_waitMe($(".box-primary"), 'Generando...');
        },
        success: function (data) {
            if (data.resultado === "success") {
                $("#FECHA_CIERRE").val(data.data.FECHA_CIERRE);
                $("#USUARIO_CIERRE").val(data.data.USUARIO_CIERRE);
                tabItems.clear();
                tabItems.rows.add(data.data.DETALLE);
                tabItems.draw();                
                $("#btnGenerar").prop("disabled", true);
            }
            else if (data.resultado === "error") {
                swal({
                    type: data.resultado,
                    text: 'Error al generar corte de inventario. ' + data.mensaje,
                    confirmButtonColor: '#00BCD4'
                });
            }
            $(".box-primary").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-primary").waitMe('hide');
            swal({
                type: 'error',
                text: 'Error al cargar los datos.',
                confirmButtonColor: '#00BCD4'
            });
        },
        complete: function () {
        }
    });
}

var CierreInventario = function () {
    return {
        init: function () {

            
            tabItems= $('#tabItems').DataTable({
                "autoWidth": false,
                "paging": false,
                "data": items,
                "order": [[1, 'asc']],
                "columnDefs": [{ "targets": [2, 3 ],  "className": "text-right" }],
                "columns": [
                    { "data": 'CODIGO' },
                    { "data": 'ITEM' },
                    { "data": 'CANTIDAD_ACTUAL' },
                    { "data": 'COSTO_PROMEDIO' }
                ]
            });

            $("#btnCancelar").on('click', function () {
                window.location.href = '/Consulta/ListaCierreInventario';
            });

            $("#btnGenerar").on('click', function () {
                swal({
                    text: '¿Seguro desea generar el corte de inventario?',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#00BCD4',
                    cancelButtonColor: '#EF5350',
                    confirmButtonText: 'Aceptar',
                    cancelButtonText: 'Cancelar'
                }).then(function () {
                    GenerarCorte();
                });
            });

        }
    };
}();