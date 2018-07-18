var tabMovimiento;
var ldata = [];

function CargaDatos() {

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Movimiento/GetListaMovimiento",
        beforeSend: function () {
            run_waitMe($(".box"), 'Cargando...');
        },
        success: function (data) {
            if (data.error) {
                swal({
                    type: 'error',
                    text: 'Error al cargar los datos.',
                    confirmButtonColor: '#00BCD4'
                });
            }
            else {
                //var data = $.parseJSON(data.data);
                tabMovimiento.clear();
                tabMovimiento.rows.add(data.data);
                tabMovimiento.draw();
            }
            $(".box").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box").waitMe('hide');
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

var ListaMovimiento = function () {
    return {
        init: function () {

            //$.fn.dataTable.moment('D-M-Y');
            $.fn.dataTable.moment('DD/MM/YYYY');
            tabMovimiento = $('#tabMovimiento').DataTable({
                "autoWidth": false,
                "data": ldata,
                "order": [[6, "desc"]],
                "columnDefs":
                    [
                        { "targets": [1], "className": "text-right" }
                    ],
                "columns": [
                    { "data": 'ANIO' },
                    { "data": 'NUMERO_MOVIMIENTO' },
                    { "data": 'MOVIMIENTO' },
                    { "data": 'OBSERVACION' },
                    { "data": 'NOMBREESTADO' },
                    { "data": 'USUARIO_SOLICITA' },
                    { "data": 'FECHA_SOLICITUD' },
                    { "data": 'FECHA_APROBACION' },
                    { "data": "ACCION", "orderable": true }
                ]
            });

            CargaDatos();

            $("#butNuevo").on('click', function () {
                window.location.href = '/Movimiento/Movimiento/0';
            });
        }
    };
}();