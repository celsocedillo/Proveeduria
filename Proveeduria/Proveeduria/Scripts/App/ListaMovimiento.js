var tabMovimiento;
var ldata = [];
var parametros;

function CargaDatos(parametros) {
    
    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: parametros,
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

            $.fn.dataTable.moment('DD/MM/YYYY');
            tabMovimiento = $('#tabMovimiento').DataTable({
                "autoWidth": false,
                "pageLength" : 25,
                "data": ldata,
                "order": [[6, "desc"]],
                //"columnDefs":
                //    [
                //        { "targets": [1], "className": "text-right" }
                //    ],
                "columns": [
                    { "data": 'ANIO' },
                    { "data": 'NUMERO_MOVIMIENTO' },
                    { "data": 'MOVIMIENTO' },
                    { "data": 'OBSERVACION' },
                    { "data": 'NOMBREESTADO' },
                    { "data": 'USUARIO_SOLICITA' },
                    { "data": 'FECHA_SOLICITUD' },
                    { "data": "ACCION", "orderable": true }
                ]
            });

            $("#txtFecIni").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });

            $("#txtFecFin").datepicker({
                format: "dd/mm/yyyy",
                todayHighlight: true,
                autoclose: true
            });
            var fecha1 = new Date();
            var fecha2 = new Date();
            fecha1.setDate(fecha2.getDate() - 30);
            parametros = JSON.stringify(
                {
                    pfecha_inicio: fecha1,
                    pfecha_fin: fecha2,
                    pid_tipo_movimiento :0
                });
            $("#txtFecIni").val(fecha1.toJSON().slice(0, 10).split("-").reverse().join("/"));
            $("#txtFecFin").val(fecha2.toJSON().slice(0, 10).split("-").reverse().join("/"));

            CargaDatos(parametros);

            $("#butNuevo").on('click', function () {
                window.location.href = '/Movimiento/Movimiento/0';
            });

            $("#btnConsultar").on('click', function () {
                parametros = JSON.stringify(
                    {
                        pfecha_inicio: $("#txtFecIni").val(),
                        pfecha_fin: $("#txtFecFin").val(),
                        pid_tipo_movimiento: $("#selIdTipoMovimiento").val()
                    });
                CargaDatos(parametros);
            });
        }
    };
}();