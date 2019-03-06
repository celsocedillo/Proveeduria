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
                $("#FECHA_CORTE").val(data.data.FECHA_CORTE);
                $("#USUARIO_CORTE").val(data.data.USUARIO_CORTE);
                tabItems.clear();
                tabItems.rows.add(data.data.DETALLE);
                tabItems.draw();                
                $("#btnGenerar").prop("disabled", true);
                $("#divButtons").show();
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

function Filtro(pfiltro) {
    parametros = JSON.stringify(
        {
            pid_corte: $("#ID_CORTE").val(),
            pfiltro: pfiltro
        });
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
        contentType: 'application/json; charset=utf-8',
        url: "/Consulta/CorteInventarioFiltro",
        beforeSend: function () {
            run_waitMe($(".box-default"), 'Generando...');
        },
        success: function (data) {
            if (data.resultado === "success") {
                tabItems.clear();
                tabItems.rows.add(data.data);
                tabItems.draw();
            }
            else if (data.resultado === "error") {
                swal({
                    type: data.resultado,
                    text: 'Error al filtrar corte de inventario. ' + data.mensaje,
                    confirmButtonColor: '#00BCD4'
                });
            }
            $(".box-default").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-default").waitMe('hide');
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

var CorteInventario = function () {
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

            if ($("#ID_CORTE").val() == 0) {
                $("#divButtons").hide();
            }

            $("#btnCancelar").on('click', function () {
                window.location.href = '/Consulta/ListaCorteInventario';
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

            $('input[type=radio][name=oprSaldo]').change(function () {
                Filtro(this.value);
            });

            $("#btnExcel").click(function (event) {
                window.location.href = '/Consulta/CorteInventarioExcel?pid_corte=' + $("#ID_CORTE").val() + '&pfiltro=' + $('input[type=radio][name=oprSaldo]:checked').val();
            });

        }
    };
}();