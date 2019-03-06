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
                var data = $.parseJSON(data.data);
                tabitem.clear();
                tabitem.rows.add(data);
                tabitem.draw();
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



var ListaItem = function () {
    return {
        init: function () {

            //tabitem = $('#tabItem').DataTable({
            //    "searching": true,
            //    "filter": false,
            //    //"dom": 't',
            //    "autoWidth": false,
            //    "columnDefs":
            //        [
            //            { "targets": [0], "visible": false, "orderable": false },
            //            { "targets": [5], "defaultContent": '<button id="butEditar" type="button" class="btn btn-default btn-xs clsedit"><i class="fa fa-pencil"></i></button>' }                    ],
            //    "data": ldata,
            //    "columns": [
            //        { "data": 'ID_ITEM' },
            //        { "data": 'CODIGO' },
            //        { "data": 'DESCRIPCION' },
            //        { "data": 'MEDIDA' },
            //        { "data": 'GRUPO' }
            //    ]
            //});


            tabitem = $("#tabItem").DataTable({
                "autoWidth": false,
                "pageLength" : 50,
                "responsive": true,
                "processing": true,
                "serverSide": true,
                "info": true,
                "stateSave": true,
                "ajax": {
                    "url": "/Item/GetListaItem",
                    "type": "POST",
                    "error": function () {
                        console.log("error remote load data using datatable");
                    },
                    'beforeSend': function (request) {
                        run_waitMe($(".box"), 'Cargando...');
                    },
                    "complete": function (request) {
                        $(".box").waitMe('hide');
                    }
                },
                "columnDefs":
                    [
                        { "targets": [0], "visible": false, "orderable": false },
                        { "targets": [5, 6], "className": "text-right" },
                        { "targets": [5], "orderable": false },
                        { "targets": [7], "className": "text-center", "orderable": false },
                        {
                            "targets": [6],
                            render: function (data, type, row) {
                                var color = 'black';
                                if (data == 'Inactivo') {
                                    color = 'red';
                                }
                                return '<span style="color:' + color + '">' + data + '</span>'
                            }
                        }
                    ],
                "columns": [
                    { "data": "ID_ITEM"},
                    { "data": "CODIGO"},
                    { "data": "DESCRIPCION"},
                    { "data": "MEDIDA"},
                    { "data": "GRUPO"},
                    { "data": "STOCK_GENERAL"},
                    { "data": "ESTADO_REGISTRO"},
                    { "data": "ACCION"}
                ]
            });

            //CargaDatos();

            //tabitem.on('click', 'button.clsedit', function (e) {
            //    var row = $(this).closest('tr');
            //    var data = tabitem.row($(this).parents('tr')).data();
            //    window.location.href = '/Item/Item/'+data.ID_ITEM;

            //});

            $("#butNuevo").on('click', function () {
                window.location.href = '/Item/Item/0';
            });
            


        }
    };
}();