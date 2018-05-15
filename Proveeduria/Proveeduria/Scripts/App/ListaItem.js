var tabitem;
var ldata = [];
var tipomov;


function CargaDatos() {

    $.ajax({
        type: "POST",
        traditional: true,
        datatype: "json",
        data: null,
        contentType: 'application/json; charset=utf-8',
        url: "/Item/GetListaItem",
        beforeSend: function () {
            run_waitMe($(".box-body"), 'Cargando...');
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
                var data = $.parseJSON(data.data);
                tabitem.clear();
                tabitem.rows.add(data);
                tabitem.draw();
            }
            $(".box-body").waitMe('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".box-body").waitMe('hide');
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



var ListaItem = function () {
    return {
        init: function () {

            tabitem = $('#tabItem').DataTable({
                "searching": true,
                "filter": false,
                //"dom": 't',
                "autoWidth": false,
                "columnDefs":
                    [
                        { "targets": [0], "visible": false, "orderable": false },
                        { "targets": [5], "defaultContent": '<button id="butEditar" type="button" class="btn btn-default btn-xs clsedit"><i class="fa fa-pencil"></i></button>' }                    ],
                "data": ldata,
                "columns": [
                    { "data": 'ID_ITEM' },
                    { "data": 'CODIGO' },
                    { "data": 'DESCRIPCION' },
                    { "data": 'MEDIDA' },
                    { "data": 'GRUPO' }
                ]
            });

            CargaDatos();

            tabitem.on('click', 'button.clsedit', function (e) {
                var row = $(this).closest('tr');
                var data = tabitem.row($(this).parents('tr')).data();
                window.location.href = '/Item/Item/'+data.ID_ITEM;

            });

            $("#butNuevo").on('click', function () {
                window.location.href = '/Item/Item/0';
            });
            


        }
    };
}();