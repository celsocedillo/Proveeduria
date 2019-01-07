var tabCierre;
var ldata = [];


function CargaDatos() {

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Consulta/GetListaCierres",
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
                tabCierre.clear();
                tabCierre.rows.add(data.data);
                tabCierre.draw();
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



var ListaCierreInventario = function () {
    return {
        init: function () {

            //$.fn.dataTable.moment('D-M-Y');
            $.fn.dataTable.moment('DD/MM/YYYY');
            tabCierre= $('#tabCierre').DataTable({
                "autoWidth": false,
                "data": ldata,
                "columnDefs":
                    [
                        { "targets": [2, 3], "className": "text-right" },
                        { "targets": [4], "className": "text-center" }
                    ],
                "columns": [
                    { "data": 'BODEGA' },
                    { "data": 'FECHA_CIERRE' },
                    { "data": 'NUMERO_ITEMS' },
                    { "data": 'TOTAL_CIERRE' },
                    { "data": "ACCION", "orderable": true }
                ]
            });

            CargaDatos();

            $("#butNuevo").on('click', function () {
                window.location.href = '/Consulta/CierreInventario/0';
            });

        }
    };
}();